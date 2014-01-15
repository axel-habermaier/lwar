namespace Pegasus.Processes
{
	using System;

	/// <summary>
	///     Represents an asynchronous process.
	/// </summary>
	/// <typeparam name="TResult">The type of the result of the process.</typeparam>
	public interface IProcess<out TResult> : IProcess
	{
		/// <summary>
		///     Gets the result of the process if the execution of the process is complete.
		/// </summary>
		TResult Result { get; }
	}
}