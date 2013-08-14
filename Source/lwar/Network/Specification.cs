using System;

namespace Lwar.Network
{
	using System.Net;
	using Gameplay;

	/// <summary>
	///   Provides access to lwar network specification constants.
	/// </summary>
	public static class Specification
	{
		/// <summary>
		///   The application identifier that is used to determine whether a packet has been sent by another instance of the same
		///   application.
		/// </summary>
		public const uint AppIdentifier = 0xf27087c5;

		/// <summary>
		///   The maximum allowed byte length of an UTF8-encoded player name.
		/// </summary>
		public const int PlayerNameLength = 32;

		/// <summary>
		///   The maximum allowed byte length of an UTF8-encoded chat message.
		/// </summary>
		public const int ChatMessageLength = 255;

		/// <summary>
		///   The maximum allowed packet size in bytes.
		/// </summary>
		public const int MaxPacketSize = 512;

		/// <summary>
		///   The size of the packet header in bytes.
		/// </summary>
		public const int HeaderSize = 12;

		/// <summary>
		///   The maximum allowed number of concurrently active players.
		/// </summary>
		public const int MaxPlayers = 8;

		/// <summary>
		///   The frequency in Hz that determines how often the user input is sent to the server.
		/// </summary>
		public const int InputUpdateFrequency = 30;

		/// <summary>
		///   The default server port.
		/// </summary>
		public const ushort DefaultServerPort = 32422;

		/// <summary>
		///   The factor that angles sent over the network are scaled with.
		/// </summary>
		public const float AngleFactor = 100.0f;

		/// <summary>
		///   The revision number of the network protocol that is implemented by the client.
		/// </summary>
		public const byte Revision = 18;

		/// <summary>
		///   The number of discovery messages that should be sent per minute for automatic server discovery.
		/// </summary>
		public const double DiscoveryMessageFrequency = 12;

		/// <summary>
		///   The identifier of the player that represents the server.
		/// </summary>
		public static readonly Identifier ServerPlayerIdentifier = new Identifier(0, 0);

		/// <summary>
		///   The reserved entity identifier
		/// </summary>
		public static readonly Identifier ReservedEntityIdentifier = new Identifier(UInt16.MaxValue, 0);

		/// <summary>
		///   The multicast group that is used for automatic server discovery.
		/// </summary>
		public static readonly IPEndPoint MulticastGroup = new IPEndPoint(IPAddress.Parse("FF05::3"), DefaultServerPort + 1);
	}
}