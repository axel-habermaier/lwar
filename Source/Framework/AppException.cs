using System;

namespace Pegasus.Framework
{
	/// <summary>
	///   Represents a fatal error that causes the execution of the application to be aborted.
	/// </summary>
	public class AppException : Exception
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="message">A message explaining the fatal error.</param>
		public AppException(string message)
			: base(message)
		{
		}
	}
}