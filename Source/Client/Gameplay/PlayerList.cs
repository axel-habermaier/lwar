using System;

namespace Lwar.Client.Gameplay
{
	using Network;
	using Pegasus.Framework;

	/// <summary>
	///   Manages the currently active players.
	/// </summary>
	public class PlayerList
	{
		/// <summary>
		///   The statically allocated player instances.
		/// </summary>
		private readonly Player[] _players = new Player[Specification.MaxPlayers + 1];

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public PlayerList()
		{
			for (var i = 0; i < _players.Length; ++i)
				_players[i] = new Player();
		}

		/// <summary>
		///   Gets the active player instance that corresponds to the given player identifier. Returns null
		///   if there currently is no active player with the given identifier.
		/// </summary>
		/// <param name="playerId">The identifier of the player that should be returned.</param>
		public Player this[Identifier playerId]
		{
			get
			{
				var player = _players[playerId.Id];
				return player.Id.Generation == playerId.Generation ? player : null;
			}
		}

		/// <summary>
		///   Adds a player to the list of active players.
		/// </summary>
		/// <param name="playerId">The identifier of the new player.</param>
		public void Add(Identifier playerId)
		{
			var player = _players[playerId.Id];
			Assert.That(!player.IsActive, "An active player instance is reused.");

			player.Id = playerId;
			player.IsActive = true;
		}

		/// <summary>
		///   Removes the player from the list of active players.
		/// </summary>
		/// <param name="playerId">The identifier of the player that should be removed.</param>
		public void Remove(Identifier playerId)
		{
			var player = this[playerId];
			Assert.That(player != null, "Unable to find player that should be removed.");

			player.IsActive = false;
			player.Name = null;
		}
	}
}