using System;

namespace Pegasus.Framework.Platform.Network
{
	using System.Net;
	using System.Net.Sockets;

	/// <summary>
	///   Provides extension methods for network-related types.
	/// </summary>
	public static class Extensions
	{
#if Linux
		[System.Runtime.InteropServices.DllImport("libc", EntryPoint = "setsockopt")]
		private static extern int SetSocketOption(IntPtr socket, int level, int optname, ref int optval, int optlen);
#endif

		/// <summary>
		///   Enables the socket to use both IPv6 and IPv6-mapped IPv4 addresses.
		/// </summary>
		/// <param name="socket">The socket for which dual mode should be enabled.</param>
		internal static void EnableDualMode(this Socket socket)
		{
			Assert.ArgumentNotNull(socket);
			Assert.ArgumentSatisfies(socket.AddressFamily == AddressFamily.InterNetworkV6, "Not an IPv6 socket.");

#if Windows
			socket.DualMode = true;
#elif Linux
			int value = 0;
			const int IPPROTO_IPV6 = 41;
			const int IPV6_V6ONLY = 27;
			var success = SetSocketOption(socket.Handle, IPPROTO_IPV6, IPV6_V6ONLY, ref value, sizeof(int)) == 0;

			if (success != 0)
				Platform.Logging.Log.Warn("UDP socket is IPv6-only; dual-stack mode could not be activated.");
#endif
		}

		/// <summary>
		///   Checks whether the two end points actually represent the same IP address and port. This method
		///   takes IPv4 addresses into account that have been embedded into an IPv6 one.
		/// </summary>
		/// <param name="endPoint1">The first end point that should be checked.</param>
		/// <param name="endPoint2">The second end point that should be checked.</param>
		public static bool SameEndPoint(this IPEndPoint endPoint1, IPEndPoint endPoint2)
		{
			if (endPoint1 == null && endPoint2 == null)
				return true;

			if (endPoint1 == null || endPoint2 == null)
				return false;

			if (endPoint1.Port != endPoint2.Port)
				return false;

			if (endPoint1.AddressFamily == endPoint2.AddressFamily)
				return endPoint1.Address.Equals(endPoint2.Address);

			byte[] ipv4, ipv6;
			if (endPoint1.AddressFamily == AddressFamily.InterNetwork && endPoint2.AddressFamily == AddressFamily.InterNetworkV6)
			{
				ipv4 = endPoint1.Address.GetAddressBytes();
				ipv6 = endPoint2.Address.GetAddressBytes();
			}
			else if (endPoint1.AddressFamily == AddressFamily.InterNetworkV6 && endPoint2.AddressFamily == AddressFamily.InterNetwork)
			{
				ipv4 = endPoint2.Address.GetAddressBytes();
				ipv6 = endPoint1.Address.GetAddressBytes();
			}
			else
				return false;

			for (var i = 0; i < 10; i++)
			{
				if (ipv6[i] != 0)
					return false;
			}

			for (var i = 0; i < 4; i++)
			{
				if (ipv6[i + 12] != ipv4[i])
					return false;
			}

			return true;
		}
	}
}