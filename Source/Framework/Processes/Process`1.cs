using System;

namespace Pegasus.Framework.Processes
{
	using System.Threading.Tasks;
	using Platform.Memory;

	/// <summary>
	///   Represents a wrapper around an asynchronous process that in additional to the regular process behavior also returns a
	///   result.
	/// </summary>
	/// <typeparam name="TResult">The type of the result of the process.</typeparam>
	internal class Process<TResult> : DisposableObject, IProcess<TResult>, IResumableProcess
	{
		/// <summary>
		///   The actual process that is being executed.
		/// </summary>
		private readonly Process _process = new Process();

		/// <summary>
		///   Gets the result of the process if the execution of the process is complete.
		/// </summary>
		public TResult Result
		{
			get { return ((Task<TResult>)_process.Task).Result; }
		}

		/// <summary>
		///   Gets a value indicating whether the execution of the process has been canceled before it has been completed.
		/// </summary>
		public bool IsCanceled
		{
			get { return _process.IsCanceled; }
		}

		/// <summary>
		///   Gets a value indicating whether the execution of the process has completed without being canceled.
		/// </summary>
		public bool IsCompleted
		{
			get { return _process.IsCompleted; }
		}

		/// <summary>
		///   Gets a value indicating whether the process has completed because of an unhandled exception.
		/// </summary>
		public bool IsFaulted
		{
			get { return _process.IsFaulted; }
		}

		/// <summary>
		///   Immediately cancels the execution of the process, provided that the process is still being executed.
		/// </summary>
		public void Cancel()
		{
			_process.Cancel();
		}

		/// <summary>
		///   Resumes the process if the asynchronous operation the process is currently waiting for has completed executing.
		/// </summary>
		public void Resume()
		{
			_process.Resume();
		}

		/// <summary>
		///   Runs the process.
		/// </summary>
		/// <param name="asyncFunc">The asynchronous function that the process should execute.</param>
		internal void Run(AsyncFunc<TResult> asyncFunc)
		{
			Assert.ArgumentNotNull(asyncFunc);
			_process.Task = asyncFunc(_process.Context);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_process.SafeDispose();
		}
	}
}