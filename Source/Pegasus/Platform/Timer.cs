using System;

namespace Pegasus.Platform
{
	using Memory;
	using Scripting;

	/// <summary>
	///   Represents a timer that periodically raises a timeout event.
	/// </summary>
	public sealed class Timer : PooledObject<Timer>
	{
		/// <summary>
		///   The clock that is used to determine when the timeout event should be raised.
		/// </summary>
		private Clock _clock;

		/// <summary>
		///   The timeout in milliseconds after which the timeout event is raised.
		/// </summary>
		private double _timeout;

		/// <summary>
		///   Raised when a timeout occurred.
		/// </summary>
		public event Action Timeout;

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="timeout">The timeout in milliseconds after which the timeout event should be raised.</param>
		/// <param name="scale">Scales the passing of time. If null, time advances in constant steps.</param>
		public static Timer Create(double timeout, Cvar<double> scale = null)
		{
			var timer = GetInstance();
			timer._clock = Clock.Create(scale);
			timer._timeout = timeout;
			return timer;
		}

		/// <summary>
		///   Updates the timer, raising the timeout event if enough time has passed.
		/// </summary>
		public void Update()
		{
			Assert.NotNull(Timeout, "Timeout event is not observed.");

			if (_clock.Milliseconds < _timeout)
				return;

			_clock.Reset();
			Timeout();
		}

		/// <summary>
		///   Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			_clock.SafeDispose();
		}
	}
}