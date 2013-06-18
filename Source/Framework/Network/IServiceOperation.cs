using System;

namespace Pegasus.Framework.Network
{
	using Processes;

	/// <summary>
	///   Represents a service operation that has been invoked and is being executed.
	/// </summary>
	internal interface IServiceOperation : IAsyncOperation
	{
		/// <summary>
		///   Sets the result of the operation invocation and marks the invocation as completed.
		/// </summary>
		/// <param name="packet">The packet that contains the result returned by the server.</param>
		void SetResult(IncomingPacket packet);

		/// <summary>
		///   Sets the exception that was thrown during the execution of the operation and marks the invocation as completed.
		/// </summary>
		/// <param name="exception">The exception that has been thrown.</param>
		void SetException(Exception exception);
	}
}