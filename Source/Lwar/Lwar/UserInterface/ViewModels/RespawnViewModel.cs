namespace Lwar.UserInterface.ViewModels
{
	using System;
	using Gameplay;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface.Input;
	using Scripting;

	/// <summary>
	///     Presents the respawn screen.
	/// </summary>
	public class RespawnViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The game session that is played.
		/// </summary>
		private GameSession _gameSession;

		/// <summary>
		///     Indicates whether the view is visible.
		/// </summary>
		private bool _isVisible;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that should be displayed.</param>
		public RespawnViewModel(GameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);

			_gameSession = gameSession;
			IsVisible = false;
			RespawnInput = Cvars.InputRespawn.ToString();
			Cvars.InputRespawnChanging += OnRespawnInputChanged;
		}

		/// <summary>
		///     Gets the textual representation of the respawn input.
		/// </summary>
		public string RespawnInput { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the view is visible.
		/// </summary>
		public bool IsVisible
		{
			get { return _isVisible; }
			private set { ChangePropertyValue(ref _isVisible, value); }
		}

		/// <summary>
		///     Updates the respawn view.
		/// </summary>
		public void Update()
		{
			IsVisible = _gameSession.LocalPlayer.Ship == null;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Cvars.InputRespawnChanging -= OnRespawnInputChanged;
		}

		/// <summary>
		///     Notifies the view about the new respawn input.
		/// </summary>
		/// <param name="input">The new respawn input.</param>
		private void OnRespawnInputChanged(ConfigurableInput input)
		{
			RespawnInput = input.ToString();
			OnPropertyChanged("RespawnInput");
		}
	}
}