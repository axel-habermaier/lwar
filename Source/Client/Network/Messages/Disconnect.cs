﻿using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class Disconnect : PooledObject<Disconnect>, IReliableMessage
	{
		/// <summary>
		///   The size of the message in bytes.
		/// </summary>
		private static readonly int Size = sizeof(byte);

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public void Process(GameSession session)
		{
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

			buffer.WriteByte((byte)MessageType.Disconnect);
			return true;
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }
	}
}