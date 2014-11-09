namespace Lwar.Gameplay.Server
{
	using System;
	using System.Collections.Generic;
	using Network;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Manages the active players that participate in a game session.
	/// </summary>
	public sealed class PlayerCollection : DisposableObject
	{
		/// <summary>
		///     The list of active players.
		/// </summary>
		private readonly List<Player> _players = new List<Player>();

		/// <summary>
		///     Indicates whether players are managed in server mode.
		/// </summary>
		private readonly bool _serverMode;

		/// <summary>
		///     The allocator that is used to allocate network identities for the players.
		/// </summary>
		private NetworkIdentityAllocator _identityAllocator;

		/// <summary>
		///     Maps network identities to actual player objects.
		/// </summary>
		private NetworkIdentityMap<Player> _identityMap;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate game objects.</param>
		/// <param name="serverMode">Indicates whether players should be managed in server mode.</param>
		public PlayerCollection(PoolAllocator allocator, bool serverMode)
		{
			Assert.ArgumentNotNull(allocator);

			_serverMode = serverMode;

			if (_serverMode)
			{
				_identityAllocator = new NetworkIdentityAllocator(NetworkProtocol.MaxPlayers + 1);
				Add(ServerPlayer = Player.Create(allocator, "<server>", NetworkProtocol.ServerPlayerIdentity));
			}
			else
				_identityMap = new NetworkIdentityMap<Player>(NetworkProtocol.MaxPlayers + 1);
		}

		/// <summary>
		///     Gets the player representing the server.
		/// </summary>
		public Player ServerPlayer { get; private set; }

		/// <summary>
		///     Gets the number of active players.
		/// </summary>
		public int Count
		{
			get { return _players.Count; }
		}

		/// <summary>
		///     Gets an enumerator that can be used enumerate all active players.
		/// </summary>
		public List<Player>.Enumerator GetEnumerator()
		{
			return _players.GetEnumerator();
		}

		/// <summary>
		///     Adds the given player to the list.
		/// </summary>
		/// <param name="player">The player that should be added.</param>
		public void Add(Player player)
		{
			Assert.ArgumentNotNull(player);
			Assert.That(!_players.Contains(player), "The player has already been added.");

			if (_serverMode)
				player.Identity = _identityAllocator.Allocate();
			else
				_identityMap.Add(player.Identity, player);

			_players.Add(player);
		}

		/// <summary>
		///     Removes the player from the list.
		/// </summary>
		/// <param name="player">The player that should be removed.</param>
		public void Remove(Player player)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentSatisfies(_players.Contains(player), "Cannot remove an unknown player.");

			_players.Remove(player);

			if (_serverMode)
				_identityAllocator.Free(player.Identity);
			else
				_identityMap.Remove(player.Identity);

			player.SafeDispose();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_players.SafeDisposeAll();
		}
	}
}