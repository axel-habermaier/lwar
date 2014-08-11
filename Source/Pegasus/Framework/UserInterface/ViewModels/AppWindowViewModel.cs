namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using Controls;
	using Math;
	using Platform.Memory;
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
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="mode">Indicates the window mode.</param>
		internal AppWindowViewModel(string title, Vector2i position, Size size, WindowMode mode)
		{
			Window = new AppWindow(title, position, size, mode);
			Console = new ConsoleViewModel(Window);
			DebugOverlay = new DebugOverlayViewModel(Window);

			Window.DataContext = this;
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
		///     Updates the application window.
		/// </summary>
		public void Update()
		{
			Console.Update();
			DebugOverlay.Update();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Console.SafeDispose();
			Window.SafeDispose();
			_debugOverlay.SafeDispose();
		}
	}
}