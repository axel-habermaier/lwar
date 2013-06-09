using System;

namespace Pegasus.Framework.Platform.Logging
{
	/// <summary>
	///   Describes the type of a log entry.
	/// </summary>
	public enum LogType
	{
		/// <summary>
		///   Indicates that the log entry represents a fatal error.
		/// </summary>
		Fatal = 0,

		/// <summary>
		///   Indicates that the log entry represents an error.
		/// </summary>
		Error = 1,

		/// <summary>
		///   Indicates that the log entry represents a warning.
		/// </summary>
		Warning = 2,

		/// <summary>
		///   Indicates that the log entry represents an informational message.
		/// </summary>
		Info = 3,

		/// <summary>
		///   Indicates that the log entry represents debugging information.
		/// </summary>
		Debug = 4
	}
}