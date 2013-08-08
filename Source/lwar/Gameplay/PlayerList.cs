﻿using System;

namespace Lwar.Gameplay
{
	using System.Collections;
	using System.Collections.Generic;
	using Network.Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Memory;

	/// <summary>
	///   Manages the active players that participate in a game session.
	/// </summary>
	public sealed class PlayerList : DisposableObject, IEnumerable<Player>
	{
		/// <summary>
		///   Maps generational identifiers to player instances.
		/// </summary>
		private readonly IdentifierMap<Player> _playerMap = new IdentifierMap<Player>();

		/// <summary>
		///   The list of active players.
		/// </summary>
		private readonly DeferredList<Player> _players = new DeferredList<Player>(false);

		/// <summary>
		///   Gets the local player.
		/// </summary>
		public Player LocalPlayer { get; private set; }

		/// <summary>
		///   Gets the player that corresponds to the given identifier. Returns null if no player with the given identifier could
		///   be found, or if the generation did not match.
		/// </summary>
		/// <param name="identifier">The identifier of the player that should be returned.</param>
		public Player this[Identifier identifier]
		{
			get { return _playerMap[identifier]; }
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		IEnumerator<Player> IEnumerable<Player>.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///   Adds the given player to the list.
		/// </summary>
		/// <param name="playerId">The identifier of the player that should be added.</param>
		/// <param name="name">The name of the new player.</param>
		/// <param name="isLocalPlayer">Indicates whether the new player is the local one.</param>
		public void Add(Identifier playerId, string name, bool isLocalPlayer)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.That(_playerMap[playerId] == null, "A player with the same id has already been added.");
			Assert.That(!isLocalPlayer || LocalPlayer == null, "Cannot change the local player.");

			var player = Player.Create(playerId, name);
			_players.Add(player);
			_playerMap.Add(player);

			if (isLocalPlayer)
			{
				LocalPlayer = player;
				LocalPlayer.IsLocalPlayer = true;
			}
		}

		/// <summary>
		///   Removes the player with the given id from the list.
		/// </summary>
		/// <param name="playerId">The id of the player that should be removed.</param>
		public void Remove(Identifier playerId)
		{
			Assert.ArgumentSatisfies(LocalPlayer == null || playerId != LocalPlayer.Id,
									 "Cannot remove the local player.");

			var player = _playerMap[playerId];
			Assert.NotNull(player, "Cannot remove unknown player.");

			_players.Remove(player);
			_playerMap.Remove(player);
		}

		/// <summary>
		///   Updates the player list.
		/// </summary>
		public void Update()
		{
			_players.Update();
		}

		/// <summary>
		///   Enumerates all active players.
		/// </summary>
		public List<Player>.Enumerator GetEnumerator()
		{
			return _players.GetEnumerator();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_players.SafeDispose();
		}

		/// <summary>
		///   Changes the name of the player with the given id.
		/// </summary>
		/// <param name="playerId">The id of the player whose name should be changed.</param>
		/// <param name="name">The new name of the player.</param>
		public void ChangeName(Identifier playerId, string name)
		{
			Assert.ArgumentNotNull(name);

			var player = _playerMap[playerId];
			Assert.NotNull(player, "Cannot change the name of an unknown player.");

			player.Name = name;
		}

		/// <summary>
		/// Updates the statistics of the corresponding player.
		/// </summary>
		/// <param name="stats">The updated statistics that should be copied to the player instance.</param>
		public void UpdateStats(StatsMessage stats)
		{
			var player = this[stats.Player];
			if (player == null)
				return;

			player.Kills = stats.Kills;
			player.Deaths = stats.Deaths;
			player.Ping = stats.Ping;
		}
	}
}