namespace Pegasus.Processes
{
	using System;
	using Platform;
	using Platform.Memory;

	/// <summary>
	///     Represents an asynchronous operation that just waits for a given amount of time to elapse.
	/// </summary>
	internal class DelayOperation : PooledObject<DelayOperation>, IAsyncOperation
	{
		/// <summary>
		///     The clock that provides time measurements.
		/// </summary>
		private Clock _clock;

		/// <summary>
		///     The amount of time in milliseconds that the operation waits before terminating.
		/// </summary>
		private double _time;

		/// <summary>
		///     Gets the exception that has been thrown during the execution of the operation.
		/// </summary>
		public Exception Exception
		{
			get { return null; }
		}

		/// <summary>
		///     Gets a value indicating whether the process has completed.
		/// </summary>
		public bool IsCompleted { get; private set; }

		/// <summary>
		///     Updates the state of the asynchronous operation, setting its Exception and IsCompleted properties if the
		///     process has terminated.
		/// </summary>
		public void UpdateState()
		{
			IsCompleted = _clock.Milliseconds >= _time;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="time">The amount of time in milliseconds that the operation waits before terminating.</param>
		public static DelayOperation Create(double time)
		{
			var operation = GetInstance();
			operation._clock = new Clock();
			operation._time = time;
			operation.SetDescription(String.Format("Delaying process for {0}ms.", time));
			operation.IsCompleted = false;
			return operation;
		}
	}
}