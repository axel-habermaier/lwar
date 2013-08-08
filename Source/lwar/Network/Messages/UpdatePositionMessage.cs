using System;

namespace Lwar.Network.Messages
{
	using System.Runtime.InteropServices;
	using Gameplay;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Holds the payload of an UpdatePosition message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct UpdatePositionMessage
	{
		/// <summary>
		///   The entity that is updated.
		/// </summary>
		public Identifier Entity;

		/// <summary>
		///   The new entity position.
		/// </summary>
		public Vector2 Position;
	}
}