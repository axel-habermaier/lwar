namespace Lwar.Network.Messages
{
	using System;
	using System.Runtime.InteropServices;
	using Gameplay;
	using Pegasus.Math;

	/// <summary>
	///   Holds the payload of an Update message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct UpdateMessage
	{
		/// <summary>
		///   The entity that is updated.
		/// </summary>
		public Identifier Entity;

		/// <summary>
		///   The new entity health.
		/// </summary>
		public int Health;

		/// <summary>
		///   The new entity position.
		/// </summary>
		public Vector2 Position;

		/// <summary>
		///   The new entity rotation.
		/// </summary>
		public float Rotation;
	}
}