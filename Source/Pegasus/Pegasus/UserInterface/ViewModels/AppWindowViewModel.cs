namespace Pegasus.UserInterface.ViewModels
{
	using System;
	using System.Text;
	using Controls;
	using UserInterface;
	using Math;
	using Platform.Memory;
	using Scripting;
	using Utilities;
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
		private bool _fullscreen = Cvars.WindowMode == WindowMode.Fullscreen;

		/// <summary>
		///     The particle effect viewer that is currently open.
		/// </summary>
		private static ParticleEffectViewerViewModel _particleEffectViewer;

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

			Commands.OnShowParticleEffectViewer += ShowParticleEffectViewer;
		}

		/// <summary>
		///     Gets the window the application is rendered to.
		/// </summary>
		public AppWindow Window { get; private set; }

		/// <summary>
		///     Gets or sets the size of the window.
		/// </summary>
		public Size Size
		{
			get { return Cvars.WindowSize; }
			set
			{
				if (Cvars.WindowMode == WindowMode.Normal && Cvars.WindowSize != value)
					Cvars.WindowSize = value;
			}
		}

		/// <summary>
		///     Gets or sets the position of the window.
		/// </summary>
		public Vector2 Position
		{
			get { return Cvars.WindowPosition; }
			set
			{
				if (Cvars.WindowMode == WindowMode.Normal && Cvars.WindowPosition != value)
					Cvars.WindowPosition = value;
			}
		}

		/// <summary>
		///     Gets or sets the mode of the window.
		/// </summary>
		public WindowMode WindowMode
		{
			get { return Cvars.WindowMode; }
			set { Cvars.WindowMode = value; }
		}

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
		///     Shows the particle effect viewer.
		/// </summary>
		private void ShowParticleEffectViewer()
		{
			_particleEffectViewer.SafeDispose();
			_particleEffectViewer = new ParticleEffectViewerViewModel();
		}

		/// <summary>
		///     Toggles between fullscreen and windowed mode.
		/// </summary>
		public void ToggleFullscreen()
		{
			Fullscreen = !Fullscreen;
		}

		/// <summary>
		///     Shows or hides the console.
		/// </summary>
		public void ToggleConsole()
		{
			Console.IsVisible = !Console.IsVisible;
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
			Console.SafeDispose();
			DebugOverlay.SafeDispose();
			_particleEffectViewer.SafeDispose();
			Window.SafeDispose();
		}
	}
}