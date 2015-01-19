namespace Pegasus.Platform.Network
{
	using System;
	using Utilities;

	/// <summary>
	///     Raised when a network error occurred.
	/// </summary>
	public class NetworkException : Exception
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="message">A message explaining the exception.</param>
		/// <param name="args">The format arguments for the exception message.</param>
		[StringFormatMethod("message")]
		public NetworkException(string message, params object[] args)
			: base(String.Format(message, args))
		{
		}
	}
}