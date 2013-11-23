namespace Pegasus.Framework
{
	using System;
	using System.Linq;
	using Platform.Assets;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Rendering;
	using Scripting;
	using UserInterface;
	using UserInterface.Controls;

	/// <summary>
	///   Represents the application.
	/// </summary>
	public abstract class Application : DisposableObject
	{
		/// <summary>
		///   The root of the visual tree managed by the application.
		/// </summary>
		private readonly RootUIElement _root = new RootUIElement();

		/// <summary>
		///   Indicates whether the application should continue to run.
		/// </summary>
		private bool _running = true;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected Application()
		{
			Commands.OnExit += Exit;
		}

		/// <summary>
		///   Gets the application-wide resources.
		/// </summary>
		public ResourceDictionary Resources
		{
			get { return _root.Resources; }
		}

		/// <summary>
		///   Gets the graphics device that is used to render the application.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///   Gets the name of the application.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the application-wide assets manager.
		/// </summary>
		public AssetsManager Assets { get; private set; }

		/// <summary>
		///   Invoked when the application should update the its state.
		/// </summary>
		protected abstract void Update();

		/// <summary>
		///   Exists the application.
		/// </summary>
		protected void Exit()
		{
			Log.Info("Exiting...");
			_running = false;
		}

		/// <summary>
		///   Invoked when the application is initializing.
		/// </summary>
		protected abstract void Initialize();

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Assets.SafeDispose();
			GraphicsDevice.SafeDispose();
		}

		/// <summary>
		///   Runs the application. This method does not return until the application is shut down.
		/// </summary>
		/// <param name="name">The name of the application.</param>
		internal void Run(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);

			Name = name;
			GraphicsDevice = new GraphicsDevice();
			Assets = new AssetsManager(GraphicsDevice);

			InitializeFontLoader();
			Initialize();

			using (var spriteEffect = InitializeSpriteEffect())
			using (var spriteBatch = new SpriteBatch(GraphicsDevice, spriteEffect))
			while (_running)
			{
				_root.HandleInput();

				Update();

				_root.UpdateLayout();
				_root.Draw(spriteBatch);
			}
		}

		/// <summary>
		///   Creates a new window.
		/// </summary>
		/// <param name="window">The window that should be shown.</param>
		public void ShowWindow(Window window)
		{
			Assert.ArgumentNotNull(window);
			
			_root.Add(window);
			window.Open(this);
		}

		/// <summary>
		///   Removes the window from the application.
		/// </summary>
		/// <param name="window">The window that should be removed.</param>
		internal void RemoveWindow(Window window)
		{
			Assert.ArgumentNotNull(window);
			_root.Remove(window);
		}

		/// <summary>
		///   Initializes the font loader instance.
		/// </summary>
		private void InitializeFontLoader()
		{
			var fontLoaderType = (from a in AppDomain.CurrentDomain.GetAssemblies()
								  from t in a.GetTypes()
								  where typeof(IFontLoader).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass
								  select t).SingleOrDefault();

			if (fontLoaderType == null)
				Log.Die("Unable to find an implementation of '{0}' or multiple implementations were found.", typeof(IFontLoader));

			_root.Resources.Add(typeof(IFontLoader), Activator.CreateInstance(fontLoaderType, Assets));
		}

		/// <summary>
		///   Initializes the sprite effect.
		/// </summary>
		private ISpriteEffect InitializeSpriteEffect()
		{
			var spriteEffectType = (from a in AppDomain.CurrentDomain.GetAssemblies()
									from t in a.GetTypes()
									where typeof(ISpriteEffect).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass
									select t).SingleOrDefault();

			if (spriteEffectType == null)
				Log.Die("Unable to find an implementation of '{0}' or multiple implementations were found.", typeof(ISpriteEffect));

			var spriteEffect = (ISpriteEffect)Activator.CreateInstance(spriteEffectType);
			spriteEffect.Initialize(GraphicsDevice, Assets);
			return spriteEffect;
		}
	}
}

///// <summary>
//		///   Runs the application. This method does not return until the application is shut down.
//		/// </summary>
//		/// <param name="logFile">The log file that writes all generated log entries to the file system.</param>
//		/// <param name="appName">The name of the application.</param>
//		/// <param name="defaultFont">The default font that is used to draw the console and the statistics.</param>
//		internal void Run(LogFile logFile, string appName, AssetIdentifier<Font> defaultFont)
//		{
//			Assert.ArgumentNotNull(logFile);

//			using (new NativeLibrary())
//			using (var graphicsDevice = new GraphicsDevice())
//			using (var window = new NativeWindow(appName, Cvars.WindowPosition, Cvars.WindowSize, Cvars.WindowMode))
//			using (var swapChain = new SwapChain(graphicsDevice, window, Cvars.Fullscreen, Cvars.Resolution))
//			using (var assets = new AssetsManager(graphicsDevice))
//			using (var keyboard = new Keyboard(window))
//			using (var mouse = new Mouse(window))
//			using (var inputDevice = new LogicalInputDevice(keyboard, mouse))
//			using (var bindings = new Bindings(inputDevice))
//			using (var resolutionManager = new ResolutionManager(window, swapChain))
//			using (var camera2D = new Camera2D(graphicsDevice))
//			using (var sceneOutput = new RenderOutput(graphicsDevice) { RenderTarget = swapChain.BackBuffer })
//			using (var uiOutput = new RenderOutput(graphicsDevice) { Camera = camera2D, RenderTarget = swapChain.BackBuffer })
//			{
//				window.Title = appName;
//				InitializeFontLoader(assets);

//				var font = assets.LoadFont(defaultFont);
//				using (var statistics = new DebugOverlay(graphicsDevice, font))
//				using (var spriteEffect = InitializeSpriteEffect(graphicsDevice, assets))
//				using (var spriteBatch = new SpriteBatch(graphicsDevice, spriteEffect))
//				using (var console = new Console(graphicsDevice, inputDevice, font, appName))
//				{
//					// Ensure that the console and the statistics are properly initialized
//					statistics.Update(window.Size);
//					console.Update(window.Size);

//					// Copy the recorded log history to the console and explain the usage of the console
//					logFile.WriteToConsole(console);
//					Commands.Help();

//					// Let the application initialize itself
//					Initialize();

//					// Initialize the sprite batch
//					spriteBatch.BlendState = BlendState.Premultiplied;
//					spriteBatch.DepthStencilState = DepthStencilState.DepthDisabled;
//					spriteBatch.SamplerState = SamplerState.PointClampNoMipmaps;

//					inputDevice.ActivateLayer(new InputLayer(1)); // TODO: Refactor this
//					while (_running)
//					{
//						// Update the keyboard and mouse state first (this ensures that WentDown returns 
//						// false for all keys and buttons, etc.)
//						inputDevice.Keyboard.Update();
//						inputDevice.Mouse.Update();

//						// Process all window events 
//						window.ProcessEvents();

//						// Update the logical inputs based on the new state of the input system
//						inputDevice.Update();

//						// Check if any command bindings have been triggered and update the resolution manager
//						bindings.Update();
//						resolutionManager.Update();

//						// Update the application logic
//						Update();

//						// Update the user interface
//						var size = new SizeD(window.Width, window.Height);
//						_canvas.Measure(size);
//						_canvas.Arrange(new RectangleD(0, 0, size));

//						// Update the statistics
//						statistics.Update(window.Size);
//						console.Update(window.Size);

//						// React to window size changes
//						var viewport = new Rectangle(Vector2i.Zero, window.Size);
//						sceneOutput.Viewport = viewport;
//						uiOutput.Viewport = viewport;
//						camera2D.Viewport = viewport;

//						// Draw the current frame
//						using (new Measurement(statistics.GraphicsDeviceProfiler))
//						using (new Measurement(statistics.CpuFrameTime))
//						{
//							// Let the application draw the 3D elements of the current frame
//							Draw(sceneOutput);

//							// Let the application draw the 2D elements of the current frame
//							DepthStencilState.DepthDisabled.Bind();
//							BlendState.Premultiplied.Bind();
//							DrawUserInterface(spriteBatch);
//							_canvas.Draw(spriteBatch);

//							// Draw the console and the statistics on top of the current frame
//							statistics.Draw(spriteBatch);
//							console.Draw(spriteBatch);

//							spriteBatch.DrawBatch(uiOutput);
//						}

//						// Present the current frame to the screen and write the log file, if necessary
//						swapChain.Present();
//						logFile.WriteToFile();

//						if (!window.Focused)
//							Thread.Sleep(50);
//					}

//					// The game loop has been exited; time to clean up
//					Dispose();
//				}
//			}
//		}