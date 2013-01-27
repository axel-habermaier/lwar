using System;

namespace Pegasus.Framework.Processes
{
	using System.Threading.Tasks;

	/// <summary>
	///   Represents an asynchronous process.
	/// </summary>
	/// <typeparam name="TResult">The type of the result of the process.</typeparam>
	internal class Process<TResult> : Process, IProcess<TResult>
	{
		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="scheduler">The scheduler that manages the process.</param>
		/// <param name="asyncFunc">The asynchronous function that the process should execute.</param>
		public static Process<TResult> Create(ProcessScheduler scheduler, AsyncFunc<TResult> asyncFunc)
		{
			Assert.ArgumentNotNull(scheduler, () => scheduler);
			Assert.ArgumentNotNull(asyncFunc, () => asyncFunc);

			var process = new Process<TResult>();
			process.Task = asyncFunc(process.Context);
			return process;
		}

		/// <summary>
		///   Gets the result of the process if the execution of the process is complete.
		/// </summary>
		public TResult Result
		{
			get { return ((Task<TResult>)Task).Result; }
		}
	}
}