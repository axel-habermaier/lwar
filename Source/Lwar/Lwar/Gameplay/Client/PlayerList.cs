namespace Lwar.Gameplay.Client
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Network;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Manages the active players that participate in a game session.
	/// </summary>
	public sealed class PlayerList : DisposableObject, IEnumerable<Player>
	{
		/// <summary>
		///     The game session  the player list belongs to.
		/// </summary>
		private readonly ClientGameSession _gameSession;

		/// <summary>
		///     Maps generational identities to player instances.
		/// </summary>
		private readonly IdentifierMap<Player> _playerMap = new IdentifierMap<Player>(NetworkProtocol.MaxPlayers + 1);

		/// <summary>
		///     The list of active players.
		/// </summary>
		private readonly List<Player> _players = new List<Player>();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session  the player list belongs to.</param>
		public PlayerList(ClientGameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);

			ServerPlayer = Player.Create(gameSession, NetworkProtocol.ServerPlayerIdentity, "<Server>");
			_playerMap.Add(ServerPlayer.Identity, ServerPlayer);
			_gameSession = gameSession;
		}

		/// <summary>
		///     Gets or sets the local player.
		/// </summary>
		public Player LocalPlayer { get; set; }

		/// <summary>
		///     Gets the server player.
		/// </summary>
		public Player ServerPlayer { get; private set; }

		/// <summary>
		///     Gets the player that corresponds to the given identity. Returns null if no player with the given identity could
		///     be found, or if the generation did not match.
		/// </summary>
		/// <param name="identity">The identity of the player that should be returned.</param>
		public Player this[Identity identity]
		{
			get { return _playerMap[identity]; }
		}

		/// <summary>
		///     Returns an enumerator that iterates through the collection.
		/// </summary>
		IEnumerator<Player> IEnumerable<Player>.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///     Returns an enumerator that iterates through the collection.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///     Adds the given player to the list.
		/// </summary>
		/// <param name="playerIdentifier">The identity of the player that should be added.</param>
		/// <param name="name">The name of the new player.</param>
		public void Add(Identity playerIdentifier, string name)
		{
			if (playerIdentifier == NetworkProtocol.ServerPlayerIdentity)
				return;

			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.That(!_playerMap.Contains(playerIdentifier), "A player with the same id has already been added.");

			var player = Player.Create(_gameSession, playerIdentifier, name);
			_players.Add(player);
			_playerMap.Add(player.Identity, player);

			if (PlayerAdded != null)
				PlayerAdded(player);

			UpdatePlayerNameUniqueness();
		}

		/// <summary>
		///     Removes the player with the given id from the list.
		/// </summary>
		/// <param name="playerIdentifier">The id of the player that should be removed.</param>
		public void Remove(Identity playerIdentifier)
		{
			Assert.ArgumentSatisfies(LocalPlayer == null || playerIdentifier != LocalPlayer.Identity, "Cannot remove the local player.");
			Assert.ArgumentSatisfies(_playerMap.Contains(playerIdentifier), "Cannot remove unknown player.");

			var player = _playerMap[playerIdentifier];

			_players.Remove(player);
			_playerMap.Remove(player.Identity);

			if (PlayerRemoved != null)
				PlayerRemoved(player);

			UpdatePlayerNameUniqueness();
			player.SafeDispose();
		}

		/// <summary>
		///     Raised when a player has been added.
		/// </summary>
		public event Action<Player> PlayerAdded;

		/// <summary>
		///     Raised when a player has been removed.
		/// </summary>
		public event Action<Player> PlayerRemoved;

		/// <summary>
		///     Raised when the stats of a player have been updated.
		/// </summary>
		public event Action<Player> PlayerStatsUpdated;

		/// <summary>
		///     Enumerates all active players.
		/// </summary>
		public List<Player>.Enumerator GetEnumerator()
		{
			return _players.GetEnumerator();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			ServerPlayer.SafeDispose();
			_players.SafeDisposeAll();
		}

		/// <summary>
		///     Changes the name of the player with the given id.
		/// </summary>
		/// <param name="playerIdentifier">The id of the player whose name should be changed.</param>
		/// <param name="name">The new name of the player.</param>
		public void ChangeName(Identity playerIdentifier, string name)
		{
			Assert.ArgumentNotNull(name);

			var player = _playerMap[playerIdentifier];
			Assert.NotNull(player, "Cannot change the name of an unknown player.");

			player.Name = name;
			UpdatePlayerNameUniqueness();
		}

		/// <summary>
		///     Updates the statistics of the corresponding player.
		/// </summary>
		/// <param name="playerIdentifier">The identity of the player that should be updated.</param>
		/// <param name="kills">The number of kills the player has scored.</param>
		/// <param name="deaths">The number of deaths of the player.</param>
		/// <param name="ping">The current ping of the player.</param>
		/// <param name="sequenceNumber">The sequence number of the player update.</param>
		public void UpdateStats(Identity playerIdentifier, int kills, int deaths, int ping, uint sequenceNumber)
		{
			var player = this[playerIdentifier];
			if (player == null || !player.AcceptUpdate(sequenceNumber))
				return;

			var hasChanged = player.Kills != kills ||
							 player.Deaths != deaths ||
							 player.Ping != ping;

			if (!hasChanged)
				return;

			player.Kills = kills;
			player.Deaths = deaths;
			player.Ping = ping;

			if (PlayerStatsUpdated != null)
				PlayerStatsUpdated(player);
		}

		/// <summary>
		///     Updates the players' name uniqueness states.
		/// </summary>
		private void UpdatePlayerNameUniqueness()
		{
			foreach (var player in _players)
				player.HasUniqueName = true;

			for (var i = 0; i < _players.Count; ++i)
			{
				for (var j = i + 1; j < _players.Count; ++j)
				{
					using (var name1 = new TextString(_players[i].Name))
					using (var name2 = new TextString(_players[j].Name))
					{
						if (!name1.Equals(name2))
							continue;
					}

					_players[i].HasUniqueName = false;
					_players[j].HasUniqueName = false;
				}
			}
		}
	}
}