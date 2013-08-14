using System;

namespace Lwar.Gameplay
{
	using Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Memory;

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
		///   Gets or sets the player's ping.
		/// </summary>
		public int Ping { get; set; }

		/// <summary>
		///   Gets or sets the number of kills that the player has scored.
		/// </summary>
		public int Kills { get; set; }

		/// <summary>
		///   Gets or sets the number of deaths.
		/// </summary>
		public int Deaths { get; set; }

		/// <summary>
		///   Gets or sets the player's ship.
		/// </summary>
		public Ship Ship { get; set; }

		/// <summary>
		///   Gets or sets a value indicating whether this player is the local one.
		/// </summary>
		public bool IsLocalPlayer { get; set; }

		/// <summary>
		///   Gets the player's identifier.
		/// </summary>
		public Identifier Identifier { get; private set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The identifier of the player.</param>
		/// <param name="name">The name of the player.</param>
		public static Player Create(Identifier id, string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);

			var player = GetInstance();
			player.Identifier = id;
			player.Ship = null;
			player.Name = name;
			return player;
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("'{0}' ({1})", Name, Identifier);
		}
	}
}