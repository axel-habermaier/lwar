using System;

namespace Lwar.Network.Messages
{
	using System.Runtime.InteropServices;
	using Gameplay;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Holds the payload of a Collision message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct CollisionMessage
	{
		/// <summary>
		///   The first entity involved in the collision.
		/// </summary>
		public Identifier Entity1;

		/// <summary>
		///   The second entity involved in the collision.
		/// </summary>
		public Identifier Entity2;

		/// <summary>
		///   The position of the impact.
		/// </summary>
		public Vector2 Position;
	}
}