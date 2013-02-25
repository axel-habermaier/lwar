using System;

namespace Pegasus.Framework.Network
{
	using System.Net;
	using System.Net.Sockets;

	/// <summary>
	///   Provides extension methods for the IPEndPoint class.
	/// </summary>
	public static class IPEndPointExtensions
	{
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