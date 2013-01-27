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
		internal TcpSocket()
		{
			_state = State.Disconnected;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="socket">The underlying socket that should be used to send and receive packets.</param>
		internal TcpSocket(Socket socket)
		{
			Assert.ArgumentNotNull(socket, () => socket);
#if Windows
			Assert.ArgumentSatisfies(socket.DualMode, () => socket, "Socket is not in dual mode.");
#endif

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
		///   Creates a new socket instance that uses Tcp as the transport protocol. On Windows, dual mode is enabled to support
		///   both IPv6 and IPv4. On Linux, only IPv4 is supported.
		/// </summary>
		internal static Socket Create()
		{
#if Windows
			return new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp) { DualMode = true };
#elif Linux
			return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#endif
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

			_socket = _socket.Reinitialize();
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
					// Send the size of the next packet
					using (var sizePacket = OutgoingPacket.Create())
					{
						sizePacket.Writer.WriteUInt16((ushort)packet.Size);
						await _socket.SendAsync(context, new ArraySegment<byte>(sizePacket.Data, 0, SizeByteCount));
					}

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
				using (var sizePacket = IncomingPacket.Create(SizeByteCount))
				{
					await ReceiveAsync(context, sizePacket);
					packet = IncomingPacket.Create(sizePacket.Reader.ReadUInt16());
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