using System;

namespace Pegasus.Framework
{
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Input;
	using Rendering;
	using Rendering.UserInterface;
	using Scripting;
	using Scripting.Requests;

	/// <summary>
	///   Represents the application.
	/// </summary>
	public abstract class App : DisposableObject
	{
		/// <summary>
		///   The native platform library instance.
		/// </summary>
		private readonly NativeLibrary _nativeLibrary = new NativeLibrary();

		/// <summary>
		///   Indicates whether the application should continue to run.
		/// </summary>
		private bool _running = true;

		/// <summary>
		///   Gets the graphics device.
		/// </summary>
		protected GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///   Gets the application window.
		/// </summary>
		protected Window Window { get; private set; }

		/// <summary>
		///   Gets the asset manager of the application.
		/// </summary>
		protected AssetsManager Assets { get; private set; }

		/// <summary>
		///   Gets the keyboard that handles all keyboard inputs for the window.
		/// </summary>
		protected Keyboard Keyboard { get; private set; }

		/// <summary>
		///   Gets the mouse that handles all mouse inputs for the window.
		/// </summary>
		protected Mouse Mouse { get; private set; }

		/// <summary>
		///   Gets the logical input device that handles all input.
		/// </summary>
		protected LogicalInputDevice LogicalInputDevice { get; private set; }

		/// <summary>
		///   Gets or sets the statistics manager that is used to measure performance values.
		/// </summary>
		protected Statistics Statistics { get; set; }

		/// <summary>
		///   Gets or sets the default font that is used to draw the console and the statistics.
		/// </summary>
		protected Font DefaultFont { get; set; }

		/// <summary>
		///   Gets or sets a the sprite effect instance that is used to draw the console and the statistics.
		/// </summary>
		protected ISpriteEffectAdaptor SpriteEffect { get; set; }

		/// <summary>
		///   Invoked when the application should update the game state.
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
			Assert.NotDisposed(this);
			_running = false;
		}

		/// <summary>
		///   Invoked when the application is initializing.
		/// </summary>
		protected virtual void Initialize()
		{
		}

		/// <summary>
		///   Runs the application. This method does not return until the application is shut down.
		/// </summary>
		/// <param name="logFile">The log file that writes all generated log entries to the file system.</param>
		internal void Run(LogFile logFile)
		{
			Assert.ArgumentNotNull(logFile, () => logFile);

			// Initialize app window and input
			Window = new Window();
			Keyboard = new Keyboard(Window);
			Mouse = new Mouse(Window);
			LogicalInputDevice = new LogicalInputDevice(Keyboard, Mouse);

			// Initialize graphics and assets manager
			GraphicsDevice = new GraphicsDevice();
			using (var swapChain = new SwapChain(GraphicsDevice, Window))
			using (var interpreter = new Interpreter())
			using (var bindings = new RequestBindings(LogicalInputDevice))
			using (var camera2D = new Camera2D(GraphicsDevice))
			using (var sceneOutput = new RenderOutput(GraphicsDevice) { RenderTarget = swapChain.BackBuffer })
			using (
				var userInterfaceOuput = new RenderOutput(GraphicsDevice) { Camera = camera2D, RenderTarget = swapChain.BackBuffer })
			{
				Assets = new AssetsManager(GraphicsDevice);

				// Run the application-specific initialization logic
				Initialize();
				Assert.NotNull(DefaultFont, "The Initialize() method must set the DefaultFont property.");
				Assert.NotNull(Statistics, "The Initialize() method must set the Statistics property.");
				Assert.NotNull(SpriteEffect, "The Initialize() method must set the SpriteEffect property.");

				using (var spriteBatch = new SpriteBatch(GraphicsDevice, userInterfaceOuput, SpriteEffect))
				using (var console = new Console(GraphicsDevice, LogicalInputDevice, spriteBatch, DefaultFont))
				{
					Statistics.Initialize(GraphicsDevice, spriteBatch, DefaultFont);

					// Initialize commands and cvars
					console.UserInput += interpreter.Execute;
					Commands.Exit.Invoked += Exit;

					// Ensure that the size of the console and the statistics always matches that of the window
					console.Resize(Window.Size);
					Statistics.Resize(Window.Size);

					Window.Resized += console.Resize;
					Window.Resized += Statistics.Resize;

					// Copy the recorded log history to the console
					logFile.WriteToConsole(console);

					while (_running)
					{
						// Update the input system and let the console respond to any input
						using (new Measurement(Statistics.UpdateInput))
						{
							UpdateInput();
							console.HandleInput();
						}

						// Check if any command bindings have been triggered
						bindings.InvokeTriggeredBindings();

						// Update the application logic 
						Update();

						var viewport = new Rectangle(Vector2i.Zero, Window.Size);
						sceneOutput.Viewport = viewport;
						userInterfaceOuput.Viewport = viewport;

						Statistics.Update();

						using (new Measurement(Statistics.GpuFrameTime))
						using (new Measurement(Statistics.CpuFrameTime))
						{
							// Let the application draw the current frame
							Draw(sceneOutput);
							DrawUserInterface(spriteBatch);

							// Draw the console and the statistics on top of the current frame
							console.Draw();
							Statistics.Draw();
						}

						// Present the current frame to the screen and write the log file, if necessary
						swapChain.Present();
						logFile.WriteToFile();
					}
				}
			}
		}

		/// <summary>
		///   Updates the input devices.
		/// </summary>
		private void UpdateInput()
		{
			// Update the keyboard and mouse state first (this ensures that WentDown returns 
			// false for all keys and buttons, etc.)
			Keyboard.Update();
			Mouse.Update();

			// Process all new input; this might set WentDown, etc. to true for some keys and buttons
			Window.ProcessEvents();

			// Update the logical inputs based on the new state of the input system
			LogicalInputDevice.Update();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			SpriteEffect.SafeDispose();
			Statistics.SafeDispose();
			Keyboard.SafeDispose();
			Mouse.SafeDispose();
			Assets.SafeDispose();
			GraphicsDevice.SafeDispose();
			Window.SafeDispose();
			_nativeLibrary.SafeDispose();
		}
	}
}