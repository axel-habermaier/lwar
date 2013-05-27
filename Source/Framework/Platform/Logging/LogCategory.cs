using System;

namespace Pegasus.Framework.Platform.Logging
{
	/// <summary>
	///   Describes the category of a log entry.
	/// </summary>
	public enum LogCategory
	{
		/// <summary>
		///   Indicates that the log entry is unclassified.
		/// </summary>
		Unclassified,

		/// <summary>
		///   Indicates that the log entry is network related, but neither specifically client- or server related.
		/// </summary>
		Network,

		/// <summary>
		///   Indicates that the log entry is a client related network message.
		/// </summary>
		Client,

		/// <summary>
		///   Indicates that the log entry is a server related network message.
		/// </summary>
		Server,

		/// <summary>
		///   Indicates that the log entry is memory related.
		/// </summary>
		Memory,

		/// <summary>
		///   Indicates that the log entry is file-system related.
		/// </summary>
		FileSystem,

		/// <summary>
		///   Indicates that the log entry is asset related.
		/// </summary>
		Assets,

		/// <summary>
		///   Indicates that the log entry is graphics related.
		/// </summary>
		Graphics,

		/// <summary>
		/// Indicates that the log entry is platform related.
		/// </summary>
		Platform,

		/// <summary>
		///   Indicates that the log entry originates from or is related to an external process.
		/// </summary>
		External
	}
}