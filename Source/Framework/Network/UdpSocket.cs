using System;

namespace Pegasus.Framework.Network
{
	using System.Net;
	using System.Net.Sockets;
	using System.Threading.Tasks;
	using Processes;

	/// <summary>
	///   Represents a Udp-based socket connection that can be used to unreliably send and receive packets over the network.
	/// </summary>
	internal class UdpSocket : DisposableObject
	{
		/// <summary>
		///   The underlying socket that is used to send and receive packets.
		/// </summary>
		private readonly Socket _socket;

		/// <summary>
		///   Initializes a new instance. On Windows, dual mode is enabled to support both IPv6 and IPv4. On Linux, only IPv4 is
		///   supported.
		/// </summary>
		internal UdpSocket()
		{
#if Windows
			_socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp) { DualMode = true };
#elif Linux
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
#endif
		}

		/// <summary>
		///   Sends the given packet sent over the connection.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="packet">The packet that should be sent. The packet is returned to the pool before this function returns.</param>
		/// <param name="remoteEndPoint">The endpoint of the peer the packet should be sent to.</param>
		public async Task SendAsync(ProcessContext context, OutgoingPacket packet, IPEndPoint remoteEndPoint)
		{
			Assert.ArgumentNotNull(packet, () => packet);
			Assert.ArgumentNotNull(remoteEndPoint, () => remoteEndPoint);

			using (packet)
			{
				try
				{
					var packetData = packet.DataBuffer;
					await _socket.SendToAsync(context, packetData, remoteEndPoint);
				}
				catch (SocketException e)
				{
					throw new SocketOperationException("Unable to send Udp packet to {0}: {1}.", remoteEndPoint, e.Message);
				}
			}
		}

		/// <summary>
		///   Receives a packet sent over the connection.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="remoteEndPoint">After the method completes, contains the endpoint of the peer that sent the packet.</param>
		public async Task<IncomingPacket> ReceiveAsync(ProcessContext context, IPEndPoint remoteEndPoint)
		{
			var packet = IncomingPacket.Create();

			var size = await _socket.ReceiveFromAsync(context, packet.DataBuffer, remoteEndPoint);
			if (size == 0 || size > Packet.MaxSize)
			{
				packet.Dispose();
				throw new SocketOperationException("Received a Udp packet of invalid size ({0} bytes).", size);
			}

			return packet;
		}

		/// <summary>
		///   Binds the socket to the given local endpoint.
		/// </summary>
		/// <param name="localEndPoint">The local endpoint the socket should be bound to.</param>
		public void Bind(IPEndPoint localEndPoint)
		{
			Assert.ArgumentNotNull(localEndPoint, () => localEndPoint);

			try
			{
				_socket.Bind(localEndPoint);
			}
			catch (SocketException e)
			{
				throw new SocketOperationException("Failed to bind Udp socket to {0}: {1}.", localEndPoint, e);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_socket.Close();
			_socket.SafeDispose();
		}
	}
}