using System;

namespace Pegasus.Platform.Network
{
	using System.Net.Sockets;

	/// <summary>
	///   Raised when a socket operation throws an exception.
	/// </summary>
	public class SocketOperationException : Exception
	{
		/// <summary>
		///   Initializes a new instance with the given error message.
		/// </summary>
		/// <param name="message">The error message describing the exception.</param>
		/// <param name="arguments">The arguments that should be copied into the error message.</param>
		public SocketOperationException(string message, params object[] arguments)
			: base(String.Format(message, arguments))
		{
		}

		/// <summary>
		///   Initializes a new instance with the given error message and the socket exception that has been raised.
		/// </summary>
		/// <param name="message">The error message describing the exception.</param>
		/// <param name="socketException">The socket exception that has been raised.</param>
		/// <param name="arguments">The arguments that should be copied into the error message.</param>
		public SocketOperationException(string message, SocketException socketException, params object[] arguments)
			: base(String.Format("{0} [{1}.]", String.Format(message, arguments), socketException.Message), socketException)
		{
		}
	}
}