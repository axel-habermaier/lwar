﻿using System;

namespace Pegasus.Scripting.Parsing
{
	/// <summary>
	///   Indicates whether the parser that generated a reply succeeded.
	/// </summary>
	public enum ReplyStatus
	{
		/// <summary>
		///   Indicates that the parser generated an error.
		/// </summary>
		Error,

		/// <summary>
		///   Indicates that the parser generated a fatal error from which no recovery should be attempted.
		/// </summary>
		FatalError,

		/// <summary>
		///   Indicates that the parser was successful.
		/// </summary>
		Success
	}
}