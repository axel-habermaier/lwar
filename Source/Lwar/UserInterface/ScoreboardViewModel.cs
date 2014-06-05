namespace Lwar.UserInterface
{
	using System;
	using System.Linq;
	using Gameplay;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Platform.Input;
	using Scripting;

	/// <summary>
	///     Displays the score board.
	/// </summary>
	public class ScoreboardViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The input that causes the scoreboard to be shown. TODO: Replace with UI framework input
		/// </summary>
		private readonly LogicalInput _showScoreboard = new LogicalInput(Cvars.InputShowScoreboardCvar, InputLayers.Game);

		/// <summary>
		///     Indicates whether the player list is potentially dirty.
		/// </summary>
		private bool _isDirty;

		/// <summary>
		///     The players of the current game session.
		/// </summary>
		private PlayerList _players;

		/// <summary>
		///     Indicates whether the scoreboard should be shown.
		/// </summary>
		private bool _isVisible;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that is played.</param>
		public ScoreboardViewModel(GameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);

			_players = gameSession.Players;
			Players = new ObservableCollection<Player>();
			Application.Current.Window.InputDevice.Add(_showScoreboard);

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
			private set { ChangePropertyValue(ref _isVisible, value); }
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
			IsVisible = _showScoreboard.IsTriggered;

			if (!_isDirty || !IsVisible)
				return;

			_isDirty = false;

			Players.Clear();
			Players.AddRange(_players
				.OrderByDescending(player => player.Kills)
				.ThenBy(player => player.Deaths)
				.ThenBy(player => player.Name));
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