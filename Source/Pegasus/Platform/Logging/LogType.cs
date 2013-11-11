namespace Pegasus.Platform.Logging
{
	using System;

	/// <summary>
	///   Describes the type of a log entry.
	/// </summary>
	public enum LogType
	{
		/// <summary>
		///   Indicates that the log entry represents a fatal error.
		/// </summary>
		Fatal = 1,

		/// <summary>
		///   Indicates that the log entry represents an error.
		/// </summary>
		Error = 2,

		/// <summary>
		///   Indicates that the log entry represents a warning.
		/// </summary>
		Warning = 3,

		/// <summary>
		///   Indicates that the log entry represents an informational message.
		/// </summary>
		Info = 4,

		/// <summary>
		///   Indicates that the log entry represents debugging information.
		/// </summary>
		Debug = 5
	}
}