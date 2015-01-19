namespace Pegasus.AssetsCompiler.Utilities
{
	using System;

	/// <summary>
	///     Represents a fatal error that causes the execution of the application to be aborted.
	/// </summary>
	public class PegasusException : Exception
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="message">A message explaining the fatal error.</param>
		public PegasusException(string message)
			: base(message)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="message">A message explaining the fatal error.</param>
		/// <param name="arguments">The arguments that should be used to format the message.</param>
		[StringFormatMethod("message")]
		public PegasusException(string message, params object[] arguments)
			: base(String.Format(message, arguments))
		{
		}
	}
}