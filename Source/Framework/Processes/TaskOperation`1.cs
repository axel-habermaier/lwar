using System;

namespace Pegasus.Framework.Processes
{
	using System.Threading.Tasks;
	using Platform.Memory;

	/// <summary>
	///   Represents an asynchronous operation that waits for the completion of a task.
	/// </summary>
	/// <typeparam name="TResult">The type of the task's result.</typeparam>
	internal class TaskOperation<TResult> : PooledObject<TaskOperation<TResult>>, IAsyncOperation<TResult>
	{
		/// <summary>
		///   The task whose completion is awaited.
		/// </summary>
		private Task<TResult> _task;

		/// <summary>
		///   Gets the exception that has been thrown during the execution of the task.
		/// </summary>
		public Exception Exception
		{
			get { return _task.Exception; }
		}

		/// <summary>
		///   Gets a value indicating whether the task has completed.
		/// </summary>
		public bool IsCompleted { get; private set; }

		/// <summary>
		///   Updates the state of the asynchronous operation, setting its Exception and IsCompleted properties if the
		///   process has terminated.
		/// </summary>
		public void UpdateState()
		{
			// IsCompleted cannot return _task.IsCompleted, as that sometimes leads to an inconsistent state where _task.IsCompleted is
			// false when the process registers the task operation but already true when IsCompleted on the awaiter is called
			// by C#'s async/await state machine
			IsCompleted = _task.IsCompleted;
		}

		/// <summary>
		///   Gets the result produced by the asynchronous operation.
		/// </summary>
		public TResult Result
		{
			get { return _task.Result; }
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="func">The function that should be executed on another thread and whose completion should be awaited.</param>
		public static TaskOperation<TResult> Create(Func<TResult> func)
		{
			Assert.ArgumentNotNull(func, () => func);

			var operation = GetInstance();
			operation._task = Task.Factory.StartNew(func);
			return operation;
		}
	}
}