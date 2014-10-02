namespace Pegasus.Platform.Network
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Logging;

	/// <summary>
	///     Represents an IPv4 or IPv6 internet protocol address.
	/// </summary>
	public unsafe struct IPAddress : IEquatable<IPAddress>
	{
		/// <summary>
		///     Represents the IP address of the local host.
		/// </summary>
		public static readonly IPAddress LocalHost = Parse("::1");

		/// <summary>
		///     Represents the '::' IPv6 address.
		/// </summary>
		public static readonly IPAddress Any = new IPAddress();

		/// <summary>
		///     The underlying native IP address.
		/// </summary>
		[UsedImplicitly]
		private fixed byte _bytes [16];

		/// <summary>
		///     Gets a value indicating whether the IP address is an IPv6-mapped IPv4 address.
		/// </summary>
		public bool IsMappedIPv4
		{
			get
			{
				fixed (IPAddress* ip = &this)
				{
					for (var i = 0; i < 8; ++i)
					{
						if (ip->_bytes[i] != 0)
							return false;
					}

					return ip->_bytes[10] == 255 && ip->_bytes[11] == 255;
				}
			}
		}

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

			address = new IPAddress();
			fixed (IPAddress* addr = &address)
				return NativeMethods.TryParseIPAddress(ipAddress, addr);
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			var that = this;
			var address = NativeMethods.ToString(&that);

			if (address == IntPtr.Zero)
				return "<unknown>";

			const string ipv6Prefix = "::ffff:";

			var str = Marshal.PtrToStringAnsi(address);
			if (IsMappedIPv4 && str.StartsWith(ipv6Prefix))
				return str.Substring(ipv6Prefix.Length);

			return str;
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

		/// <summary>
		///     Provides access to the native IP address functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgTryParseIPAddress")]
			public static extern bool TryParseIPAddress(string address, IPAddress* ipAddress);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgIPAddressToString")]
			public static extern IntPtr ToString(IPAddress* ipAddress);
		}
	}
}