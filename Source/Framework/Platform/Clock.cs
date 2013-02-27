using System;

namespace Pegasus.Framework.Platform
{
	using System.Runtime.InteropServices;
	using Scripting;

	/// <summary>
	///   Represents a clock.
	/// </summary>
	public sealed class Clock : PooledObject<Clock>
	{
		/// <summary>
		///   Indicates whether the clock is affected by the current time scale factor.
		/// </summary>
		private bool _allowScaling;

		/// <summary>
		///   The offset that is applied to all times returned by this instance.
		/// </summary>
		private double _offset;

		/// <summary>
		///   The current time in seconds.
		/// </summary>
		private double _time;

		/// <summary>
		///   Gets the unmodified system time in seconds.
		/// </summary>
		public static double SystemTime
		{
			get { return NativeMethods.GetTime(); }
		}

		/// <summary>
		///   Gets the current time in seconds.
		/// </summary>
		public double Seconds
		{
			get
			{
				Update();
				return _time;
			}
		}

		/// <summary>
		///   Gets the current time in milliseconds.
		/// </summary>
		public double Milliseconds
		{
			get { return Seconds * 1000; }
		}

		/// <summary>
		///   Invoked when the time scale factor is about to be changed.
		/// </summary>
		/// <param name="factor">The new time scale factor.</param>
		private void TimeScaleChanging(double factor)
		{
			// Update the current time with the old factor
			Update();
		}

		/// <summary>
		///   Resets the clock to zero.
		/// </summary>
		public void Reset()
		{
			_offset = SystemTime;
			_time = 0;
		}

		/// <summary>
		///   Updates the internal time.
		/// </summary>
		private void Update()
		{
			// Get the elapsed system time since the last update
			var systemTime = SystemTime;
			var elapsedTime = systemTime - _offset;
			_offset = systemTime;

			// Scale the elapsedTime with the current scaling factor and add it to the internal time
			var scale = _allowScaling ? Cvars.TimeScaleFactor.Value : 1;
			_time += elapsedTime * scale;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="allowScaling">Indicates whether the clock should be affected by the current time scale factor.</param>
		public static Clock Create(bool allowScaling = false)
		{
			var clock = GetInstance();
			clock._time = 0;
			clock._offset = SystemTime;
			clock._allowScaling = allowScaling;

			if (allowScaling)
				Cvars.TimeScaleFactor.Changing += clock.TimeScaleChanging;

			return clock;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnReturning()
		{
			if (_allowScaling)
				Cvars.TimeScaleFactor.Changing -= TimeScaleChanging;
		}

		/// <summary>
		///   Provides access to the native function.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetTime")]
			public static extern double GetTime();
		}
	}
}