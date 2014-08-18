namespace Pegasus.Processes
{
	using System;

	/// <summary>
	///     Represents an resumable asynchronous process.
	/// </summary>
	internal interface IResumableProcess : IProcess
	{
		/// <summary>
		///     Resumes the process if the asynchronous operation the process is currently waiting for has completed executing.
		/// </summary>
		void Resume();
	}
}