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
	public class UdpSocket : DisposableObject
	{
		/// <summary>
		///   Used to create incoming and outgoing packets.
		/// </summary>
		private readonly IPacketFactory _packetFactory;

		/// <summary>
		///   The underlying socket that is used to send and receive packets.
		/// </summary>
		private readonly Socket _socket;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public UdpSocket(IPacketFactory packetFactory)
		{
			Assert.ArgumentNotNull(packetFactory, () => packetFactory);
			_packetFactory = packetFactory;

			_socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp) { Blocking = false };
			_socket.EnableDualMode();
		}

		/// <summary>
		///   Sends the given packet over the connection.
		/// </summary>
		/// <param name="packet">The packet that should be sent. The packet is returned to the pool before this function returns.</param>
		/// <param name="remoteEndPoint">The endpoint of the peer the packet should be sent to.</param>
		public void Send(OutgoingPacket packet, IPEndPoint remoteEndPoint)
		{
			Assert.ArgumentNotNull(packet, () => packet);
			Assert.ArgumentNotNull(remoteEndPoint, () => remoteEndPoint);
			Assert.InRange(packet.Size, 1, Packet.MaxSize);

			using (packet)
			{
				try
				{
					_socket.SendTo(packet.Data, 0, packet.Size, SocketFlags.None, remoteEndPoint);
				}
				catch (SocketException e)
				{
					throw new SocketOperationException("Unable to send Udp packet to {0}: {1}.", remoteEndPoint, e.Message);
				}
			}
		}

		/// <summary>
		///   Sends the given packet over the connection.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="packet">The packet that should be sent. The packet is returned to the pool before this function returns.</param>
		/// <param name="remoteEndPoint">The endpoint of the peer the packet should be sent to.</param>
		public async Task SendAsync(ProcessContext context, OutgoingPacket packet, IPEndPoint remoteEndPoint)
		{
			Assert.ArgumentNotNull(packet, () => packet);
			Assert.ArgumentNotNull(remoteEndPoint, () => remoteEndPoint);
			Assert.InRange(packet.Size, 1, Packet.MaxSize);

			using (packet)
			{
				try
				{
					await _socket.SendToAsync(context, new ArraySegment<byte>(packet.Data, 0, packet.Size), remoteEndPoint);
				}
				catch (SocketException e)
				{
					throw new SocketOperationException("Unable to send Udp packet to {0}: {1}.", remoteEndPoint, e.Message);
				}
			}
		}

		/// <summary>
		///   Tries to receive a packet sent over the connection. Returns true if a packet has been received, false otherwise.
		/// </summary>
		/// <param name="remoteEndPoint">After the method completes, contains the endpoint of the peer that sent the packet.</param>
		/// <param name="packet">The packet that has been received.</param>
		public bool TryReceive(ref IPEndPoint remoteEndPoint, out IncomingPacket packet)
		{
			packet = null;

			if (_socket.Available == 0)
				return false;

			packet = _packetFactory.CreateIncomingPacket();
			var packetReturned = false;

			try
			{
				var endPoint = (EndPoint)remoteEndPoint;
				var size = _socket.ReceiveFrom(packet.Data, ref endPoint);
				remoteEndPoint = (IPEndPoint)endPoint;

				packet.SetDataRange(size);
				packetReturned = true;
				return true;
			}
			catch (SocketException e)
			{
				throw new SocketOperationException("Error while trying to receive Udp packet: {0}.", e.Message);
			}
			finally
			{
				if (!packetReturned)
					packet.SafeDispose();
			}
		}

		/// <summary>
		///   Receives a packet sent over the connection.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="remoteEndPoint">After the method completes, contains the endpoint of the peer that sent the packet.</param>
		public async Task<IncomingPacket> ReceiveAsync(ProcessContext context, IPEndPoint remoteEndPoint)
		{
			var packet = _packetFactory.CreateIncomingPacket();
			var packetReturned = false;

			try
			{
				var size = await _socket.ReceiveFromAsync(context, new ArraySegment<byte>(packet.Data), remoteEndPoint);

				packet.SetDataRange(size);
				packetReturned = true;
				return packet;
			}
			catch (SocketException e)
			{
				throw new SocketOperationException("Error while trying to receive Udp packet: {0}.", e.Message);
			}
			finally
			{
				if (!packetReturned)
					packet.SafeDispose();
			}
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