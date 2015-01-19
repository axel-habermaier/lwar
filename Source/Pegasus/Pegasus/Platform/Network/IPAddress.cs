namespace Pegasus.Platform.Network
{
	using System;
	using System.Net.Sockets;
	using System.Runtime.InteropServices;
	using Logging;
	using Utilities;

	/// <summary>
	///     Represents an IPv4 or IPv6 internet protocol address.
	/// </summary>
	/// <remarks>
	///     We're not using the System.Net.IPAddress in order to reduce the pressure on the garbage collector as each invocation of
	///     System.Net.Sockets.Socket.ReceiveFrom and System.Net.Sockets.Socket.SendTo allocates memory.
	/// </remarks>
	public unsafe struct IPAddress : IEquatable<IPAddress>
	{
		/// <summary>
		///     Represents the IP address of the local host.
		/// </summary>
		public static readonly IPAddress LocalHost;

		/// <summary>
		///     The underlying native IP address.
		/// </summary>
		private fixed byte _bytes [16];

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		static IPAddress()
		{
			LocalHost = new IPAddress(System.Net.IPAddress.IPv6Loopback);
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="address">The .NET IP address that should be used to initialize this instance.</param>
		private IPAddress(System.Net.IPAddress address)
			: this()
		{
			var bytes = address.GetAddressBytes();
			fixed (IPAddress* ip = &this)
			{
				switch (address.AddressFamily)
				{
					case AddressFamily.InterNetwork:
						Assert.That(bytes.Length == 4, "Unexpected size.");
						ip->IsIPv4 = true;
						ip->_bytes[10] = 255;
						ip->_bytes[11] = 255;
						Marshal.Copy(bytes, 0, new IntPtr(ip->_bytes) + 12, 4);
						break;
					case AddressFamily.InterNetworkV6:
						Assert.That(bytes.Length == 16, "Unexpected size.");
						Marshal.Copy(bytes, 0, new IntPtr(ip->_bytes), 16);
						break;
					default:
						throw new InvalidOperationException("Unsupported address family.");
				}
			}
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="address">The socket address the IP address should be initialized from.</param>
		internal IPAddress(ref SocketAddress address)
			: this()
		{
			fixed (IPAddress* ip = &this)
			fixed (SocketAddress* addr = &address)
			{
				switch (address.AddressFamily)
				{
					case AddressFamily.InterNetwork:
						ip->IsIPv4 = true;
						ip->_bytes[10] = 255;
						ip->_bytes[11] = 255;
						for (var i = 0; i < 4; ++i)
							ip->_bytes[i + 12] = addr->IPv4[i];
						break;
					case AddressFamily.InterNetworkV6:
						for (var i = 0; i < 16; ++i)
							ip->_bytes[i] = addr->IPv6[i];
						break;
					default:
						throw new InvalidOperationException(String.Format("Unsupported address family '{0}'.", address.AddressFamily));
				}
			}
		}

		/// <summary>
		///     Gets a value indicating whether the IP address is an IPv4 address.
		/// </summary>
		public bool IsIPv4 { get; private set; }

		/// <summary>
		///     Indicates whether the the given IP address is equal to the current one.
		/// </summary>
		public bool Equals(IPAddress other)
		{
			fixed (IPAddress* ip = &this)
			{
				for (var i = 0; i < 16; ++i)
				{
					if (ip->_bytes[i] != other._bytes[i])
						return false;
				}
			}

			return true;
		}

		/// <summary>
		///     Writes the IP address into the given socket address.
		/// </summary>
		/// <param name="address">The location the IP address should be written to.</param>
		internal void CopyTo(byte* address)
		{
			fixed (IPAddress* ip = &this)
			{
				for (var i = 0; i < 16; ++i)
					address[i] = ip->_bytes[i];
			}
		}

		/// <summary>
		///     Initializes a new instance from a string.
		/// </summary>
		/// <param name="ipAddress">The textual representation of the IP address.</param>
		public static IPAddress Parse(string ipAddress)
		{
			IPAddress address;
			if (!TryParse(ipAddress, out address))
				Log.Die("'{0}' is not a valid IP address.", ipAddress);

			return address;
		}

		/// <summary>
		///     Tries to initialize a new instance from a string.
		/// </summary>
		/// <param name="ipAddress">The textual representation of the IP address.</param>
		/// <param name="address">The IP address instance that is returned on success</param>
		public static bool TryParse(string ipAddress, out IPAddress address)
		{
			Assert.ArgumentNotNull(ipAddress);

			System.Net.IPAddress addr;
			if (System.Net.IPAddress.TryParse(ipAddress, out addr))
			{
				address = new IPAddress(addr);
				return true;
			}

			address = new IPAddress();
			return false;
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return ToSystemAddress().ToString();
		}

		/// <summary>
		///     Converts the IP address to a System.Net.IPAddress instance.
		/// </summary>
		internal System.Net.IPAddress ToSystemAddress()
		{
			var byteCount = IsIPv4 ? 4 : 16;
			var offset = IsIPv4 ? 12 : 0;

			var bytes = new byte[byteCount];
			fixed (IPAddress* ip = &this)
				Marshal.Copy(new IntPtr(ip->_bytes) + offset, bytes, 0, byteCount);

			return new System.Net.IPAddress(bytes);
		}

		/// <summary>
		///     Indicates whether the the given object is equal to the current one.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is IPAddress && Equals((IPAddress)obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			fixed (IPAddress* ip = &this)
				return ip->_bytes[15] * 397 ^ ip->_bytes[14];
		}

		/// <summary>
		///     Indicates whether the two given IP addresses are equal.
		/// </summary>
		public static bool operator ==(IPAddress left, IPAddress right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Indicates whether the two given IP addresses are not equal.
		/// </summary>
		public static bool operator !=(IPAddress left, IPAddress right)
		{
			return !left.Equals(right);
		}
	}
}