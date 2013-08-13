using System;

namespace Pegasus.Framework.Platform.Network
{
	using System.Net;
	using System.Net.Sockets;
	using Memory;

	/// <summary>
	///   Represents a Udp-based socket that can be used to send and receive multicast packets over the network.
	/// </summary>
	public class MulticastSocket : DisposableObject
	{
		/// <summary>
		///   The multicast group that the socket listens to or sends to.
		/// </summary>
		private readonly IPEndPoint _multicastGroup;

		/// <summary>
		///   The UDP socket that is used for sending and receiving the data.
		/// </summary>
		private readonly UdpSocket _udpSocket = new UdpSocket();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="multicastGroup">The multicast group that the socket should listen to or send to.</param>
		public MulticastSocket(IPEndPoint multicastGroup)
		{
			Assert.ArgumentNotNull(multicastGroup);
			Assert.ArgumentSatisfies(multicastGroup.AddressFamily == AddressFamily.InterNetworkV6, "Not a valid IPv6 multicast address.");
			Assert.ArgumentSatisfies(multicastGroup.Address.IsIPv6Multicast, "Not a valid multicast group.");

			_multicastGroup = multicastGroup;
		}

		/// <summary>
		///   Connects the socket to the multicast group and prepares it for sending.
		/// </summary>
		/// <param name="timeToLive">The time to live for the multicast messages.</param>
		public void Connect(int timeToLive)
		{
			Assert.ArgumentInRange(timeToLive, 1, Int32.MaxValue);

			_udpSocket.Socket.MulticastLoopback = true;
			_udpSocket.Socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastTimeToLive, timeToLive);
			_udpSocket.Socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership,
											  new IPv6MulticastOption(_multicastGroup.Address));
		}

		/// <summary>
		///   Binds the socket to the multicast group and prepares it for receiving.
		/// </summary>
		public void Bind()
		{
			_udpSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, _multicastGroup.Port));
			_udpSocket.Socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership,
											  new IPv6MulticastOption(_multicastGroup.Address));
		}

		/// <summary>
		///   Sends the given packet over the connection.
		/// </summary>
		/// <param name="buffer">The buffer that contains the data that should be sent.</param>
		/// <param name="size">The number of bytes that should be sent.</param>
		public void Send(byte[] buffer, int size)
		{
			_udpSocket.Send(buffer, size, _multicastGroup);
		}

		/// <summary>
		///   Tries to receive a packet sent over the connection. Returns true if a packet has been received, false otherwise.
		/// </summary>
		/// <param name="buffer">The buffer the received data should be written to.</param>
		/// <param name="remoteEndPoint">After the method completes, contains the endpoint of the peer that sent the packet.</param>
		/// <param name="size">Returns the number of bytes that have been received.</param>
		public bool TryReceive(byte[] buffer, ref IPEndPoint remoteEndPoint, out int size)
		{
			return _udpSocket.TryReceive(buffer, ref remoteEndPoint, out size);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_udpSocket.SafeDispose();
		}
	}
}