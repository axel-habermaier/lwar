using System;

namespace Lwar.Client.Gameplay
{
	/// <summary>
	///   Represents a player.
	/// </summary>
	public class Player
	{
		/// <summary>
		///   Gets or sets the player's identifier.
		/// </summary>
		public Identifier Id { get; set; }

		/// <summary>
		///   Gets or sets the name of the player.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///   Gets or sets a value indicating whether this player instance is currently in use.
		/// </summary>
		public bool IsActive { get; set; }
	}
}