namespace Lwar.Network.Messages
{
	using System;
	using System.Runtime.InteropServices;
	using Gameplay;
	using Gameplay.Entities;

	/// <summary>
	///   Holds the payload of an Add message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct AddMessage
	{
		/// <summary>
		///   The entity that is added.
		/// </summary>
		public Identifier Entity;

		/// <summary>
		///   The player the entity belongs to.
		/// </summary>
		public Identifier Player;

		/// <summary>
		///   The type of the entity that is added.
		/// </summary>
		public EntityType Type;
	}
}