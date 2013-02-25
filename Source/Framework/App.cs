using System;

namespace Pegasus.Framework
{
	using Platform;
	using Platform.Assets;
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
		///   Gets the swap chain that is used for rendering to the window.
		/// </summary>
		protected SwapChain SwapChain { get; private set; }

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
		///   Gets the statistics manager that is used to measure performance values.
		/// </summary>
		protected Statistics Statistics { get; private set; }

		/// <summary>
		///   Invoked when the application should update the game state.
		/// </summary>
		protected abstract void Update();

		/// <summary>
		///   Invoked when the application should draw a frame.
		/// </summary>
		protected abstract void Draw();

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
		///   Creates the statistics manager that is used to measure performance values.
		/// </summary>
		protected virtual Statistics CreateStatistics()
		{
			return new Statistics();
		}

		/// <summary>
		///   Runs the application. This method does not return until the application is shut down.
		/// </summary>
		/// <param name="logFile">The log file that writes all generated log entries to the file system.</param>
		internal void Run(LogFile logFile)
		{
			Assert.ArgumentNotNull(logFile, () => logFile);

			Log.Info("Starting {0}, version {1}.{2}.", Cvars.AppName.Value, Cvars.AppVersionMajor.Value, Cvars.AppVersionMinor.Value);
			Log.Info("Running on {0} {1}bit, using {2}.", PlatformInfo.Platform, IntPtr.Size == 4 ? "32" : "64",
					 PlatformInfo.GraphicsApi);

			// Initialize app window and input
			Window = new Window();
			Keyboard = new Keyboard(Window);
			Mouse = new Mouse(Window);
			LogicalInputDevice = new LogicalInputDevice(Keyboard, Mouse);

			// Initialize graphics and assets manager
			GraphicsDevice = new GraphicsDevice();
			SwapChain = new SwapChain(GraphicsDevice, Window);
			Assets = new AssetsManager(GraphicsDevice);

#if DEBUG
			Commands.ReloadAssets.Invoke();
#endif

			SpriteBatch.LoadShaders(Assets);
			SwapChain.BackBuffer.Bind();

			var font = Assets.LoadFont("Fonts/Liberation Mono 12");
			using (var interpreter = new Interpreter())
			using (var bindings = new RequestBindings(LogicalInputDevice))
			using (var console = new Console(GraphicsDevice, font, LogicalInputDevice))
			{
				// Ensure that the size of the console always matches that of the window
				console.Resize(Window.Size);
				Window.Resized += console.Resize;

				// Copy the recorded log history to the console and initialize the statistics
				logFile.WriteToConsole(console);
				Statistics = CreateStatistics();
				Statistics.Initialize(GraphicsDevice, font);
				Window.Resized += Statistics.Resize;

				// Initialize commands and cvars
				console.UserInput += interpreter.Execute;
				Commands.Exit.Invoked += Exit;

				// Run the application-specific initialization logic
				Initialize();

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
					Statistics.Update();

					using (new Measurement(Statistics.GpuFrameTime))
					using (new Measurement(Statistics.CpuFrameTime))
					{
						// Let the application draw the current frame
						Draw();

						// Draw the console and the statistics on top of the current frame
						console.Draw();
						Statistics.Draw();
					}

					// Present the current frame to the screen
					SwapChain.Present();
					logFile.WriteToFile();
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
			Statistics.SafeDispose();
			Keyboard.SafeDispose();
			Mouse.SafeDispose();
			Assets.SafeDispose();
			SwapChain.SafeDispose();
			GraphicsDevice.SafeDispose();
			Window.SafeDispose();
			_nativeLibrary.SafeDispose();
		}
	}
}