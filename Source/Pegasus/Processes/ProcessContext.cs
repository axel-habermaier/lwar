using System;

namespace Pegasus.Framework.Processes
{
	/// <summary>
	///   Represents the context of a process that can be by the asynchronous methods that the process executes to check
	///   whether cancellation has been requested and to set resume conditions for the process.
	/// </summary>
	public struct ProcessContext
	{
		/// <summary>
		///   The process that the context belongs to.
		/// </summary>
		private readonly Process _process;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="process">The process that the context belongs to.</param>
		internal ProcessContext(Process process)
		{
			_process = process;
		}

		/// <summary>
		///   Gets a value indicating whether cancellation has been requested and the process should abort the execution as quickly
		///   as possible.
		/// </summary>
		public bool IsCanceled
		{
			get { return _process.IsCanceled; }
		}

		/// <summary>
		///   Instructs the process to wait for the completion of the given asynchronous operation.
		/// </summary>
		/// <param name="asyncOperation">The asynchronous operation that the process should wait for.</param>
		public Awaiter WaitFor(IAsyncOperation asyncOperation)
		{
			Assert.ArgumentNotNull(asyncOperation);

			_process.WaitFor(asyncOperation);
			return new Awaiter(_process, asyncOperation);
		}

		/// <summary>
		///   Instructs the process to wait for the completion of the given asynchronous operation.
		/// </summary>
		/// <typeparam name="TResult">The type of the operation's result.</typeparam>
		/// <param name="asyncOperation">The asynchronous operation that the process should wait for.</param>
		public Awaiter<TResult> WaitFor<TResult>(IAsyncOperation<TResult> asyncOperation)
		{
			Assert.ArgumentNotNull(asyncOperation);

			_process.WaitFor(asyncOperation);
			return new Awaiter<TResult>(_process, asyncOperation);
		}

		/// <summary>
		///   Instructs the process to wait for the completion of the given action that will be executed on another thread.
		/// </summary>
		/// <param name="action">The action that the process should wait for.</param>
		public Awaiter WaitForTask(Action action)
		{
			Assert.ArgumentNotNull(action);
			return WaitFor(TaskOperation.Create(action));
		}

		/// <summary>
		///   Instructs the process to wait for the completion of the given function that will be executed on another thread.
		/// </summary>
		/// <typeparam name="TResult">The type of the operation's result.</typeparam>
		/// <param name="func">The function that the process should wait for.</param>
		public Awaiter<TResult> WaitForTask<TResult>(Func<TResult> func)
		{
			Assert.ArgumentNotNull(func);
			return WaitFor(TaskOperation<TResult>.Create(func));
		}

		/// <summary>
		///   Instructs the process to wait for the given function to return true.
		/// </summary>
		/// <param name="func">The function that the process should wait for.</param>
		public Awaiter WaitFor(Func<bool> func)
		{
			Assert.ArgumentNotNull(func);
			return WaitFor(DelegateOperation.Create(func));
		}

		/// <summary>
		///   Delays the execution of the process by the given amount of time.
		/// </summary>
		/// <param name="time">The amount of time in milliseconds that the process should wait before continuing.</param>
		public Awaiter Delay(double time)
		{
			return WaitFor(DelayOperation.Create(time));
		}

		/// <summary>
		///   Delays the execution of the process until the next frame.
		/// </summary>
		public Awaiter NextFrame()
		{
			return WaitFor(DelayOperation.Create(0));
		}
	}
}