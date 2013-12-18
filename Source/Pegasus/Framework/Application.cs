namespace Pegasus.Framework
{
	using System;
	using System.Threading;
	using Assets;
	using Platform;
	using Platform.Assets;
	using Platform.Graphics;
	using Platform.Input;
	using Platform.Logging;
	using Platform.Performance;
	using Rendering;
	using Scripting;
	using UserInterface;
	using UserInterface.Controls;
	using Console = Rendering.UserInterface.Console;

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
		///     Gets the application-wide Assets manager.
		/// </summary>
		public AssetsManager Assets { get; private set; }

		/// <summary>
		///     Gets the Window the application is rendered to.
		/// </summary>
		public AppWindow Window { get; private set; }

		/// <summary>
		///     Gets the logical input device that the application uses to handle user input.
		/// </summary>
		public LogicalInputDevice InputDevice { get; private set; }

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
			using (var keyboard = new Keyboard(Window.NativeWindow))
			using (var mouse = new Mouse(Window.NativeWindow))
			using (var inputDevice = new LogicalInputDevice(keyboard, mouse))
			using (var bindings = new Bindings(inputDevice))
			using (var resolutionManager = new ResolutionManager(Window.NativeWindow, Window.SwapChain))
			using (var camera = new Camera2D(GraphicsDevice))
			{
				Window.Title = name;
				RegisterFontLoader(new FontLoader(Assets));

				var font = Assets.LoadFont(Fonts.LiberationMono11);
				using (var debugOverlay = new DebugOverlay(GraphicsDevice, font))
				using (var spriteBatch = new SpriteBatch(GraphicsDevice, Assets))
				using (var console = new Console(GraphicsDevice, inputDevice, font, name))
				{
					// Ensure that the console and the statistics are properly initialized
					debugOverlay.Update(Window.Size);
					console.Update(Window.Size);

					// Copy the recorded log history to the console and explain the usage of the console
					logFile.WriteToConsole(console);
					Commands.Help();

					// Let the application initialize itself
					Assets = Assets;
					InputDevice = inputDevice;
					Window = Window;
					Initialize();

					// Initialize the sprite batch
					spriteBatch.BlendState = BlendState.Premultiplied;
					spriteBatch.DepthStencilState = DepthStencilState.DepthDisabled;
					spriteBatch.SamplerState = SamplerState.PointClampNoMipmaps;

					inputDevice.ActivateLayer(new InputLayer(1)); // TODO: Refactor this
					while (_running)
					{
						// Update the keyboard and mouse state first (this ensures that WentDown returns 
						// false for all keys and buttons, etc.)
						inputDevice.Keyboard.Update();
						inputDevice.Mouse.Update();

						// Process all Window events 
						Window.ProcessEvents();

						// Update the user interface and the logical inputs based on the new state of the input system
						_root.HandleInput();
						inputDevice.Update();

						// Check if any command bindings have been triggered and update the resolution manager
						bindings.Update();
						resolutionManager.Update();

						// Update the application logic and the UI
						Update();
						_root.UpdateLayout();

						// Update the statistics
						debugOverlay.Update(Window.Size);
						console.Update(Window.Size);

						// Draw the current frame
						using (new Measurement(debugOverlay.GraphicsDeviceProfiler))
						using (new Measurement(debugOverlay.CpuFrameTime))
						{
							// Let the application perform all custom drawing for the current frame
							Draw(Window.RenderOutput);

							// Draw the console and the statistics on top of the current frame
							//DepthStencilState.DepthDisabled.Bind();
							//BlendState.Premultiplied.Bind();

							//debugOverlay.Draw(spriteBatch);
							//console.Draw(spriteBatch);

							//spriteBatch.DrawBatch(uiOutput);

							// Draw the user interface
							_root.Draw();
						}

						if (!_root.HasFocusedWindows)
							Thread.Sleep(50);
					}

					// The game loop has been exited; time to clean up
					Dispose();
				}
			}

			Current = null;
		}

		/// <summary>
		///     Adds the Window to the application.
		/// </summary>
		/// <param name="Window">The Window that should be added.</param>
		internal void AddWindow(Window Window)
		{
			Assert.ArgumentNotNull(Window);
			_root.Children.Add(Window);
		}

		/// <summary>
		///     Removes the Window from the application.
		/// </summary>
		/// <param name="Window">The Window that should be removed.</param>
		internal void RemoveWindow(Window Window)
		{
			Assert.ArgumentNotNull(Window);
			_root.Children.Remove(Window);
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