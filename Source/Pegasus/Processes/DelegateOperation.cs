using System;

namespace Pegasus.Processes
{
	using Platform.Memory;

	/// <summary>
	///   Represents an asynchronous operation that waits for a delegate to return true.
	/// </summary>
	internal class DelegateOperation : PooledObject<DelegateOperation>, IAsyncOperation
	{
		/// <summary>
		///   The delegate that is awaited to become true.
		/// </summary>
		private Func<bool> _delegate;

		/// <summary>
		///   Gets the exception that has been thrown during the execution of the task.
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the operation has completed.
		/// </summary>
		public bool IsCompleted { get; private set; }

		/// <summary>
		///   Updates the state of the asynchronous operation, setting its Exception and IsCompleted properties if the
		///   process has terminated.
		/// </summary>
		public void UpdateState()
		{
			try
			{
				IsCompleted = _delegate();
			}
			catch (Exception e)
			{
				Exception = e;
			}
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="func">The function that should be awaited to return true.</param>
		public static DelegateOperation Create(Func<bool> func)
		{
			Assert.ArgumentNotNull(func);

			var operation = GetInstance();
			operation._delegate = func;

			// Check whether the function completes synchronously
			operation.UpdateState();
			return operation;
		}
	}
}