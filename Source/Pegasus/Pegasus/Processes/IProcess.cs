namespace Pegasus.Processes
{
	using System;

	/// <summary>
	///     Represents an asynchronous process.
	/// </summary>
	public interface IProcess : IDisposable
	{
		/// <summary>
		///     Gets a value indicating whether the execution of the process has been canceled before it has been completed.
		/// </summary>
		bool IsCanceled { get; }

		/// <summary>
		///     Gets a value indicating whether the execution of the process has completed without being canceled.
		/// </summary>
		bool IsCompleted { get; }

		/// <summary>
		///     Gets a value indicating whether the process has completed because of an unhandled exception.
		/// </summary>
		bool IsFaulted { get; }

		/// <summary>
		///     Immediately cancels the execution of the process, provided that the process is still being executed.
		/// </summary>
		void Cancel();
	}
}