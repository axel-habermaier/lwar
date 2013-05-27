using System;

namespace Pegasus.Framework.Scripting.Parsing
{
	/// <summary>
	///   Represents a parser error message.
	/// </summary>
	public struct ErrorMessage
	{
		/// <summary>
		///   An error message describing an unexpected end of input error.
		/// </summary>
		internal static readonly ErrorMessage UnexpectedEndOfInput = new ErrorMessage(ErrorType.Unexpected, "end of input");

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="type">A value describing the type of the error.</param>
		/// <param name="message">A message describing the error.</param>
		internal ErrorMessage(ErrorType type, string message)
			: this()
		{
			Assert.ArgumentInRange(type);
			Assert.ArgumentNotNullOrWhitespace(message);

			Type = type;
			Message = message;
		}

		/// <summary>
		///   Gets a value describing the type of the error.
		/// </summary>
		public ErrorType Type { get; private set; }

		/// <summary>
		///   Gets a message describing the error.
		/// </summary>
		public string Message { get; private set; }
	}
}