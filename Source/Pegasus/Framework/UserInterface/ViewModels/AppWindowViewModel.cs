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
		///     Initializes a new instance.
		/// </summary>
		/// <param name="consoleViewModel">The view model that should be used for the in-game console.</param>
		internal AppWindowViewModel(ConsoleViewModel consoleViewModel)
		{
			Assert.ArgumentNotNull(consoleViewModel);

			Console = consoleViewModel;
			Console.InitializePrompt();

			DebugOverlay = new DebugOverlayViewModel();
			Window = new AppWindow(this, Application.Current.Name, Cvars.WindowPosition, Cvars.WindowSize, Cvars.WindowMode);
		}

		/// <summary>
		///     Gets the window the application is rendered to.
		/// </summary>
		public AppWindow Window { get; private set; }

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
			Console.SafeDispose();
			DebugOverlay.SafeDispose();
			Window.SafeDispose();
		}
	}
}