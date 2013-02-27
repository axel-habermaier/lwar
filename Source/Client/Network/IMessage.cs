using System;

namespace Lwar.Client.Network
{
	using Gameplay;
	using Pegasus.Framework.Platform;
	using Rendering;

	/// <summary>
	///   Represents a message that is used for the communication between the server and the clients.
	/// </summary>
	public interface IMessage : IDisposable
	{
		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="gameSession">The game session that should be affected by the message.</param>
		/// <param name="renderContext">The render context that should be affected by the message.</param>
		void Process(GameSession gameSession, RenderContext renderContext);

		/// <summary>
		///   Writes the message into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		bool Write(BufferWriter buffer);
	}
}