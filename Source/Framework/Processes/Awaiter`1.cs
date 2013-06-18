using System;

namespace Pegasus.Framework.Processes
{
	using System.Runtime.CompilerServices;

	/// <summary>
	///   Represents an awaiter that can be used in conjunction with C#'s async/await feature.
	/// </summary>
	/// <typeparam name="TResult">The type of the operation's result.</typeparam>
	public struct Awaiter<TResult> : INotifyCompletion
	{
		/// <summary>
		///   The asynchronous operation that the awaiter awaits.
		/// </summary>
		private readonly IAsyncOperation<TResult> _asyncOperation;

		/// <summary>
		///   The process that is waiting for the asynchronous operation to complete.
		/// </summary>
		private readonly Process _process;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="process">The process that is waiting for the asynchronous operation to complete.</param>
		/// <param name="asyncOperation">he asynchronous operation that the awaiter should await.</param>
		internal Awaiter(Process process, IAsyncOperation<TResult> asyncOperation)
		{
			_process = process;
			_asyncOperation = asyncOperation;
		}

		/// <summary>
		///   Gets a value indicating whether the asynchronous operation has been completed.
		/// </summary>
		public bool IsCompleted
		{
			get { return _asyncOperation.IsCompleted; }
		}

		/// <summary>
		///   Schedules the continuation action that is invoked when the asynchronous operation completes.
		/// </summary>
		/// <param name="continuation">The action to invoke when the operation completes.</param>
		public void OnCompleted(Action continuation)
		{
			Assert.ArgumentNotNull(continuation);
			_process.Continuation = continuation;
		}

		/// <summary>
		///   Invoked by the C# compiler when an asynchronous method resumes after an await to retrieve its result. Ensures that
		///   exceptions thrown by asynchronous operations are propagated properly. Additionally, if the process has been canceled,
		///   an OperationCanceledException is thrown to ensure that the continuation stops executing immediately.
		/// </summary>
		public TResult GetResult()
		{
			if (_asyncOperation.Exception != null)
				throw _asyncOperation.Exception;

			if (_process.IsCanceled)
				throw new OperationCanceledException();

			return _asyncOperation.Result;
		}

		/// <summary>
		///   Invoked by the C# compiler to get the awaiter for the instance. Here, the awaiter is the instance itself.
		/// </summary>
		public Awaiter<TResult> GetAwaiter()
		{
			return this;
		}
	}
}