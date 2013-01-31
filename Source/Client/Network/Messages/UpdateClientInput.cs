using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class UpdateClientInput : PooledObject<UpdateClientInput>, IUnreliableMessage
	{
		/// <summary>
		///   The input state that is sent with the message.
		/// </summary>
		private InputStateHistory _inputState;

		/// <summary>
		///   The identifier of the player that is change his or her input state.
		/// </summary>
		private Identifier _playerId;

		/// <summary>
		///   Gets or sets the timestamp of the message.
		/// </summary>
		public uint Timestamp { get; set; }

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public void Process(GameSession session)
		{
			Assert.That(false, "The client cannot process this type of message.");
		}

		/// <summary>
		///   Writes the message into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public void Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			buffer.WriteByte((byte)MessageType.UpdateClientInput);
			buffer.WriteIdentifier(_playerId);
			_inputState.Write(buffer);
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="playerId">The identifier of the player that is change his or her input state.</param>
		/// <param name="inputState">The input state that should be sent with the message.</param>
		public static UpdateClientInput Create(Identifier playerId, InputStateHistory inputState)
		{
			var update = GetInstance();
			update._inputState = inputState;
			update._playerId = playerId;
			return update;
		}
	}
}