using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework.Platform;

	/// <summary>
	///   Manages the current and the previous seven input stated.
	/// </summary>
	public struct InputStateHistory
	{
		/// <summary>
		///   Indicates whether the player moves backwards.
		/// </summary>
		private byte _backward;

		/// <summary>
		///   Indicates whether the player moves foward
		/// </summary>
		private byte _forward;

		/// <summary>
		///   Indicates whether the player moves to the left.
		/// </summary>
		private byte _left;

		/// <summary>
		///   The orientation of the player.
		/// </summary>
		private ushort _orientation;

		/// <summary>
		///   Indicates whether the player moves to the left.
		/// </summary>
		private byte _right;

		/// <summary>
		///   Indicates whether the player is shooting.
		/// </summary>
		private byte _shooting;

		/// <summary>
		///   The input state version that is incremented each time the input state is changed.
		/// </summary>
		private uint _version;

		/// <summary>
		///   Updates the current input state, also storing the seven previous states.
		/// </summary>
		public void Update(bool forward, bool backward, bool left, bool right, bool shooting, ushort orientation)
		{
			_forward = Update(_forward, forward);
			_backward = Update(_backward, backward);
			_left = Update(_left, left);
			_right = Update(_right, right);
			_shooting = Update(_shooting, shooting);
			_orientation = orientation;

			_version += 1;
		}

		/// <summary>
		///   Writes the current input state into the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the input state should be written into.</param>
		public void Write(BufferWriter buffer)
		{
			buffer.WriteUInt32(_version);
			buffer.WriteByte(_forward);
			buffer.WriteByte(_backward);
			buffer.WriteByte(_left);
			buffer.WriteByte(_right);
			buffer.WriteByte(_shooting);
			buffer.WriteUInt16(_orientation);
		}

		/// <summary>
		///   Removes the oldest input state and adds the new one.
		/// </summary>
		/// <param name="previous">The previous input state.</param>
		/// <param name="current">Indicates whether the state bit should be set.</param>
		private static byte Update(byte previous, bool current)
		{
			return (byte)((previous << 1) | (current ? 1 : 0));
		}
	}
}