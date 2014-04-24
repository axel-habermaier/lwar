namespace Pegasus.Platform.Network
{
	using System;
	using Memory;

	/// <summary>
	///     Represents an endpoint consisting of an IP address and a port number.
	/// </summary>
	public class IPEndPoint : PooledObject<IPEndPoint>
	{
		/// <summary>
		///     The IP address of the endpoint.
		/// </summary>
		private IPAddress _address;

		/// <summary>
		///     Gets the port of the endpoint.
		/// </summary>
		public ushort Port { get; set; }

		/// <summary>
		///     Gets the IP address of the endpoint.
		/// </summary>
		public IPAddress Address
		{
			get { return _address; }
			set
			{
				_address.SafeDispose();
				_address = value;
			}
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			Address.SafeDispose();
			Address = null;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="ipAddress">The IP address of the endpoint.</param>
		/// <param name="port">The port of the endpoint.</param>
		public static IPEndPoint Create(IPAddress ipAddress, ushort port)
		{
			Assert.ArgumentNotNull(ipAddress);

			var endpoint = GetInstance();
			endpoint.Address = ipAddress;
			endpoint.Port = port;

			return endpoint;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public static IPEndPoint Create()
		{
			var endpoint = GetInstance();
			endpoint.Address = IPAddress.Create("127.0.0.1");

			return endpoint;
		}

		/// <summary>
		///     Determines whether the given IP endpoint is equivalent to the current instance. IP endpoints are considered equivalent
		///     if their ports are equal and their IP address are equivalent.
		/// </summary>
		public bool IsEquivalentTo(IPEndPoint other)
		{
			Assert.ArgumentNotNull(other);
			return Port == other.Port && Address.IsEquivalentTo(other.Address);
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			if (Address.Family == AddressFamily.IPv4)
				return String.Format("{0}:{1}", Address, Port);
				
			return String.Format("[{0}]:{1}", Address, Port);
		}
	}
}