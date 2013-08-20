using System;

namespace Pegasus.Framework
{
	/// <summary>
	///   Represents a fatal error that causes the execution of the application to be aborted.
	/// </summary>
	[Serializable]
	public class PegasusException : Exception
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="message">A message explaining the fatal error.</param>
		public PegasusException(string message)
			: base(message)
		{
		}
	}
}