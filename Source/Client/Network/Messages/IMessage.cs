using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework.Platform;

	/// <summary>
	///   Represents a message that is used for the communication between the server and the clients.
	/// </summary>
	public interface IMessage : IDisposable
	{
		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		void Process(GameSession session);

		/// <summary>
		///   Serializes the message into the given buffer, returning false if the message did not fit.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		bool Serialize(BufferWriter buffer);
	}
}