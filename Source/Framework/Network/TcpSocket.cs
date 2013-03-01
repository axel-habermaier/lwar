using System;

namespace Pegasus.Framework.Network
{
	using System.Net;
	using System.Net.Sockets;
	using System.Threading.Tasks;
	using Processes;

	/// <summary>
	///   Represents a bi-directional Tcp-based socket connection that can be used to reliably send and receive packets over
	///   the network.
	/// </summary>
	internal class TcpSocket : DisposableObject
	{
		/// <summary>
		///   The number of bytes used to transmit the packet size.
		/// </summary>
		private const int SizeByteCount = sizeof(ushort);

		/// <summary>
		///   Used to create incoming and outgoing packets.
		/// </summary>
		private readonly IPacketFactory _packetFactory;

		/// <summary>
		///   The underlying socket that is used to send and receive packets.
		/// </summary>
		private Socket _socket;

		/// <summary>
		///   The current connection state.
		/// </summary>
		private State _state;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="packetFactory">The packet factory that should be used to create incoming and outgoing packets.</param>
		internal TcpSocket(IPacketFactory packetFactory)
		{
			Assert.ArgumentNotNull(packetFactory, () => packetFactory);

			_packetFactory = packetFactory;
			_state = State.Disconnected;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="packetFactory">The packet factory that should be used to create incoming and outgoing packets.</param>
		/// <param name="socket">The underlying socket that should be used to send and receive packets.</param>
		internal TcpSocket(IPacketFactory packetFactory, Socket socket)
		{
			Assert.ArgumentNotNull(packetFactory, () => packetFactory);
			Assert.ArgumentNotNull(socket, () => socket);

			_packetFactory = packetFactory;
			_socket = socket;
			_state = socket.Connected ? State.Connected : State.Faulted;
			RemoteEndPoint = (IPEndPoint)_socket.RemoteEndPoint;
			LocalEndPoint = (IPEndPoint)_socket.LocalEndPoint;
		}

		/// <summary>
		///   Gets the remote endpoint.
		/// </summary>
		public IPEndPoint RemoteEndPoint { get; private set; }

		/// <summary>
		///   Gets the local endpoint.
		/// </summary>
		public IPEndPoint LocalEndPoint { get; private set; }

		/// <summary>
		///   Gets a value that indicates whether the connection to the remote endpoint is established.
		/// </summary>
		public bool IsConnected
		{
			get { return _state == State.Connected; }
		}

		/// <summary>
		///   Gets a value that indicates whether the connection to the remote endpoint is being established.
		/// </summary>
		public bool IsConnecting
		{
			get { return _state == State.Connecting; }
		}

		/// <summary>
		///   Gets a value that indicates whether the connection to the remote endpoint is not established.
		/// </summary>
		public bool IsDisconnected
		{
			get { return _state == State.Disconnected; }
		}

		/// <summary>
		///   Gets a value that indicates whether the connection is faulted due to an error and can no longer be used to send and
		///   receive any data.
		/// </summary>
		public bool IsFaulted
		{
			get { return _state == State.Faulted; }
		}

		/// <summary>
		///   Creates a new socket instance that uses Tcp as the transport protocol.
		/// </summary>
		internal static Socket Create()
		{
			var socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
			socket.EnableDualMode();
			return socket;
		}

		/// <summary>
		///   Tries to establish a connection to the given endpoint.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="remoteEndPoint">The remote endpoint of the connection.</param>
		public async Task ConnectAsync(ProcessContext context, IPEndPoint remoteEndPoint)
		{
			Assert.ArgumentNotNull(remoteEndPoint, () => remoteEndPoint);
			Assert.That(!IsConnected && !IsConnecting, "Already connected or connecting.");

			_socket.SafeDispose();
			_socket = Create();
			RemoteEndPoint = remoteEndPoint;
			_state = State.Connecting;

			try
			{
				await _socket.ConnectAsync(context, remoteEndPoint);

				_state = State.Connected;
				LocalEndPoint = (IPEndPoint)_socket.LocalEndPoint;
			}
			catch (SocketException e)
			{
				_state = State.Disconnected;
				throw new SocketOperationException("Failed to connect to {0}.", e, remoteEndPoint);
			}
		}

		/// <summary>
		///   Sends the given packet sent over the connection.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="packet">The packet that should be sent. The packet is returned to the pool before this function returns.</param>
		public async Task SendAsync(ProcessContext context, OutgoingPacket packet)
		{
			Assert.ArgumentNotNull(packet, () => packet);
			Assert.That(IsConnected, "Cannot send: Not connected.");
			Assert.InRange(packet.Size, 1, Packet.MaxSize);

			using (packet)
			{
				try
				{
					await SendPackSizeAsync(context, packet.Size);
					await _socket.SendAsync(context, new ArraySegment<byte>(packet.Data, 0, packet.Size));
				}
				catch (SocketException e)
				{
					_state = State.Faulted;
					throw new SocketOperationException("Disconnected from {0} while trying to send data: {1}.", RemoteEndPoint, e.Message);
				}
			}
		}

		/// <summary>
		///   Sends the size of the next packet.
		/// </summary>
		/// <param name="context">The context in which the asynchronous method should be executed.</param>
		/// <param name="size">The size that should be sent.</param>
		private async Task SendPackSizeAsync(ProcessContext context, int size)
		{
			using (var packet = _packetFactory.CreateOutgoingPacket())
			{
				packet.Writer.WriteUInt16((ushort)size);
				await _socket.SendAsync(context, new ArraySegment<byte>(packet.Data, 0, SizeByteCount));
			}
		}

		/// <summary>
		///   Receives a packet sent over the connection.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		public async Task<IncomingPacket> ReceiveAsync(ProcessContext context)
		{
			Assert.That(IsConnected, "Cannot receive: Not connected.");

			IncomingPacket packet = null;
			var packetReturned = false;

			try
			{
				// Read the size of the next packet
				using (var sizePacket = _packetFactory.CreateIncomingPacket())
				{
					sizePacket.SetDataRange(SizeByteCount);
					await ReceiveAsync(context, sizePacket);

					packet = _packetFactory.CreateIncomingPacket();
					packet.SetDataRange(sizePacket.Reader.ReadUInt16());
				}

				if (packet.Size + SizeByteCount > Packet.MaxSize)
				{
					_state = State.Faulted;
					throw new SocketOperationException(
						"Connection to {0} terminated. Received packet of size {1} while {2} is the maximum size allowed.",
						RemoteEndPoint, packet.Size + SizeByteCount, Packet.MaxSize);
				}

				await ReceiveAsync(context, packet);
				packetReturned = true;
				return packet;
			}
			finally
			{
				// If we did not return the packet to due some exception (socket error, process cancellation), we
				// have to dispose it
				if (packet != null && !packetReturned)
					packet.Dispose();
			}
		}

		/// <summary>
		///   Tries to receive exactly the given number of bytes and copies them into the buffer.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="packet">The packet the incoming data should be written to.</param>
		private async Task ReceiveAsync(ProcessContext context, IncomingPacket packet)
		{
			try
			{
				var bytesReceived = 0;
				while (bytesReceived < packet.Size)
				{
					var missingBytes = packet.Size - bytesReceived;
					var bufferSegment = new ArraySegment<byte>(packet.Data, bytesReceived, missingBytes);
					bytesReceived = await _socket.ReceiveAsync(context, bufferSegment);

					if (bytesReceived == 0)
					{
						_state = State.Disconnected;
						throw new SocketOperationException("Graceful disconnect from {0}.", RemoteEndPoint);
					}
				}
			}
			catch (SocketException e)
			{
				_state = State.Faulted;
				throw new SocketOperationException("Disconnected from {0} while trying to receive data.", RemoteEndPoint, e);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			try
			{
				if (_state == State.Connected)
					_socket.Shutdown(SocketShutdown.Both);
			}
			catch (SocketException e)
			{
				NetworkLog.DebugInfo("When calling shutdown on socket connected to {0}: {1}.", RemoteEndPoint, e.Message);
			}

			_socket.Close();
			_socket.SafeDispose();
		}

		/// <summary>
		///   Describes the state of a connection.
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