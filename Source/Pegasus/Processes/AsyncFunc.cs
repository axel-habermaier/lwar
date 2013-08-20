using System;

namespace Pegasus.Framework.Processes
{
	using System.Threading.Tasks;

	/// <summary>
	///   Represents a function that is scheduled and executed asynchronously on the main thread.
	/// </summary>
	/// <param name="context">The context of the process that represents the asynchronous action.</param>
	public delegate Task<TResult> AsyncFunc<TResult>(ProcessContext context);
}