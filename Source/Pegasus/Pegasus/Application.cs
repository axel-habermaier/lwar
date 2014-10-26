namespace Pegasus
{
	using System;
	using System.Threading;
	using Assets;
	using Platform.Graphics;
	using Platform.Logging;
	using Rendering.Particles;
	using Scripting;
	using UserInterface;
	using UserInterface.Controls;
	using UserInterface.Input;
	using UserInterface.ViewModels;
	using UserInterface.Views;
	using Utilities;

	/// <summary>
	///     Represents the application. There can be only one instance per app domain.
	/// </summary>
	public abstract class Application
	{
		/// <summary>
		///     The application instance of this app domain.
		/// </summary>
		private static Application _current;

		/// <summary>
		///     The root of the visual tree managed by the application.
		/// </summary>
		private readonly RootUIElement _root = new RootUIElement();

		/// <summary>
		///     Gets the view model of the window the application is rendered to.
		/// </summary>
		private AppWindowViewModel _appWindowViewModel;

		/// <summary>
		///     Indicates whether the application should continue to run.
		/// </summary>
		private bool _running = true;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		protected Application()
		{
			Current = this;
		}

		/// <summary>
		///     Gets the application startup time in seconds, i.e., the timestamp of the internal CPU clock at application startup.
		/// </summary>
		/// <summary>
		///     Gets the application instance of this app domain.
		/// </summary>
		public static Application Current
		{
			get
			{
				Assert.NotNull(_current, "No application is currently active in this app domain.");
				return _current;
			}
			private set
			{
				Assert.That(_current == null || value == null,
					"There can only be one instance of '{0}' per app domain.", typeof(Application).FullName);

				_current = value;
			}
		}

		/// <summary>
		///     Gets the application-wide resources.
		/// </summary>
		protected ResourceDictionary Resources
		{
			get { return _root.Resources; }
		}

		/// <summary>
		///     Indicates whether the console is currently open.
		/// </summary>
		public bool IsConsoleOpen
		{
			get { return _appWindowViewModel.Console.IsVisible; }
		}

		/// <summary>
		///     Gets the name of the application.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     Gets the application-wide assets manager.
		/// </summary>
		public AssetsManager Assets { get; private set; }

		/// <summary>
		///     Gets the window the application is rendered to.
		/// </summary>
		public AppWindow Window
		{
			get { return _appWindowViewModel.Window; }
		}

		/// <summary>
		///     Gets the graphics device of the application.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///     Gets the view model of the debug overlay.
		/// </summary>
		private DebugOverlayViewModel DebugOverlay
		{
			get { return _appWindowViewModel.DebugOverlay; }
		}

		/// <summary>
		///     Invoked when the application is initializing.
		/// </summary>
		protected abstract void Initialize();

		/// <summary>
		///     Invoked when the application should update the its state.
		/// </summary>
		protected abstract void Update();

		/// <summary>
		///     Exists the application.
		/// </summary>
		protected void Exit()
		{
			Log.Info("Exiting {0}...", Name);
			_running = false;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected virtual void Dispose()
		{
		}

		/// <summary>
		///     Runs the application. This method does not return until the application is shut down.
		/// </summary>
		/// <param name="name">The name of the application.</param>
		/// <param name="consoleViewModel">The view model that should be used for the in-game console.</param>
		internal unsafe void Run(string name, ConsoleViewModel consoleViewModel)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.ArgumentNotNull(consoleViewModel);

			Name = name;

			using (GraphicsDevice = new GraphicsDevice())
			using (Assets = new AssetsManager(GraphicsDevice, asyncLoading: false))
			{
				RegisterFontLoader(new FontLoader(Assets));
				Cursors.Load(Assets);

				using (_appWindowViewModel = new AppWindowViewModel(consoleViewModel))
				{
					Initialize();
					Commands.Help();
					Commands.OnExit += Exit;

					while (_running)
					{
						// Update the input, application, and UI state
						double updateTime, drawTime;
						using (TimeMeasurement.Measure(&updateTime))
						{
							_root.HandleInput();
							Update();

							_appWindowViewModel.Update();
							_root.UpdateLayout();
						}

						// Draw the frame
						GraphicsDevice.BeginFrame();
						using (TimeMeasurement.Measure(&drawTime))
							_root.Draw();
						GraphicsDevice.EndFrame();

						// Update the debug overlay and particle statistics
						DebugOverlay.GpuTime = GraphicsDevice.FrameTime;
						DebugOverlay.RenderTime = drawTime;
						DebugOverlay.UpdateTime = updateTime;
						ParticleStatistics.UpdateDebugOverlay(DebugOverlay);

						// Present the contents of all windows' backbuffers
						_root.Present();

						// Save CPU when there are no focused windows
						if (!_root.HasFocusedWindows)
							Thread.Sleep(10);
					}

					// The game loop has been exited; time to clean up
					Dispose();
				}
			}

			Current = null;
		}

		/// <summary>
		///     Adds the window to the application.
		/// </summary>
		/// <param name="window">The window that should be added.</param>
		internal void AddWindow(Window window)
		{
			Assert.ArgumentNotNull(window);
			_root.Children.Add(window);
		}

		/// <summary>
		///     Removes the window from the application.
		/// </summary>
		/// <param name="window">The window that should be removed.</param>
		internal void RemoveWindow(Window window)
		{
			Assert.ArgumentNotNull(window);
			_root.Children.Remove(window);
		}

		/// <summary>
		///     Registers the given font loader on the application.
		/// </summary>
		/// <param name="fontLoader">The font loader that should be registered.</param>
		protected void RegisterFontLoader(IFontLoader fontLoader)
		{
			Assert.ArgumentNotNull(fontLoader);

			object currentFontLoader;
			if (_root.Resources.TryGetValue(typeof(IFontLoader), out currentFontLoader))
				fontLoader.Next = (IFontLoader)currentFontLoader;

			_root.Resources.AddOrReplace(typeof(IFontLoader), fontLoader);
		}
	}
}