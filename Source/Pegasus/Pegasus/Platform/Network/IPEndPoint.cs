namespace Pegasus.Platform.Network
{
	using System;

	/// <summary>
	///     Represents an endpoint consisting of an IP address and a port number.
	/// </summary>
	public struct IPEndPoint
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="address">The IP address of the endpoint.</param>
		/// <param name="port">The port of the endpoint.</param>
		public IPEndPoint(IPAddress address, ushort port)
			: this()
		{
			Address = address;
			Port = port;
		}

		/// <summary>
		///     Gets the IP address of the endpoint.
		/// </summary>
		public IPAddress Address { get; private set; }

		/// <summary>
		///     Gets the port of the endpoint.
		/// </summary>
		public ushort Port { get; private set; }

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			if (Address.IsMappedIPv4)
				return String.Format("{0}:{1}", Address, Port);

			return String.Format("[{0}]:{1}", Address, Port);
		}

		/// <summary>
		///     Indicates whether the the given IP endpoint is equal to the current one.
		/// </summary>
		public bool Equals(IPEndPoint other)
		{
			return Address.Equals(other.Address) && Port == other.Port;
		}

		/// <summary>
		///     Indicates whether the the given object is equal to the current one.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is IPEndPoint && Equals((IPEndPoint)obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				return (Address.GetHashCode() * 397) ^ Port.GetHashCode();
			}
		}

		/// <summary>
		///     Indicates whether the two given IP endpoints are equal.
		/// </summary>
		public static bool operator ==(IPEndPoint left, IPEndPoint right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Indicates whether the two given IP endpoints are not equal.
		/// </summary>
		public static bool operator !=(IPEndPoint left, IPEndPoint right)
		{
			return !left.Equals(right);
		}
	}
}