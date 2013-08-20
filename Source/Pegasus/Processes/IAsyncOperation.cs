using System;

namespace Pegasus.Processes
{
	/// <summary>
	///   Represents an asynchronous operation that can block the execution of an asynchronous process until the operation
	///   returns.
	/// </summary>
	public interface IAsyncOperation : IDisposable
	{
		/// <summary>
		///   Gets the exception that has been thrown during the execution of the operation.
		/// </summary>
		Exception Exception { get; }

		/// <summary>
		///   Gets a value indicating whether the process has completed.
		/// </summary>
		bool IsCompleted { get; }

		/// <summary>
		///   Updates the state of the asynchronous operation, setting its Exception and IsCompleted properties if the
		///   process has terminated.
		/// </summary>
		void UpdateState();
	}
}