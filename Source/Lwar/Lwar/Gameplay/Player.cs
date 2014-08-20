﻿namespace Lwar.Gameplay
{
	using System;
	using Entities;
	using Pegasus;
	using Pegasus.Framework;

	/// <summary>
	///     Represents a player.
	/// </summary>
	public class Player : PooledNotifyPropertyChanged, IGenerationalIdentity
	{
		/// <summary>
		///     The number of deaths.
		/// </summary>
		private int _deaths;

		/// <summary>
		///     The game session the player belongs to.
		/// </summary>
		private GameSession _gameSession;

		/// <summary>
		///     The number of kills that the player has scored.
		/// </summary>
		private int _kills;

		/// <summary>
		///     The name of the player
		/// </summary>
		private string _name;

		/// <summary>
		///     The player's ping.
		/// </summary>
		private int _ping;

		/// <summary>
		///     Gets or sets the name of the player.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { ChangePropertyValue(ref _name, value); }
		}

		/// <summary>
		///     Gets or sets the player's ping.
		/// </summary>
		public int Ping
		{
			get { return _ping; }
			set { ChangePropertyValue(ref _ping, value); }
		}

		/// <summary>
		///     Gets or sets the number of kills that the player has scored.
		/// </summary>
		public int Kills
		{
			get { return _kills; }
			set { ChangePropertyValue(ref _kills, value); }
		}

		/// <summary>
		///     Gets or sets the number of deaths.
		/// </summary>
		public int Deaths
		{
			get { return _deaths; }
			set { ChangePropertyValue(ref _deaths, value); }
		}

		/// <summary>
		///     Gets or sets the player's ship.
		/// </summary>
		public Ship Ship { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether this player is the local one.
		/// </summary>
		public bool IsLocalPlayer { get; set; }

		/// <summary>
		///     Gets the player's identifier.
		/// </summary>
		public Identifier Identifier { get; private set; }

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The identifier of the player.</param>
		/// <param name="name">The name of the player.</param>
		public static Player Create(GameSession gameSession, Identifier id, string name)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNullOrWhitespace(name);

			var player = gameSession.Allocate<Player>();
			player._gameSession = gameSession;
			player.Identifier = id;
			player.Ship = null;
			player.Name = name;
			return player;
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("'{0}' ({1})", Name, Identifier);
		}
	}
}