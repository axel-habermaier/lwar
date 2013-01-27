﻿using System;

namespace Pegasus.Framework.Network
{
	using System.Net;
	using System.Net.Sockets;
	using System.Threading.Tasks;
	using Processes;
	using Scripting;

	/// <summary>
	///   Represents a Udp-based socket connection that can be used to unreliably send and receive packets over the network.
	/// </summary>
	public class UdpSocket : DisposableObject
	{
		/// <summary>
		///   The underlying socket that is used to send and receive packets.
		/// </summary>
		private readonly Socket _socket;

		/// <summary>
		///   Initializes a new instance. On Windows, dual mode is enabled to support both IPv6 and IPv4. On Linux, only IPv4 is
		///   supported.
		/// </summary>
		public UdpSocket()
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
			Assert.InRange(packet.Size, 1, Packet.MaxSize);

			using (packet)
			{
				try
				{
					await _socket.SendToAsync(context, new ArraySegment<byte>(packet.Data), remoteEndPoint);
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
			var packetReturned = false;

			try
			{
				var size = await _socket.ReceiveFromAsync(context, new ArraySegment<byte>(packet.Data), remoteEndPoint);

				packet.Initialize(size);
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