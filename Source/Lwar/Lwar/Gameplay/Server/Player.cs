﻿namespace Lwar.Gameplay.Server
{
	using System;
	using Network;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a player that is participating in a game session.
	/// </summary>
	public class Player : UniquePooledObject
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Player()
		{
			ConstructorCache.Register(() => new Player());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Player()
		{
			WeaponTypes = new EntityType[NetworkProtocol.WeaponSlotCount];
		}

		/// <summary>
		///     Gets or sets the name of the player.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Gets or sets the player's ping.
		/// </summary>
		public int Ping { get; set; }

		/// <summary>
		///     Gets or sets the reason why the player left the game session.
		/// </summary>
		public LeaveReason LeaveReason { get; set; }

		/// <summary>
		///     Gets or sets the number of kills that the player has scored.
		/// </summary>
		public int Kills { get; set; }

		/// <summary>
		///     Gets or sets the number of deaths.
		/// </summary>
		public int Deaths { get; set; }

		/// <summary>
		///     Gets or sets the entity controlled by the player.
		/// </summary>
		public Entity ControlledEntity { get; set; }

		/// <summary>
		///     Gets or sets the selected ship type.
		/// </summary>
		public EntityType ShipType { get; set; }

		/// <summary>
		///     Gets the selected weapon types.
		/// </summary>
		public EntityType[] WeaponTypes { get; private set; }

		/// <summary>
		///     Gets or sets a value indicating whether this player is the server player.
		/// </summary>
		public bool IsServerPlayer
		{
			get { return Identity == NetworkProtocol.ServerPlayerIdentity; }
		}

		/// <summary>
		///     Gets or sets the player's network identity.
		/// </summary>
		public Identity Identity { get; set; }

		/// <summary>
		///     Allocates a player using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the player.</param>
		/// <param name="name">The name of the player.</param>
		/// <param name="identity">The network identity of the player.</param>
		public static Player Create(PoolAllocator allocator, string name, Identity identity = default(Identity))
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNullOrWhitespace(name);

			var player = allocator.Allocate<Player>();
			player.Identity = identity;
			player.Name = name;
			player.Kills = 0;
			player.Deaths = 0;
			player.Ping = 0;
			player.ShipType = EntityType.Ship;
			player.LeaveReason = LeaveReason.Unknown;

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				player.WeaponTypes[i] = EntityType.Gun;

			return player;
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("'{0}' ({1})", Name, Identity);
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			Assert.That(!IsServerPlayer || ControlledEntity == Entity.None, "The server player cannot control an entity.");
		}
	}
}