namespace Pegasus.Platform
{
	using System;
	using Scripting;
	using Utilities;

	/// <summary>
	///     Represents a timer that periodically raises a timeout event.
	/// </summary>
	public struct Timer
	{
		/// <summary>
		///     The timeout in milliseconds after which the timeout event is raised.
		/// </summary>
		private readonly double _timeout;

		/// <summary>
		///     The clock that is used to determine when the timeout event should be raised.
		/// </summary>
		private Clock _clock;

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="timeout">The timeout in milliseconds after which the timeout event should be raised.</param>
		/// <param name="scale">Scales the passing of time. If null, time advances in constant steps.</param>
		public Timer(double timeout, Cvar<double> scale = null)
			: this()
		{
			_clock = new Clock(scale);
			_timeout = timeout;
		}

		/// <summary>
		///     Raised when a timeout occurred.
		/// </summary>
		public event Action Timeout;

		/// <summary>
		///     Updates the timer, raising the timeout event if enough time has passed.
		/// </summary>
		public void Update()
		{
			Assert.NotNull(Timeout, "Timeout event is not observed.");

			if (_clock.Milliseconds < _timeout)
				return;

			_clock.Reset();
			Timeout();
		}
	}
}