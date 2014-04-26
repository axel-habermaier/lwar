namespace Pegasus.Platform
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Memory;
	using Scripting;

	/// <summary>
	///     Represents a clock.
	/// </summary>
	public sealed class Clock : PooledObject<Clock>
	{
		/// <summary>
		///     The offset that is applied to all times returned by this instance.
		/// </summary>
		private double _offset;

		/// <summary>
		///     Scales the passing of time. If null, time advances in constant steps.
		/// </summary>
		private Cvar<double> _scale;

		/// <summary>
		///     The current time in seconds.
		/// </summary>
		private double _time;

		/// <summary>
		///     Gets the unmodified system time in seconds.
		/// </summary>
		public static double SystemTime
		{
			get { return NativeMethods.GetTime(); }
		}

		/// <summary>
		///     Gets the current time in seconds.
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
		///     Gets the current time in milliseconds.
		/// </summary>
		public double Milliseconds
		{
			get { return Seconds * 1000; }
		}

		/// <summary>
		///     Invoked when the time scale factor is about to be changed.
		/// </summary>
		/// <param name="factor">The new time scale factor.</param>
		private void TimeScaleChanging(double factor)
		{
			// Update the current time with the old factor
			Update();
		}

		/// <summary>
		///     Resets the clock to zero.
		/// </summary>
		public void Reset()
		{
			_offset = SystemTime;
			_time = 0;
		}

		/// <summary>
		///     Updates the internal time.
		/// </summary>
		private void Update()
		{
			// Get the elapsed system time since the last update
			var systemTime = SystemTime;
			var elapsedTime = systemTime - _offset;
			_offset = systemTime;

			// Scale the elapsedTime with the current scaling factor and add it to the internal time
			var scale = _scale == null ? 1 : _scale.Value;
			_time += elapsedTime * scale;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="scale">Scales the passing of time. If null, time advances in constant steps.</param>
		public static Clock Create(Cvar<double> scale = null)
		{
			var clock = GetInstance();
			clock._time = 0;
			clock._offset = SystemTime;
			clock._scale = scale;

			if (scale != null)
				scale.Changing += clock.TimeScaleChanging;

			return clock;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnReturning()
		{
			if (_scale != null)
				_scale.Changing -= TimeScaleChanging;
		}

		/// <summary>
		///     Provides access to the native function.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetTime")]
			public static extern double GetTime();
		}
	}
}