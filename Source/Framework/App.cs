using System;

namespace Pegasus.Framework
{
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Input;
	using Platform.Logging;
	using Rendering;
	using Rendering.UserInterface;
	using Scripting;

	/// <summary>
	///   Represents the application.
	/// </summary>
	public abstract class App
	{
		/// <summary>
		///   Indicates whether the application should continue to run.
		/// </summary>
		private bool _running = true;

		/// <summary>
		///   Gets the context of the application, providing access to all framework objects that can be used by the application.
		/// </summary>
		protected IAppContext Context { get; private set; }

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
		/// <param name="context">
		///   The context of the application, providing access to all framework objects that can be used by the application.
		/// </param>
		/// <param name="logFile">The log file that writes all generated log entries to the file system.</param>
		internal void Run(AppContext context, LogFile logFile)
		{
			Assert.ArgumentNotNull(context);
			Assert.ArgumentNotNull(logFile);

			using (new NativeLibrary())
			using (var window = context.Window = new Window())
			using (var graphicsDevice = context.GraphicsDevice = new GraphicsDevice())
			using (var statistics = context.Statistics)
			using (var spriteEffect = context.SpriteEffect)
			using (var swapChain = new SwapChain(graphicsDevice, window))
			using (var assets = context.Assets = new AssetsManager(graphicsDevice))
			using (var keyboard = new Keyboard(window))
			using (var mouse = new Mouse(window))
			using (var inputDevice =context.LogicalInputDevice = new LogicalInputDevice(keyboard, mouse))
			using (var bindings = new Bindings(inputDevice, context.Commands, context.Cvars))
			using (var camera2D = new Camera2D(graphicsDevice))
			using (var sceneOutput = new RenderOutput(graphicsDevice) { RenderTarget = swapChain.BackBuffer })
			using (var uiOutput = new RenderOutput(graphicsDevice) { Camera = camera2D, RenderTarget = swapChain.BackBuffer })
			{
				window.Title = context.AppName;
				swapChain.BackBuffer.SetName("Back Buffer");
				spriteEffect.Initialize(context.GraphicsDevice, context.Assets);

				context.Commands.OnExit += Exit;
				Context = context;
				Initialize();

				var defaultFont = assets.LoadFont(context.DefaultFontName);
				using (var spriteBatch = new SpriteBatch(graphicsDevice, uiOutput, spriteEffect))
				using (var console = new Console(graphicsDevice, inputDevice, spriteBatch, defaultFont,
												 context.Commands, context.Cvars))
				{
					statistics.Initialize(graphicsDevice, spriteBatch, defaultFont);

					// Ensure that the size of the console and the statistics always matches that of the window
					console.Resize(window.Size);
					statistics.Resize(window.Size);

					window.Resized += console.Resize;
					window.Resized += statistics.Resize;
					context.Commands.OnReloadAssets += assets.ReloadAssets;

					// Copy the recorded log history to the console and explain the usage of the console
					logFile.WriteToConsole(console);
					context.Commands.Help();

					while (_running)
					{
						// Update the input system and let the console respond to any input
						using (new Measurement(statistics.UpdateInput))
						{
							// Update the keyboard and mouse state first (this ensures that WentDown returns 
							// false for all keys and buttons, etc.)
							inputDevice.Keyboard.Update();
							inputDevice.Mouse.Update();

							// Process all new input; this might set WentDown, etc. to true for some keys and buttons
							window.ProcessEvents();

							// Update the logical inputs based on the new state of the input system
							inputDevice.Update();
							console.HandleInput();
						}

						// Check if any command bindings have been triggered
						bindings.Update();

						// Update the application logic and the statistics
						Update();
						statistics.Update();

						// React to window size changes
						var viewport = new Rectangle(Vector2i.Zero, window.Size);
						sceneOutput.Viewport = viewport;
						uiOutput.Viewport = viewport;
						camera2D.Viewport = viewport;

						// Draw the current frame
						using (new Measurement(statistics.GpuFrameTime))
						using (new Measurement(statistics.CpuFrameTime))
						{
							// Let the application draw the current frame
							Draw(sceneOutput);
							DrawUserInterface(spriteBatch);

							// Draw the console and the statistics on top of the current frame
							console.Draw();
							statistics.Draw();
						}

						// Present the current frame to the screen and write the log file, if necessary
						swapChain.Present();
						logFile.WriteToFile();
					}

					// The game loop has been exited; time to clean up
					Dispose();
				}
			}
		}
	}
}