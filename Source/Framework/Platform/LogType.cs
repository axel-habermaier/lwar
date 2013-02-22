using System;

namespace Pegasus.Framework.Platform
{
	/// <summary>
	///   Describes the type of a log entry.
	/// </summary>
	public enum LogType
	{
		/// <summary>
		///   Indicates that the log entry represents a fatal error.
		/// </summary>
		FatalError,

		/// <summary>
		///   Indicates that the log entry represents an error.
		/// </summary>
		Error,

		/// <summary>
		///   Indicates that the log entry represents a warning.
		/// </summary>
		Warning,

		/// <summary>
		///   Indicates that the log entry represents an informational message.
		/// </summary>
		Info,

		/// <summary>
		///   Indicates that the log entry represents debugging information.
		/// </summary>
		DebugInfo
	}
}