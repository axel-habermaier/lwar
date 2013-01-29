using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class UpdateClientInput : PooledObject<UpdateClientInput>, IUnreliableMessage
	{
		/// <summary>
		///   The size of the message in bytes.
		/// </summary>
		private const int Size = sizeof(byte) + Identifier.Size + 5 * sizeof(byte);

		/// <summary>
		///   Indicates whether the player wants to move backwards.
		/// </summary>
		private bool _backward;

		/// <summary>
		///   Indicates whether the player wants to move forward.
		/// </summary>
		private bool _forward;

		/// <summary>
		///   Indicates whether the player wants to move to the left.
		/// </summary>
		private bool _left;

		/// <summary>
		///   The identifier of the player that is added.
		/// </summary>
		private Identifier _playerId;

		/// <summary>
		///   Indicates whether the player wants to to the right.
		/// </summary>
		private bool _right;

		/// <summary>
		///   Indicates whether the player wants to shoot.
		/// </summary>
		private bool _shooting;

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
		///   Serializes the message into the given buffer, returning false if the message did not fit.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public bool Serialize(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			if (!buffer.CanWrite(Size))
				return false;

			buffer.WriteByte((byte)MessageType.UpdateClientInput);
			buffer.WriteIdentifier(_playerId);
			buffer.WriteBoolean(_forward);
			buffer.WriteBoolean(_backward);
			buffer.WriteBoolean(_left);
			buffer.WriteBoolean(_right);
			buffer.WriteBoolean(_shooting);

			return true;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		public UpdateClientInput Create(Identifier playerId, bool forward, bool backward, bool left, bool right, bool shooting)
		{
			var update = GetInstance();
			update._forward = forward;
			update._backward = backward;
			update._left = left;
			update._right = right;
			update._shooting = shooting;
			update._playerId = playerId;
			return update;
		}
	}
}