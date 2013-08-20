using System;

namespace Lwar.Network.Messages
{
	using System.Runtime.InteropServices;
	using Gameplay;
	using Pegasus.Math;

	/// <summary>
	///   Holds the payload of an Input message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct InputMessage
	{
		/// <summary>
		///   The boolean state value for the backwards input, including the seven previous states.
		/// </summary>
		public byte Backward;

		/// <summary>
		///   The boolean state value for the forward input, including the seven previous states.
		/// </summary>
		public byte Forward;

		/// <summary>
		///   The monotonically increasing frame number, starting at 1.
		/// </summary>
		public uint FrameNumber;

		/// <summary>
		///   The player that generated the input.
		/// </summary>
		public Identifier Player;

		/// <summary>
		///   The boolean state value for the first shooting input, including the seven previous states.
		/// </summary>
		public byte Shooting1;

		/// <summary>
		///   The boolean state value for the second shooting input, including the seven previous states.
		/// </summary>
		public byte Shooting2;

		/// <summary>
		///   The boolean state value for the third shooting input, including the seven previous states.
		/// </summary>
		public byte Shooting3;

		/// <summary>
		///   The boolean state value for the fourth shooting input, including the seven previous states.
		/// </summary>
		public byte Shooting4;

		/// <summary>
		///   The boolean state value for the strafe left input, including the seven previous states.
		/// </summary>
		public byte StrafeLeft;

		/// <summary>
		///   The boolean state value for the strafe right input, including the seven previous states.
		/// </summary>
		public byte StrafeRight;

		/// <summary>
		///   The position of the client's target relative to the client's ship.
		/// </summary>
		public Vector2 Target;

		/// <summary>
		///   The boolean state value for the turn left input, including the seven previous states.
		/// </summary>
		public byte TurnLeft;

		/// <summary>
		///   The boolean state value for the turn right input, including the seven previous states.
		/// </summary>
		public byte TurnRight;
	}
}