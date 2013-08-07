using System;

namespace Pegasus.Framework.Network
{
	using System.Net.Sockets;
	using Platform.Logging;

	/// <summary>
	///   Provides extension methods for awaitable socket operations.
	/// </summary>
	internal static class SocketExtensions
	{
#if Linux
		[System.Runtime.InteropServices.DllImport("libc", EntryPoint = "setsockopt")]
		private static extern int SetSocketOption(IntPtr socket, int level, int optname, ref int optval, int optlen);
#endif

		/// <summary>
		///   Enables the socket to use both IPv6 and IPv6-mapped IPv4 addresses.
		/// </summary>
		/// <param name="socket">The socket for which dual mode should be enabled.</param>
		public static void EnableDualMode(this Socket socket)
		{
			Assert.ArgumentNotNull(socket);
			Assert.ArgumentSatisfies(socket.AddressFamily == AddressFamily.InterNetworkV6, "Not an IPv6 socket.");

#if Windows
			socket.DualMode = true;
			var success = socket.DualMode;
#elif Linux
			int value = 0;
			const int IPPROTO_IPV6 = 41;
			const int IPV6_V6ONLY = 27;
			var success = SetSocketOption(socket.Handle, IPPROTO_IPV6, IPV6_V6ONLY, ref value, sizeof(int)) == 0;
#endif

			if (!success)
				Log.Warn("UDP socket is IPv6-only; dual-stack mode could not be activated.");
		}
	}
}