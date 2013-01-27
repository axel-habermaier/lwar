using System;

namespace Pegasus.Framework.Processes
{
	/// <summary>
	///   Represents an asynchronous operation that just waits for a given amount of time to elapse.
	/// </summary>
	internal class DelayOperation : PooledObject<DelayOperation>, IAsyncOperation
	{
		/// <summary>
		///   The number of frames to wait before the operation completes.
		/// </summary>
		private int _frameCount;

		/// <summary>
		///   Gets the exception that has been thrown during the execution of the operation.
		/// </summary>
		public Exception Exception
		{
			get { return null; }
		}

		/// <summary>
		///   Gets a value indicating whether the process has completed.
		/// </summary>
		public bool IsCompleted { get; private set; }

		/// <summary>
		///   Updates the state of the asynchronous operation, setting its Exception and IsCompleted properties if the
		///   process has terminated.
		/// </summary>
		public void UpdateState()
		{
			--_frameCount;
			IsCompleted = _frameCount <= 0;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="time">The amount of time in milliseconds that the operation waits before terminating.</param>
		public static DelayOperation Create(int time)
		{
			var operation = GetInstance();
			operation._frameCount = time * App.UpdatesPerSecond / 1000 + 1;
			operation.SetDescription(String.Format("Delaying process for {0}ms.", time));
			operation.IsCompleted = false;
			return operation;
		}
	}
}