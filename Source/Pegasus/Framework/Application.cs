namespace Pegasus.Framework
{
	using System;
	using System.Threading;
	using Assets;
	using Platform;
	using Platform.Assets;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Performance;
	using Rendering;
	using Scripting;
	using UserInterface;
	using UserInterface.Controls;

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
		///     Indicates whether the application should continue to run.
		/// </summary>
		private bool _running = true;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		protected Application()
		{
			Commands.OnExit += Exit;
			Current = this;
		}

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
		public ResourceDictionary Resources
		{
			get { return _root.Resources; }
		}

		/// <summary>
		///     Gets the graphics device that is used to render the application.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }

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
		public AppWindow Window { get; private set; }

		/// <summary>
		///     Invoked when the application is initializing.
		/// </summary>
		protected abstract void Initialize();

		/// <summary>
		///     Invoked when the application should update the its state.
		/// </summary>
		protected abstract void Update();

		/// <summary>
		///     Invoked when the application should draw a frame.
		/// </summary>
		/// <param name="output">The render output that should be used to draw the frame.</param>
		protected abstract void Draw(RenderOutput output);

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
		/// <param name="logFile">The log file that is used to serialize log message to the disk.</param>
		internal void Run(string name, LogFile logFile)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.ArgumentNotNull(logFile);

			Name = name;

			using (GraphicsDevice = new GraphicsDevice())
			using (Assets = new AssetsManager(GraphicsDevice))
			using (Window = new AppWindow(name, Cvars.WindowPosition, Cvars.WindowSize, Cvars.WindowMode))
			using (var resolutionManager = new ResolutionManager(Window.NativeWindow, Window.SwapChain))
			{
				Window.Title = name;
				RegisterFontLoader(new FontLoader(Assets));

				// Copy the recorded log history to the console and explain the usage of the console
				logFile.WriteToConsole(Window.Console);
				Commands.Help();

				// Let the application initialize itself
				Initialize();

				while (_running)
				{
					using (new Measurement(Window.DebugOverlay.CpuFrameTime))
					{
						// Handle all input
						_root.HandleInput();

						// Update the application logic and the UI
						Update();

						resolutionManager.Update();
						_root.UpdateLayout();

						// Update the statistics
						Window.DebugOverlay.Update(Window.Size);
						Window.Console.Update(Window.Size);

						// Draw the current frame
						GraphicsDevice.BeginFrame();

						// Let the application perform all custom drawing for the current frame
						Draw(Window.RenderOutput);

						// Draw the user interface
						_root.Draw();

						GraphicsDevice.EndFrame();
						Window.DebugOverlay.GpuFrameTime.AddMeasurement(GraphicsDevice.FrameTime);
					}

					// Presents the contents of all windows' backbuffers.
					_root.Present();

					if (!_root.HasFocusedWindows)
						Thread.Sleep(50);
				}

				// The game loop has been exited; time to clean up
				Dispose();
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