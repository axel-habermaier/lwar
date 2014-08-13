namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using Platform.Memory;
	using Scripting;
	using Views;

	/// <summary>
	///     The view model for the main app window.
	/// </summary>
	internal class AppWindowViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The view model of the console.
		/// </summary>
		private ConsoleViewModel _console;

		/// <summary>
		///     The view model of the debug overlay.
		/// </summary>
		private DebugOverlayViewModel _debugOverlay;

		/// <summary>
		///     Indicates whether the window should be in fullscreen or windowed mode.
		/// </summary>
		private bool _fullscreen;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="consoleViewModel">The view model that should be used for the in-game console.</param>
		internal AppWindowViewModel(ConsoleViewModel consoleViewModel)
		{
			Assert.ArgumentNotNull(consoleViewModel);

			Console = consoleViewModel;
			Console.Initialize();

			DebugOverlay = new DebugOverlayViewModel();
			Window = new AppWindow(this, Application.Current.Name, Cvars.WindowPosition, Cvars.WindowSize, Cvars.WindowMode);

			Cvars.FullscreenChanged += OnFullscreenCvarChanged;
		}

		/// <summary>
		///     Gets the window the application is rendered to.
		/// </summary>
		public AppWindow Window { get; private set; }

		/// <summary>
		///     Gets or sets a value indicating whether the window should be in fullscreen or windowed mode.
		/// </summary>
		public bool Fullscreen
		{
			get { return _fullscreen; }
			private set { ChangePropertyValue(ref _fullscreen, value); }
		}

		/// <summary>
		///     Gets the view model of the console.
		/// </summary>
		public ConsoleViewModel Console
		{
			get { return _console; }
			private set { ChangePropertyValue(ref _console, value); }
		}

		/// <summary>
		///     Gets the view model of the debug overlay.
		/// </summary>
		public DebugOverlayViewModel DebugOverlay
		{
			get { return _debugOverlay; }
			private set { ChangePropertyValue(ref _debugOverlay, value); }
		}

		/// <summary>
		///     Updates the window's mode based on the cvar's value.
		/// </summary>
		/// <param name="previousValue">The previous value of the fullscreen cvar.</param>
		private void OnFullscreenCvarChanged(bool previousValue)
		{
			Fullscreen = !previousValue;
		}

		/// <summary>
		///     Toggles between fullscreen and windowed mode.
		/// </summary>
		public void ToggleFullscreen()
		{
			Cvars.Fullscreen = !Cvars.Fullscreen;
		}

		/// <summary>
		///     Shows the console.
		/// </summary>
		public void ShowConsole()
		{
			Console.IsVisible = true;
		}

		/// <summary>
		///     Updates the application window.
		/// </summary>
		public void Update()
		{
			DebugOverlay.Update();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Cvars.FullscreenChanged -= OnFullscreenCvarChanged;

			Console.SafeDispose();
			DebugOverlay.SafeDispose();
			Window.SafeDispose();
		}
	}
}