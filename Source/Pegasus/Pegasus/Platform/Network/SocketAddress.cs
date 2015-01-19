namespace Pegasus.Platform.Network
{
	using System;
	using System.Net.Sockets;
	using System.Runtime.InteropServices;
	using Memory;

	/// <summary>
	///     Represents a socket address that is used for low-level socket operations.
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Size = 28)]
	internal unsafe struct SocketAddress
	{
		/// <summary>
		///     The address family of the socket address.
		/// </summary>
		[FieldOffset(0)]
		private ushort _addressFamily;

		/// <summary>
		///     The port of the socket address.
		/// </summary>
		[FieldOffset(2)]
		private ushort _port;

		/// <summary>
		///     The IPv4 address of the socket address.
		/// </summary>
		[FieldOffset(4)]
		public fixed byte IPv4 [4];

		/// <summary>
		///     The IPv6 address of the socket address.
		/// </summary>
		[FieldOffset(8)]
		public fixed byte IPv6 [16];

		/// <summary>
		///     Gets or sets the address family of the socket address.
		/// </summary>
		/// <remarks>On Linux, AddressFamily.InterNetworkV6 has a native value of 10. Annoying.</remarks>
		public AddressFamily AddressFamily
		{
			get
			{
				if (PlatformInfo.Platform == PlatformType.Windows)
					return (AddressFamily)_addressFamily;

				return _addressFamily == 10 ? AddressFamily.InterNetworkV6 : (AddressFamily)_addressFamily;
			}
			set
			{
				if (PlatformInfo.Platform != PlatformType.Windows && value == AddressFamily.InterNetworkV6)
					_addressFamily = 10;
				else
					_addressFamily = (ushort)value;
			}
		}

		/// <summary>
		///     Gets the port of the socket address.
		/// </summary>
		public ushort Port
		{
			get { return BitConverter.IsLittleEndian ? EndianConverter.Convert(_port) : _port; }
			set { _port = BitConverter.IsLittleEndian ? EndianConverter.Convert(value) : value; }
		}
	}
}