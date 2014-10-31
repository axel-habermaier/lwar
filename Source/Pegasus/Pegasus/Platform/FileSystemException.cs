namespace Pegasus.Platform
{
	using System;

	/// <summary>
	///     Raised when a network error occurred.
	/// </summary>
	public class FileSystemException : Exception
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="message">A message providing further details about the exception.</param>
		public FileSystemException(string message)
			: base(message)
		{
		}
	}
}