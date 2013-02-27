using System;

namespace Lwar.Client.Gameplay
{
	using Entities;
	using Pegasus.Framework;

	/// <summary>
	///   Represents a player.
	/// </summary>
	public class Player : PooledObject<Player>, IGenerationalIdentity
	{
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

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The identifier of the player.</param>
		public static Player Create(Identifier id)
		{
			var player = GetInstance();
			player.Id = id;
			player.Ship = null;
			player.Name = String.Empty;
			return player;
		}
	}
}