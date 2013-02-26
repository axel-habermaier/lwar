using System;

namespace Lwar.Client.Gameplay
{
	using Entities;

	/// <summary>
	///   Represents a player.
	/// </summary>
	public class Player : IGenerationalIdentity
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="id">The identifier of the player.</param>
		public Player(Identifier id)
		{
			Id = id;
		}

		/// <summary>
		///   Gets or sets the name of the player.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///   Gets or sets the player's ship.
		/// </summary>
		public Ship Ship { get; set; }

		/// <summary>
		///   Gets the player's identifier.
		/// </summary>
		public Identifier Id { get; private set; }
	}
}