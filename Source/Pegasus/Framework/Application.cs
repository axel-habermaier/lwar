namespace Pegasus.Framework
{
	using System;
	using System.Threading;
	using Assets;
	using Math;
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
	///     Represents the application.
	/// </summary>
	public abstract class Application
	{
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
		public NativeWindow Window { get; private set; }

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
		/// <param name="output">The output the frame should be rendered to.</param>
		protected abstract void Draw(RenderOutput output);

		/// <summary>
		///     Invoked when the application should draw the user interface.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		protected abstract void DrawUserInterface(SpriteBatch spriteBatch);

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

			using (var graphicsDevice = new GraphicsDevice())
			using (var window = new NativeWindow(name, Cvars.WindowPosition, Cvars.WindowSize, Cvars.WindowMode))
			using (var swapChain = new SwapChain(graphicsDevice, window, Cvars.Fullscreen, Cvars.Resolution))
			using (var assets = new AssetsManager(graphicsDevice))
			using (var keyboard = new Keyboard(window))
			using (var mouse = new Mouse(window))
			using (var inputDevice = new LogicalInputDevice(keyboard, mouse))
			using (var bindings = new Bindings(inputDevice))
			using (var resolutionManager = new ResolutionManager(window, swapChain))
			using (var camera2D = new Camera2D(graphicsDevice))
			using (var sceneOutput = new RenderOutput(graphicsDevice) { RenderTarget = swapChain.BackBuffer })
			using (var uiOutput = new RenderOutput(graphicsDevice) { Camera = camera2D, RenderTarget = swapChain.BackBuffer })
			{
				window.Title = name;
				RegisterFontLoader(new FontLoader(assets));

				var font = assets.LoadFont(Fonts.LiberationMono11);
				using (var debugOverlay = new DebugOverlay(graphicsDevice, font))
				using (var spriteBatch = new SpriteBatch(graphicsDevice, assets))
				using (var console = new Console(graphicsDevice, inputDevice, font, name))
				{
					// Ensure that the console and the statistics are properly initialized
					debugOverlay.Update(window.Size);
					console.Update(window.Size);

					// Copy the recorded log history to the console and explain the usage of the console
					logFile.WriteToConsole(console);
					Commands.Help();

					// Let the application initialize itself
					GraphicsDevice = graphicsDevice;
					Assets = assets;
					InputDevice = inputDevice;
					Window = window;
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

						// Process all window events 
						window.ProcessEvents();

						// Update the user interface and the logical inputs based on the new state of the input system
						_root.HandleInput();
						inputDevice.Update();

						// Check if any command bindings have been triggered and update the resolution manager
						bindings.Update();
						resolutionManager.Update();

						// Update the application logic
						Update();

						// Update the statistics
						debugOverlay.Update(window.Size);
						console.Update(window.Size);

						// React to window size changes
						var viewport = new Rectangle(Vector2i.Zero, window.Size);
						sceneOutput.Viewport = viewport;
						uiOutput.Viewport = viewport;
						camera2D.Viewport = viewport;

						// Draw the current frame
						using (new Measurement(debugOverlay.GraphicsDeviceProfiler))
						using (new Measurement(debugOverlay.CpuFrameTime))
						{
							// Let the application draw the 3D elements of the current frame
							Draw(sceneOutput);

							// Let the application draw the 2D elements of the current frame
							DepthStencilState.DepthDisabled.Bind();
							BlendState.Premultiplied.Bind();
							DrawUserInterface(spriteBatch);

							// Draw the user interface
							_root.UpdateLayout();
							_root.Draw(spriteBatch);

							// Draw the console and the statistics on top of the current frame
							debugOverlay.Draw(spriteBatch);
							console.Draw(spriteBatch);

							spriteBatch.DrawBatch(uiOutput);
						}

						// Present the current frame to the screen and write the log file, if necessary
						swapChain.Present();
						//logFile.WriteToFile();

						if (!_root.HasFocusedWindows)
							Thread.Sleep(50);
					}

					// The game loop has been exited; time to clean up
					Dispose();
				}
			}
		}

		/// <summary>
		///     Creates a new window.
		/// </summary>
		/// <param name="window">The window that should be shown.</param>
		public void ShowWindow(Window window)
		{
			Assert.ArgumentNotNull(window);

			_root.Add(window);
			window.Open(this);
		}

		/// <summary>
		///     Removes the window from the application.
		/// </summary>
		/// <param name="window">The window that should be removed.</param>
		internal void RemoveWindow(Window window)
		{
			Assert.ArgumentNotNull(window);
			_root.Remove(window);
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