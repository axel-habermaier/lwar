namespace Pegasus.Platform.Network
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Logging;

	/// <summary>
	///     Represents an IPv4 or IPv6 internet protocol address.
	/// </summary>
	public struct IPAddress
	{
		/// <summary>
		///     The underlying native IP address.
		/// </summary>
		private byte[] _bytes;

		/// <summary>
		///     Gets the bytes of the IP address.
		/// </summary>
		internal byte[] AddressBytes
		{
			get { return _bytes; }
		}

		/// <summary>
		///     Initializes an empty IP address.
		/// </summary>
		public static IPAddress CreateEmpty()
		{
			return new IPAddress { _bytes = new byte[16] };
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
		public static unsafe bool TryParse(string ipAddress, out IPAddress address)
		{
			Assert.ArgumentNotNull(ipAddress);

			var bytes = new byte[16];
			fixed (byte* data = bytes)
			{
				if (!NativeMethods.TryParseIPAddress(ipAddress, data))
				{
					address = new IPAddress();
					return false;
				}

				address = new IPAddress { _bytes = bytes };
				return true;
			}
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override unsafe string ToString()
		{
			fixed (byte* bytes = _bytes)
			{
				var address = NativeMethods.ToString(bytes);
				if (address == IntPtr.Zero)
					return "<unknown>";

				return Marshal.PtrToStringAnsi(address);
			}
		}

		/// <summary>
		///     Indicates whether the the given IP address is equal to the current one.
		/// </summary>
		public bool Equals(IPAddress other)
		{
			if (_bytes == null && other._bytes == null)
				return true;

			if (_bytes == null || other._bytes == null)
				return false;

			for (var i = 0; i < 16; ++i)
			{
				if (_bytes[i] != other._bytes[i])
					return false;
			}

			return true;
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
			return (_bytes != null ? _bytes.GetHashCode() : 0);
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
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgTryParseIPAddress")]
			public static extern unsafe bool TryParseIPAddress(string address, byte* ipAddress);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgIPAddressToString")]
			public static extern unsafe IntPtr ToString(byte* ipAddress);
		}
	}
}