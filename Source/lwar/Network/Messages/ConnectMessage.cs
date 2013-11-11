namespace Lwar.Network.Messages
{
	using System;
	using System.Runtime.InteropServices;
	using System.Text;
	using Pegasus;

	/// <summary>
	///   Holds the payload of a Connect message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct ConnectMessage
	{
		/// <summary>
		///   The name of the player that is connecting.
		/// </summary>
		public string Name;

		/// <summary>
		///   The revision number of the network protocol that the connecting client implements.
		/// </summary>
		public byte NetworkRevision;

		/// <summary>
		///   Creates a connect message.
		/// </summary>
		/// <param name="playerName">The name of the player that is connecting.</param>
		public static Message Create(string playerName)
		{
			Assert.ArgumentNotNullOrWhitespace(playerName);
			Assert.That(Encoding.UTF8.GetByteCount(playerName) <= Specification.PlayerNameLength, "Player name is too long.");

			return new Message
			{
				Type = MessageType.Connect,
				Connect = new ConnectMessage
				{
					Name = playerName,
					NetworkRevision = Specification.Revision
				}
			};
		}
	}
}