using System;

namespace Pegasus.Framework
{
	using System.Threading;
	using Math;
	using Platform;
	using Platform.Assets;
	using Platform.Graphics;
	using Platform.Input;
	using Platform.Logging;
	using Platform.Performance;
	using Rendering;
	using Rendering.UserInterface;
	using Scripting;
	using UserInterface;
	using UserInterface.Controls;

	/// <summary>
	///   Represents the application.
	/// </summary>
	public abstract class Application
	{
		/// <summary>
		///   The root of the logical tree managed by the application.
		/// </summary>
		private readonly Canvas _canvas = new Canvas();

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
		///   Gets or sets the application-wide resources.
		/// </summary>
		public ResourceDictionary Resources
		{
			get { return _canvas.Resources; }
			set { _canvas.Resources = value; }
		}

		/// <summary>
		///   Invoked when the application should update the its state.
		/// </summary>
		protected abstract void Update();

		/// <summary>
		///   Invoked when the application should draw a frame.
		/// </summary>
		/// <param name="output">The output that the scene should be rendered to.</param>
		protected abstract void Draw(RenderOutput output);

		/// <summary>
		///   Invoked when the application should draw the user interface.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		protected abstract void DrawUserInterface(SpriteBatch spriteBatch);

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
		protected virtual void Initialize()
		{
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected virtual void Dispose()
		{
		}

		/// <summary>
		///   Runs the application. This method does not return until the application is shut down.
		/// </summary>
		/// <param name="logFile">The log file that writes all generated log entries to the file system.</param>
		/// <param name="appName">The name of the application.</param>
		/// <param name="defaultFont">The default font that is used to draw the console and the statistics.</param>
		/// <param name="spriteEffect">The sprite effect that should be used to draw the console and the statistics.</param>
		internal void Run(LogFile logFile, string appName, AssetIdentifier<Font> defaultFont, ISpriteEffect spriteEffect)
		{
			Assert.ArgumentNotNull(logFile);

			using (new NativeLibrary())
			using (var graphicsDevice = new GraphicsDevice())
			using (var window = new Window(appName, Cvars.WindowPosition, Cvars.WindowSize, Cvars.WindowMode))
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
				window.Title = appName;
				spriteEffect.Initialize(graphicsDevice, assets);

				var font = assets.LoadFont(defaultFont);
				using (var statistics = new DebugOverlay(graphicsDevice, font))
				using (spriteEffect)
				using (var spriteBatch = new SpriteBatch(graphicsDevice, spriteEffect))
				using (var console = new Console(graphicsDevice, inputDevice, font, appName))
				{
					// Ensure that the console and the statistics are properly initialized
					statistics.Update(window.Size);
					console.Update(window.Size);

					// Copy the recorded log history to the console and explain the usage of the console
					logFile.WriteToConsole(console);
					Commands.Help();

					// Let the application initialize itself
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

						// Update the logical inputs based on the new state of the input system
						inputDevice.Update();

						// Check if any command bindings have been triggered and update the resolution manager
						bindings.Update();
						resolutionManager.Update();

						// Update the application logic
						Update();
						
						// Update the user interface
						var size = new SizeD(window.Width, window.Height);
						_canvas.Measure(size);
						_canvas.Arrange(new RectangleD(0, 0, size));

						// Update the statistics
						statistics.Update(window.Size);
						console.Update(window.Size);

						// React to window size changes
						var viewport = new Rectangle(Vector2i.Zero, window.Size);
						sceneOutput.Viewport = viewport;
						uiOutput.Viewport = viewport;
						camera2D.Viewport = viewport;

						// Draw the current frame
						using (new Measurement(statistics.GraphicsDeviceProfiler))
						using (new Measurement(statistics.CpuFrameTime))
						{
							// Let the application draw the 3D elements of the current frame
							Draw(sceneOutput);

							// Let the application draw the 2D elements of the current frame
							DepthStencilState.DepthDisabled.Bind();
							BlendState.Premultiplied.Bind();
							DrawUserInterface(spriteBatch);
							_canvas.Draw(spriteBatch);

							// Draw the console and the statistics on top of the current frame
							statistics.Draw(spriteBatch);
							console.Draw(spriteBatch);

							spriteBatch.DrawBatch(uiOutput);
						}

						// Present the current frame to the screen and write the log file, if necessary
						swapChain.Present();
						logFile.WriteToFile();

						if (!window.Focused)
							Thread.Sleep(50);
					}

					// The game loop has been exited; time to clean up
					Dispose();
				}
			}
		}

		/// <summary>
		///   Adds the UI element to the context.
		/// </summary>
		/// <param name="element">The element that should be added.</param>
		public void Add(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			Assert.ArgumentSatisfies(element.Parent == null, "The element is already part of a logical tree.");

			_canvas.Children.Add(element);
		}

		/// <summary>
		///   Removes the UI element from the context.
		/// </summary>
		/// <param name="element">The UI element that should be removed.</param>
		public bool Remove(UIElement element)
		{
			Assert.ArgumentNotNull(element);

			if (!_canvas.Children.Remove(element))
				return false;

			return true;
		}
	}
}