using System;

namespace Pegasus.Framework.Processes
{
	using System.Threading.Tasks;

	/// <summary>
	///   Represents an asynchronous operation that waits for the completion of a task.
	/// </summary>
	internal class TaskOperation : PooledObject<TaskOperation>, IAsyncOperation
	{
		/// <summary>
		///   The task whose completion is awaited.
		/// </summary>
		private Task _task;

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
		public bool IsCompleted
		{
			get { return _task.IsCompleted; }
		}

		/// <summary>
		///   Updates the state of the asynchronous operation, setting its Exception and IsCompleted properties if the
		///   process has terminated.
		/// </summary>
		public void UpdateState()
		{
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="action">The action that should be executed on another thread and whose completion should be awaited.</param>
		public static TaskOperation Create(Action action)
		{
			Assert.ArgumentNotNull(action, () => action);

			var operation = GetInstance();
			operation._task = Task.Factory.StartNew(action);
			return operation;
		}
	}
}