using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;

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
	}
}