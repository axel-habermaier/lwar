using System;

namespace Lwar.Client.Network.Messages
{
	using System.Runtime.InteropServices;
	using Gameplay;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Holds the payload of an UpdateRay message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct UpdateRayMessage
	{
		/// <summary>
		///   The entity that is updated.
		/// </summary>
		public Identifier Entity;

		/// <summary>
		///   The new ray direction.
		/// </summary>
		public float Direction;

		/// <summary>
		///   The new ray length.
		/// </summary>
		public float Length;

		/// <summary>
		///   The new ray origin.
		/// </summary>
		public Vector2 Origin;

		/// <summary>
		///   The target entity that is hit by the ray, if any.
		/// </summary>
		public Identifier Target;
	}
}