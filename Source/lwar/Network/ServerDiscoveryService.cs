using System;

namespace Lwar.Network
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Platform.Network;

	/// <summary>
	///   Listens to incoming automatic server discovery messages and reports the end points of the servers that it has
	///   discovered.
	/// </summary>
	public class ServerDiscoveryService : DisposableObject
	{
		/// <summary>
		///   The amount of time in milliseconds after which a server is assumed to be shut down if no more discovery messages are
		///   received from the server.
		/// </summary>
		private const double ServerTimeout = 60000.0 / Specification.DiscoveryMessageFrequency * 5;

		/// <summary>
		///   The buffer that is used to receive the multi cast data.
		/// </summary>
		private readonly byte[] _buffer = new byte[Specification.MaxPacketSize];

		/// <summary>
		///   The list of known servers that have been discovered.
		/// </summary>
		private readonly List<ServerInfo> _knownServers = new List<ServerInfo>();

		/// <summary>
		///   The socket that is used to receive discovery messages.
		/// </summary>
		private readonly MulticastSocket _multicastSocket = new MulticastSocket(Specification.MulticastGroup);

		/// <summary>
		///   The end point of the server that sent the discovery message.
		/// </summary>
		private IPEndPoint _serverEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0);

		/// <summary>
		///   Raised when a new server has been discovered.
		/// </summary>
		public event Action<IPEndPoint> ServerDiscovered;

		/// <summary>
		///   Raised when a server has presumably been shut down.
		/// </summary>
		public event Action<IPEndPoint> ServerHasShutdown;

		/// <summary>
		///   Checks if any new discovery messages have been received and raises the server discovered event if a new server has
		///   been discovered. The server shutdown event is raised for servers that are presumably no longer running.
		/// </summary>
		public void Update()
		{
			// Check for incoming discovery messages
			int size;
			while (_multicastSocket.TryReceive(_buffer, ref _serverEndPoint, out size))
			{
				using (var reader = BufferReader.Create(_buffer, 0, size, Endianess.Big))
					HandleDiscoveryMessage(reader);
			}

			// Remove all servers that have timed out
			for (var i = 0; i < _knownServers.Count; ++i)
			{
				if (!_knownServers[i].HasTimedOut)
					continue;

				if (ServerHasShutdown != null)
					ServerHasShutdown(_knownServers[i].EndPoint);

				_knownServers.RemoveAt(i);
				--i;
			}
		}

		/// <summary>
		///   Handles the discovery message that has just been received.
		/// </summary>
		/// <param name="reader">The reader that should be used to read the contents of the discovery message.</param>
		private void HandleDiscoveryMessage(BufferReader reader)
		{
			// Check if this is a valid message
			var type = (MessageType)reader.ReadByte();
			var appIdentifier = reader.ReadUInt32();
			var revision = reader.ReadByte();

			if (type != MessageType.Discovery || appIdentifier != Specification.AppIdentifier || revision != Specification.Revision)
			{
				Log.Warn("Ignored invalid discovery message from {0}.", _serverEndPoint);
				return;
			}

			// Check if we already know this server; if not add it, otherwise update the server's discovery time
			var server = _knownServers.SingleOrDefault(s => s.EndPoint.Equals(_serverEndPoint));
			if (server == null)
			{
				server = new ServerInfo { EndPoint = _serverEndPoint, DiscoveryTime = DateTime.Now };
				_knownServers.Add(server);

				if (ServerDiscovered != null)
					ServerDiscovered(_serverEndPoint);
			}
			else
				server.DiscoveryTime = DateTime.Now;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_multicastSocket.SafeDispose();
		}

		/// <summary>
		///   Stores information about a discovered server.
		/// </summary>
		private class ServerInfo
		{
			/// <summary>
			///   The last time a discovery message has been received from the server.
			/// </summary>
			public DateTime DiscoveryTime;

			/// <summary>
			///   The end point of the server.
			/// </summary>
			public IPEndPoint EndPoint;

			/// <summary>
			///   Gets a value indicating whether the server has timed out and is presumably no longer running.
			/// </summary>
			public bool HasTimedOut
			{
				get { return (DateTime.Now - DiscoveryTime).TotalMilliseconds > ServerTimeout; }
			}
		}
	}
}