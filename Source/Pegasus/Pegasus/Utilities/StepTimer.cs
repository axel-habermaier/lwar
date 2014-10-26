namespace Pegasus.Utilities
{
	using System;
	using Platform;

	/// <summary>
	///     Provides time measurements and raises the update event whenever necessary. In variable
	///     timestep mode, the update event is raised as often as possible, i.e., once with every call to
	///     Tick(). In fixed timestep mode, the timer attempts to raise the update event approximately at
	///     intervals of the given target elapsed time. Therefore, if the application is calling Tick() faster
	///     than the target elapsed time, not every call to Tick() raises the update event. On the other hand,
	///     if calls to Tick() are infrequent, the update event might be raised several times to catch up.
	///     However, MaxDelta limits the maximum elapsed time between two calls to Tick(), as unreasonably long
	///     pauses are usually due to the app being inactive for some time or due to the debugger pausing the app.
	/// </summary>
	public class StepTimer
	{
		/// <summary>
		///     The maximum time delta in seconds that is allowed between two consecutive calls to Update.
		/// </summary>
		private const double MaxDelta = 0.1;

		/// <summary>
		///     The time of the last update.
		/// </summary>
		private double _lastTime;

		/// <summary>
		///     The time left over from the last frame.
		/// </summary>
		private double _leftOverTime;

		/// <summary>
		///     Used to compute the framerate.
		/// </summary>
		private double _secondCounter;

		/// <summary>
		///     The number of updates this second.
		/// </summary>
		private uint _updatesThisSecond;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public StepTimer()
		{
			TargetElapsedSeconds = 1.0 / 60.0;
			ResetElapsedTime();
		}

		/// <summary>
		///     Gets the elapsed time in seconds since the previous update.
		/// </summary>
		public double ElapsedSeconds { get; private set; }

		/// <summary>
		///     Gets the elapsed time in seconds since the creation of the timer.
		/// </summary>
		public double TotalSeconds { get; private set; }

		/// <summary>
		///     Gets the total number of updates since the creation of the timer.
		/// </summary>
		public uint TotalUpdateCount { get; private set; }

		/// <summary>
		///     Gets the current number of updates per second, updated once per second.
		/// </summary>
		public uint UpdatesPerSecond { get; private set; }

		/// <summary>
		///     Gets or sets a value indicating whether the fixed or the variable timestep mode should be used.
		/// </summary>
		public bool IsFixedTimeStep { get; set; }

		/// <summary>
		///     Gets or sets the target elapsed time between two consecutive updates in fixed timestep mode.
		/// </summary>
		public double TargetElapsedSeconds { get; set; }

		/// <summary>
		///     Resets all left-over time in fixed timestep mode, preventing the fixed timestep logic from attempting a set of catch-up
		///     updates.
		/// </summary>
		public void ResetElapsedTime()
		{
			_lastTime = Clock.SystemTime;
			_leftOverTime = 0;
			UpdatesPerSecond = 0;
			_updatesThisSecond = 0;
			_secondCounter = 0;
		}

		/// <summary>
		///     Raised when the step timer determined that an update is required.
		/// </summary>
		public event Action UpdateRequired;

		/// <summary>
		///     Updates the timer state, raising the update event the appropriate number of times.
		/// </summary>
		public void Update()
		{
			Assert.That(UpdateRequired != null, "UpdateRequired event has no listeners.");

			var currentTime = Clock.SystemTime;
			var timeDelta = currentTime - _lastTime;

			_lastTime = currentTime;
			_secondCounter += timeDelta;

			// Clamp large time deltas, as the application might have been paused in the debugger, etc.
			if (timeDelta > MaxDelta)
				timeDelta = MaxDelta;

			var lastUpdateCount = TotalUpdateCount;

			if (IsFixedTimeStep)
			{
				// If the app is running very close to the target elapsed time (within 1/4 of a millisecond) just clamp
				// the clock to exactly match the target value. This prevents tiny and irrelevant errors
				// from accumulating over time. Without this clamping, a game that requested a 60 fps
				// fixed update, running with vsync enabled on a 59.94 NTSC display, would eventually
				// accumulate enough tiny errors that it would drop a frame. It is better to just round 
				// small deviations down to zero to leave things running smoothly.
				if (Math.Abs(timeDelta - TargetElapsedSeconds) < 1.0 / 4000.0)
					timeDelta = TargetElapsedSeconds;

				_leftOverTime += timeDelta;

				while (_leftOverTime >= TargetElapsedSeconds)
				{
					ElapsedSeconds = TargetElapsedSeconds;
					TotalSeconds += TargetElapsedSeconds;
					_leftOverTime -= TargetElapsedSeconds;
					++TotalUpdateCount;

					UpdateRequired();
				}
			}
			else
			{
				ElapsedSeconds = timeDelta;
				TotalSeconds += timeDelta;
				_leftOverTime = 0;
				++TotalUpdateCount;

				UpdateRequired();
			}

			// If we've performed at least one update, add another update to the current second.
			// The update framerate discards catch-up updates.
			if (TotalUpdateCount != lastUpdateCount)
				++_updatesThisSecond;

			// Reset the second counter if it exceeds one second (the time delta never exceeds MaxDelta seconds).
			if (!(_secondCounter >= 1.0))
				return;

			UpdatesPerSecond = _updatesThisSecond;
			_updatesThisSecond = 0;
			_secondCounter -= 1.0;
		}
	}
}