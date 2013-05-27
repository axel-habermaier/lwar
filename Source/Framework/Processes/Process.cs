using System;

namespace Pegasus.Framework.Processes
{
	using System.Runtime.ExceptionServices;
	using System.Threading.Tasks;
	using Platform.Memory;

	/// <summary>
	///   Represents an asynchronous process.
	/// </summary>
	/// <remarks>
	///   Process are not pooled as the process schedulers typically hold a reference to scheduled processes even after
	///   they have been disposed. For pooled processes, it would be hard to determine correctly when a process instance
	///   can safely be returned to the pool.
	/// </remarks>
	internal sealed class Process : /* PooledObject<Process> */ DisposableObject, IResumableProcess
	{
		/// <summary>
		///   Gets the asynchronous operation that the process is currently waiting for.
		/// </summary>
		private IAsyncOperation _asyncOperation;

		/// <summary>
		///   The task returned by the asynchronous method that the process represents.
		/// </summary>
		private Task _task;

		/// <summary>
		///   Gets or sets the task returned by the asynchronous method that the process represents.
		/// </summary>
		internal Task Task
		{
			get { return _task; }
			set
			{
				_task = value;
				if (_task.IsCompleted || _task.IsFaulted)
					RethrowException();
			}
		}

		/// <summary>
		///   Sets the continuation that should be invoked when the process is resumed after waiting for an asynchronous operation.
		/// </summary>
		internal Action Continuation { private get; set; }

		/// <summary>
		///   Gets the context for the process.
		/// </summary>
		internal ProcessContext Context
		{
			get { return new ProcessContext(this); }
		}

		/// <summary>
		///   Gets a value indicating whether the process has completed because of an unhandled exception.
		/// </summary>
		public bool IsFaulted
		{
			get { return Task.IsFaulted; }
		}

		/// <summary>
		///   Gets a value indicating whether the execution of the process has been canceled before it has been completed, either
		///   by calling the Cancel() function or by the process throwing an exception.
		/// </summary>
		public bool IsCanceled { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the execution of the process has completed without being canceled.
		/// </summary>
		public bool IsCompleted
		{
			get { return Task.IsCompleted; }
		}

		/// <summary>
		///   Immediately cancels the execution of the process, provided that the process is still being executed.
		/// </summary>
		public void Cancel()
		{
			if (IsCompleted || IsCanceled || IsFaulted)
				return;

			IsCanceled = true;

			if (Continuation != null)
				Continuation();

			_asyncOperation.SafeDispose();
			_asyncOperation = null;
			RethrowException();
		}

		/// <summary>
		///   Resumes the process if the asynchronous operation the process is currently waiting for has completed executing.
		/// </summary>
		public void Resume()
		{
			if (IsFaulted)
				RethrowException();

			if (IsCompleted || IsCanceled)
				return;

			Assert.NotNull(_asyncOperation);

			_asyncOperation.UpdateState();
			if (!_asyncOperation.IsCompleted)
				return;

			using (_asyncOperation)
			{
				_asyncOperation = null;

				if (Continuation != null)
					Continuation();
			}

			if (_task.IsCompleted || _task.IsFaulted)
				RethrowException();
		}

		/// <summary>
		///   Runs the process.
		/// </summary>
		/// <param name="asyncAction">The asynchronous action that the process should execute.</param>
		internal void Run(AsyncAction asyncAction)
		{
			Assert.ArgumentNotNull(asyncAction, () => asyncAction);
			Task = asyncAction(Context);
		}

		/// <summary>
		///   Instructs the process to wait for the completion of the given asynchronous operation.
		/// </summary>
		/// <param name="asyncOperation">The asynchronous operation that the process should wait for.</param>
		internal void WaitFor(IAsyncOperation asyncOperation)
		{
			Assert.ArgumentNotNull(asyncOperation, () => asyncOperation);
			Assert.That(_asyncOperation == null, "The process is already waiting for an asynchronous operation.");

			if (asyncOperation.IsCompleted)
			{
				asyncOperation.SafeDispose();
				return;
			}

			_asyncOperation = asyncOperation;
		}

		/// <summary>
		///   Rethrows the exception that occurred during the execution of the task, if any.
		/// </summary>
		private void RethrowException()
		{
			if (Task.Exception == null)
				return;

			var exceptions = Task.Exception.InnerExceptions;
			if (exceptions.Count == 1 && exceptions[0] is OperationCanceledException)
				return;

			ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Cancel();
			_task.Dispose();
		}
	}
}