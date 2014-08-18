namespace Pegasus.Processes
{
	using System;

	/// <summary>
	///     Represents an asynchronous operation that can block the execution of an asynchronous process until the operation
	///     returns.
	/// </summary>
	/// <typeparam name="TResult">The type of the result of the </typeparam>
	public interface IAsyncOperation<out TResult> : IAsyncOperation
	{
		/// <summary>
		///     Gets the result produced by the asynchronous operation.
		/// </summary>
		TResult Result { get; }
	}
}