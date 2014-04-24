namespace Pegasus.Platform.Network
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Logging;
	using Memory;

	/// <summary>
	///     Represents an IPv4 or IPv6 internet protocol address.
	/// </summary>
	public class IPAddress : PooledObject<IPAddress>
	{
		/// <summary>
		///     The underlying native IP address.
		/// </summary>
		private IntPtr _ipAddress;

		/// <summary>
		///     Gets a value indicating whether the IP address is in IPv4 or IPv6 format.
		/// </summary>
		public AddressFamily Family
		{
			get
			{
				Assert.NotPooled(this);
				return NativeMethods.GetAddressFamily(_ipAddress);
			}
		}

		/// <summary>
		///     Gets the underlying native IP address.
		/// </summary>
		internal IntPtr NativePtr
		{
			get { return _ipAddress; }
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="ipAddress">The textual representation of the IP address.</param>
		public static IPAddress Create(string ipAddress)
		{
			Assert.ArgumentNotNull(ipAddress);

			var address = GetInstance();
			address._ipAddress = NativeMethods.CreateIPAddress(ipAddress);

			if (address._ipAddress == IntPtr.Zero)
				Log.Die("'{0}' is not a valid IP address.", ipAddress);

			return address;
		}

		/// <summary>
		///     Tries to initialize a new instance.
		/// </summary>
		/// <param name="ipAddress">The textual representation of the IP address.</param>
		/// <param name="address">The IP address instance that is returned on success</param>
		public static bool TryCreate(string ipAddress, out IPAddress address)
		{
			Assert.ArgumentNotNull(ipAddress);

			var parsedAddress = NativeMethods.CreateIPAddress(ipAddress);
			if (parsedAddress == IntPtr.Zero)
			{
				address = null;
				return false;
			}

			address = GetInstance();
			address._ipAddress = parsedAddress;
			return true;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="ipAddress">The native IP address.</param>
		internal static IPAddress Create(IntPtr ipAddress)
		{
			Assert.ArgumentNotNull(ipAddress);

			var address = GetInstance();
			address._ipAddress = ipAddress;

			return address;
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			NativeMethods.DestroyIPAddress(_ipAddress);
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			Assert.NotPooled(this);
			
			var address = NativeMethods.ToString(_ipAddress);
			if (address == IntPtr.Zero)
				return "<unknown>";

			return Marshal.PtrToStringAnsi(address);
		}

		/// <summary>
		///     Determines whether the given IP address is equivalent to the current instance. IP addresses are considered
		///     equivalent if they are equal or if one of them is an IPv4 address and the other one represents the same IPv4 address
		///     embedded into an IPv6 address.
		/// </summary>
		public bool IsEquivalentTo(IPAddress other)
		{
			Assert.ArgumentNotNull(other);
			return NativeMethods.Equals(_ipAddress, other._ipAddress);
		}

		/// <summary>
		///     Provides access to the native IP address functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateIPAddress")]
			public static extern IntPtr CreateIPAddress(string ipAddress);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyIPAddress")]
			public static extern void DestroyIPAddress(IntPtr ipAddress);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgIPAddressToString")]
			public static extern IntPtr ToString(IntPtr ipAddress);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgIpAddressesAreEqual")]
			public static extern bool Equals(IntPtr ipAddress1, IntPtr ipAddress2);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetAddressFamily")]
			public static extern AddressFamily GetAddressFamily(IntPtr ipAddress);
		}
	}
}