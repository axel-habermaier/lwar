namespace Lwar.Network.Messages
{
	using System;
	using System.Runtime.InteropServices;
	using Gameplay;
	using Pegasus.Math;

	/// <summary>
	///   Holds the payload of an UpdateCircle message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct UpdateCircleMessage
	{
		/// <summary>
		///   The entity that is updated.
		/// </summary>
		public Identifier Entity;

		/// <summary>
		///   The new circle center.
		/// </summary>
		public Vector2 Center;

		/// <summary>
		///   The new circle radius.
		/// </summary>
		public float Radius;
	}
}