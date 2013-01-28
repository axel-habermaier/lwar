using System;

namespace Lwar.Client.Network
{
	using System.Net;
	using System.Threading.Tasks;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Processes;
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Represents a proxy of an lwar server that a client can use to communicate with the server.
	/// </summary>
	public class ServerProxy : DisposableObject
	{
		/// <summary>
		///   The maximum allowed length of an ASCII player name, including the terminating 0 character.
		/// </summary>
		private const int MaxPlayerNameLength = 32;

		/// <summary>
		///   The maximum allowed length of an ASCII chat message, including the terminating 0 character.
		/// </summary>
		private const int MaxChatMessageLength = 500;

		/// <summary>
		///   The maximum number of connection attempts before giving up.
		/// </summary>
		private const int MaxConnectionAttempts = 10;

		/// <summary>
		///   The delay between two connection attempts in milliseconds.
		/// </summary>
		private const int RetryDelay = 100;

		/// <summary>
		///   The default server port.
		/// </summary>
		public const ushort DefaultPort = 32422;

		/// <summary>
		///   The application identifier that is used to determine whether a Udp packet was sent by another application.
		/// </summary>
		public const uint AppId = 0xf27087c5;

		/// <summary>
		///   The endpoint of the server.
		/// </summary>
		private readonly IPEndPoint _serverEndPoint;

		/// <summary>
		///   The Udp socket that is used for the communication with the server.
		/// </summary>
		private readonly UdpSocket _socket = new UdpSocket();

		/// <summary>
		///   The current state of the virtual connection to the server.
		/// </summary>
		private State _state = State.Disconnected;

		/// <summary>
		/// The process that handles incoming packets from the server.
		/// </summary>
		private IProcess _receiveProcess;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The endpoint of the server.</param>
		/// <param name="scheduler">The scheduler that should be used to schedule the proxy's internal processes.</param>
		public ServerProxy(IPEndPoint serverEndPoint, ProcessScheduler scheduler)
		{
			Assert.ArgumentNotNull(serverEndPoint, () => serverEndPoint);
			Assert.ArgumentNotNull(scheduler, () => scheduler);

			_serverEndPoint = serverEndPoint;
			_receiveProcess = scheduler.CreateProcess(Receive);
		}

		/// <summary>
		///   Gets a value indicating whether a connection to the server is established.
		/// </summary>
		public bool IsConnected
		{
			get { return _state == State.Connected; }
		}

		/// <summary>
		///   Sends a Connect message to the server.
		/// </summary>
		public async Task Connect(ProcessContext context)
		{
			Assert.That(_state == State.Disconnected, "The proxy is not disconnected.");

			try
			{
				var attempts = 0;
				NetworkLog.ClientInfo("Connecting to {0}.", _serverEndPoint);

				while (_state != State.Connected && attempts < MaxConnectionAttempts)
				{
					var packet = OutgoingPacket.Create();
					packet.Writer.WriteUInt32(AppId);
					packet.Writer.WriteByte((byte)MessageType.Connect);
					packet.Writer.WriteInt32(0);
					packet.Writer.WriteIdentifier(new Identifier()); // Required, but unused
					packet.Writer.WriteString(Cvars.PlayerName.Value, MaxPlayerNameLength);

					_state = State.Connecting;
					++attempts;

					await _socket.SendAsync(context, packet, _serverEndPoint);
					await context.Delay(RetryDelay);
				}

				if (attempts >= MaxConnectionAttempts)
				{
					_state = State.Faulted;
					NetworkLog.ClientError("Failed to connect to {0}. No response.", _serverEndPoint);
				}
				else
					NetworkLog.ClientInfo("Connected to {0}.", _serverEndPoint);
			}
			catch (SocketOperationException e)
			{
				_state = State.Faulted;
				NetworkLog.ClientError("Unable to establish a connection to the server: {0}", e.Message);
			}
		}

		/// <summary>
		///   Handles incoming packets from the server.
		/// </summary>
		/// <param name="context">The context in which the incoming packets should be handled.</param>
		private async Task Receive(ProcessContext context)
		{
			Assert.That(_state == State.Disconnected, "Must be called before trying to establish a connection to the server.");

			var sender = new IPEndPoint(IPAddress.Any, 0);
			while (!context.IsCanceled)
			{
				if (_state == State.Faulted)
					break;

				if (_state == State.Disconnected)
				{
					await context.NextFrame();
					continue;
				}

				try
				{
					using (var packet = await _socket.ReceiveAsync(context, sender))
					{
						_state = State.Connected;

						if (!sender.Equals(_serverEndPoint))
						{
							NetworkLog.ClientDebug("Received a packet from {0}, but expecting packets from server {1} only. Packet was ignored.",
												   sender, _serverEndPoint);
							continue;
						}
					}
				}
				catch (SocketOperationException e)
				{
					NetworkLog.ClientDebug(e.Message);
					// Ignore the error as Udp is connectionless anyway
				}
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_receiveProcess.SafeDispose();
			_socket.SafeDispose();
		}

		/// <summary>
		///   Describes the state of the virtual connection to the server.
		/// </summary>
		private enum State
		{
			/// <summary>
			///   Indicates that the connection is not established.
			/// </summary>
			Disconnected,

			/// <summary>
			///   Indicates that a connection attempt has been started.
			/// </summary>
			Connecting,

			/// <summary>
			///   Indicates that a connection is established.
			/// </summary>
			Connected,

			/// <summary>
			///   Indicates that a connection is faulted due to an error and can no longer be used to send and receive any data.
			/// </summary>
			Faulted
		}
	}
}