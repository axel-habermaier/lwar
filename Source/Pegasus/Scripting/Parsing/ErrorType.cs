using System;

namespace Pegasus.Scripting.Parsing
{
	/// <summary>
	///   Indicates which type of error occurred.
	/// </summary>
	public enum ErrorType
	{
		/// <summary>
		///   Indicates that the input does not match the expected input.
		/// </summary>
		Expected,

		/// <summary>
		///   Indicates that the parser encountered some unexpected input.
		/// </summary>
		Unexpected,

		/// <summary>
		///   Indicates that the parser generated an uncategorized error message.
		/// </summary>
		Message
	}
}