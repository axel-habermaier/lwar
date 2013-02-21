using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Manages the current and the previous seven input stated.
	/// </summary>
	public struct InputStateHistory
	{
		private byte _backward;
		private byte _forward;
		private byte _turnLeft;
		private byte _turnRight;
		private byte _strafeLeft;
		private byte _strafeRight;
		private byte _shooting1;
		private byte _shooting2;
		private byte _shooting3;
		private byte _shooting4;

		private Vector2 _target;

		/// <summary>
		///   The input state version that is incremented each time the input state is changed.
		/// </summary>
		private uint _version;

		/// <summary>
		///   Updates the current input state, also storing the seven previous states.
		/// </summary>
		public void Update(bool forward, bool backward, bool turnLeft, bool turnRight, bool strafeLeft, bool strafeRight,
		                   bool shooting1, bool shooting2, bool shooting3, bool shooting4, Vector2 target)
		{
			_forward = Update(_forward, forward);
			_backward = Update(_backward, backward);
			_turnLeft = Update(_turnLeft, turnLeft);
			_turnRight = Update(_turnRight, turnRight);
			_strafeLeft = Update(_strafeLeft, strafeLeft);
			_strafeRight = Update(_strafeRight, strafeRight);
			_shooting1 = Update(_shooting1, shooting1);
			_shooting2 = Update(_shooting2, shooting2);
			_shooting3 = Update(_shooting3, shooting3);
			_shooting4 = Update(_shooting4, shooting4);
			_target = target;

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
			buffer.WriteByte(_turnLeft);
			buffer.WriteByte(_turnRight);
			buffer.WriteByte(_strafeLeft);
			buffer.WriteByte(_strafeRight);
			buffer.WriteByte(_shooting1);
			buffer.WriteByte(_shooting2);
			buffer.WriteByte(_shooting3);
			buffer.WriteByte(_shooting4);
			buffer.WriteInt16((short)_target.X);
			buffer.WriteInt16((short)_target.Y);
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