using System;

namespace Pegasus.Framework
{
	using System.Globalization;
	using System.Threading;
	using System.Threading.Tasks;
	using Platform;
	using Platform.Assets;
	using Platform.Graphics;
	using Platform.Input;
	using Rendering;
	using Rendering.UserInterface;
	using Scripting;
	using Scripting.Requests;

	/// <summary>
	///   App base class, invoking the Update and Draw functions whenever appropriate.
	/// </summary>
	public abstract class App : DisposableObject
	{
		/// <summary>
		///   Maximum amount of time (in milliseconds) that might have elapsed between two frames. This is usually exceeded when
		///   the process is paused by the debugger.
		/// </summary>
		private const double MaxElapsedTime = 500;

		/// <summary>
		///   The native platform library instance.
		/// </summary>
		private readonly NativeLibrary _nativeLibrary = new NativeLibrary();

		/// <summary>
		///   The accumulated elapsed time; an update and draw phase is started if the value exceeds the target elapsed time.
		/// </summary>
		private double _accumulatedElapsedTime;

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
		///   Gets or sets the number of updates that should be performed per second. For instance, a value of 60 causes the Update
		///   function to be called exactly 60 times per second, provided that the device the application is run on is fast enough.
		/// </summary>
		public static int UpdatesPerSecond { get; protected set; }

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
		///   Performs fixed time step updates.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		private void DoFixedTimeStepUpdates(GameTime gameTime)
		{
			gameTime.Update();

			// Advance the accumulated elapsed time
			_accumulatedElapsedTime += gameTime.RealElapsedTime;
			var targetElapsedTime = 1000.0 / UpdatesPerSecond;

			// If not enough time has elapsed to perform an update, let the process sleep 
			if (_accumulatedElapsedTime < targetElapsedTime)
			{
				// Calculate the amount of milliseconds to sleep
				var sleepTime = (int)System.Math.Ceiling(targetElapsedTime - _accumulatedElapsedTime);
				Thread.Sleep(sleepTime);

				// Once the process resumes, we're actually going to have to do something
				DoFixedTimeStepUpdates(gameTime);
				return;
			}

			// Do not allow any update to take longer than the maximum
			if (_accumulatedElapsedTime > MaxElapsedTime)
				_accumulatedElapsedTime = MaxElapsedTime;

			var count = (int)(_accumulatedElapsedTime / targetElapsedTime);
			if (count != 1)
				Log.DebugInfo("Performing {0} update passes before drawing the next frame.", count);

			// Perform as many full fixed length time steps as possible
			while (_accumulatedElapsedTime >= targetElapsedTime)
			{
				_accumulatedElapsedTime -= targetElapsedTime;
				gameTime.ElapsedTime = targetElapsedTime;
				gameTime.Time += targetElapsedTime;

				Update();
			}
		}

		/// <summary>
		///   Runs the application. This method does not return until the application is shut down.
		/// </summary>
		public void Run()
		{
			TaskScheduler.UnobservedTaskException += (o, e) => { throw e.Exception.InnerException; };
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			// Keep a history of all logs entries that have been generated before the console can
			// be initialized.
			using (var logHistory = new LogHistory())
			{
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

					// Intialize the statistics
					Statistics = CreateStatistics();
					Statistics.Initialize(GraphicsDevice, font);
					Window.Resized += Statistics.Resize;

					// Copy the recorded log history to the console and stop recording
					console.Copy(logHistory);
					logHistory.StopRecording();

					// Initialize commands and cvars
					console.UserInput += interpreter.Execute;
					Commands.Exit.Invoked += Exit;

					// Keep track of the current game time
					var gameTime = new GameTime();

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

						// Update the application logic using a fixed time step model
						DoFixedTimeStepUpdates(gameTime);

						using (new Measurement(Statistics.GpuFrameTime))
						using (new Measurement(Statistics.CpuFrameTime))
						{
							// Let the application draw the current frame
							Draw();

							// Draw the console and the statistics on top of the current frame
							console.Draw(gameTime);
							Statistics.Draw();
						}

						// Present the current frame to the screen
						SwapChain.Present();
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