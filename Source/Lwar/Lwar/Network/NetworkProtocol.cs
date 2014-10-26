namespace Lwar.Network
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Platform.Network;

	/// <summary>
	///     Provides access to lwar network specification constants.
	/// </summary>
	public static class NetworkProtocol
	{
		/// <summary>
		///     The application identifier that is used to determine whether a packet has been sent by another instance of the same
		///     application.
		/// </summary>
		public const uint AppIdentifier = 0xf27087c5;

		/// <summary>
		///     The duration in milliseconds that the connection waits for a new packet from the remote peer before the connection is
		///     considered to be dropped.
		/// </summary>
		public const int DroppedTimeout = 10000;

		/// <summary>
		///     The duration in milliseconds that the connection waits for a new packet from the remote peer before the connection is
		///     considered to be lagging.
		/// </summary>
		public const int LaggingTimeout = 500;

		/// <summary>
		///     The maximum allowed byte length of an UTF8-encoded player name.
		/// </summary>
		public const int PlayerNameLength = 32;

		/// <summary>
		///     The maximum allowed byte length of an UTF8-encoded server name.
		/// </summary>
		public const int ServerNameLength = 32;

		/// <summary>
		///     The maximum allowed byte length of an UTF8-encoded chat message.
		/// </summary>
		public const int ChatMessageLength = 255;

		/// <summary>
		///     The maximum allowed packet size in bytes.
		/// </summary>
		public const int MaxPacketSize = 512;

		/// <summary>
		///     The size of the packet header in bytes.
		/// </summary>
		public const int HeaderSize = 8;

		/// <summary>
		///     The number of weapon slots of a player ship.
		/// </summary>
		public const int WeaponSlotCount = 4;

		/// <summary>
		///     The maximum allowed number of concurrently active players.
		/// </summary>
		public const int MaxPlayers = 8;

		/// <summary>
		///     The frequency in Hz that determines how often the user input is sent to the server.
		/// </summary>
		public const int InputUpdateFrequency = 30;

		/// <summary>
		///     The default server port.
		/// </summary>
		public const ushort DefaultServerPort = 32422;

		/// <summary>
		///     The factor that angles sent over the network are scaled with.
		/// </summary>
		public const float AngleFactor = 100.0f;

		/// <summary>
		///     The revision number of the network protocol that is implemented by the client.
		/// </summary>
		public const byte Revision = 27;

		/// <summary>
		///     The time (in seconds) after which a discovered server has presumably shut down.
		/// </summary>
		public const float DiscoveryTimeout = 5;

		/// <summary>
		///     The frequency in Hz that determines how often a server sends a discovery message.
		/// </summary>
		public const float DiscoveryFrequency = 1;

		/// <summary>
		///     The multicast time to live that is used for automatic server discovery.
		/// </summary>
		public const int MulticastTimeToLive = 1;

		/// <summary>
		///     The identity of the player that represents the server.
		/// </summary>
		public static readonly Identity ServerPlayerIdentity = new Identity(0, 0);

		/// <summary>
		///     The reserved entity identity.
		/// </summary>
		public static readonly Identity ReservedEntityIdentity = new Identity(UInt16.MaxValue, 0);

		/// <summary>
		///     The multicast group that is used for automatic server discovery.
		/// </summary>
		public static readonly IPEndPoint MulticastGroup = new IPEndPoint(IPAddress.Parse("FF05::3"), 32423);
	}
}