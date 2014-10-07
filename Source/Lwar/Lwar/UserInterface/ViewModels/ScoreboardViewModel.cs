namespace Lwar.UserInterface.ViewModels
{
	using System;
	using System.Linq;
	using Gameplay;
	using Pegasus;
	using Pegasus.Framework;

	/// <summary>
	///     Displays the score board.
	/// </summary>
	public class ScoreboardViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     Indicates whether the player list is potentially dirty.
		/// </summary>
		private bool _isDirty = true;

		/// <summary>
		///     Indicates whether the scoreboard should be shown.
		/// </summary>
		private bool _isVisible;

		/// <summary>
		///     The players of the current game session.
		/// </summary>
		private PlayerList _players;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that is played.</param>
		public ScoreboardViewModel(GameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);

			_players = gameSession.Players;
			Players = new ObservableCollection<Player>();

			_players.PlayerAdded += OnPlayerAdded;
			_players.PlayerRemoved += OnPlayerRemoved;
			_players.PlayerStatsUpdated += OnPlayerStatsChanged;
		}

		/// <summary>
		///     Gets the players of the game session.
		/// </summary>
		public ObservableCollection<Player> Players { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the scoreboard should be shown.
		/// </summary>
		public bool IsVisible
		{
			get { return _isVisible; }
			set { ChangePropertyValue(ref _isVisible, value); }
		}

		/// <summary>
		///     Adds a player to the scoreboard.
		/// </summary>
		/// <param name="player">The player that should be added.</param>
		private void OnPlayerAdded(Player player)
		{
			_isDirty = true;
			Players.Add(player);
		}

		/// <summary>
		///     Removes a player from the scoreboard.
		/// </summary>
		/// <param name="player">The player that should be removed.</param>
		private void OnPlayerRemoved(Player player)
		{
			_isDirty = true;
			Players.Remove(player);
		}

		/// <summary>
		///     Marks the player list as dirty, as the new player stats potentially require the player list to be reordered.
		/// </summary>
		/// <param name="player">The player whose stats have changed.</param>
		private void OnPlayerStatsChanged(Player player)
		{
			_isDirty = true;
		}

		/// <summary>
		///     Updates the scoreboard.
		/// </summary>
		public void Update()
		{
			if (!_isDirty || !IsVisible)
				return;

			_isDirty = false;

			Players.Clear();
			Players.AddRange(_players
				.OrderByDescending(player => player.Kills)
				.ThenBy(player => player.Deaths)
				.ThenBy(player => player.DisplayName));
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_players.PlayerAdded -= OnPlayerAdded;
			_players.PlayerRemoved -= OnPlayerRemoved;
			_players.PlayerStatsUpdated -= OnPlayerStatsChanged;
		}
	}
}