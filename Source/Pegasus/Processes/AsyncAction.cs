using System;

namespace Pegasus.Processes
{
	using System.Threading.Tasks;

	/// <summary>
	///   Represents an action that is scheduled and executed asynchronously on the main thread.
	/// </summary>
	/// <param name="context">The context of the process that represents the asynchronous action.</param>
	public delegate Task AsyncAction(ProcessContext context);
}