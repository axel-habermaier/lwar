using System;

namespace Lwar.Client.Network
{
	using System.Net;
	using System.Threading.Tasks;
	using Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Processes;

	/// <summary>
	///   Represents a proxy of an lwar server that a client can use to communicate with the server.
	/// </summary>
	public class ServerProxy : DisposableObject
	{
		/// <summary>
		///   The maximum allowed length of an UTF8-encoded player name, including the terminating 0 character.
		/// </summary>
		private const int MaxPlayerNameLength = 32;

		/// <summary>
		///   The maximum allowed length of an UTF8-encoded chat message, including the terminating 0 character.
		/// </summary>
		private const int MaxChatMessageLength = 128;

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
		///   The delivery manager that is used to enforce the delivery guarantees of all incoming and outgoing messages.
		/// </summary>
		private readonly DeliveryManager _deliveryManager = new DeliveryManager();

		/// <summary>
		///   The process that handles incoming packets from the server.
		/// </summary>
		private readonly IProcess _receiveProcess;

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
		///   Raised when a message has been received from the server.
		/// </summary>
		public event Action<IMessage> MessageReceived;

		/// <summary>
		///   Sends a Connect message to the server.
		/// </summary>
		public async Task Connect(ProcessContext context)
		{
			Assert.That(_state == State.Disconnected, "The proxy is not disconnected.");
			_state = State.Connecting;

			try
			{
				var attempts = 0;
				NetworkLog.ClientInfo("Connecting to {0}.", _serverEndPoint);

				while (_state != State.Connected && attempts < MaxConnectionAttempts)
				{
					var packet = OutgoingPacket.Create();
					packet.Writer.WriteUInt16(3);
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

						foreach (var message in MessageDeserializer.Deserialize(packet, _deliveryManager))
							MessageReceived(message);
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