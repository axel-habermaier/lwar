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
		General = 0,

		/// <summary>
		///   Indicates that the log entry is network related, but neither specifically client- or server related.
		/// </summary>
		Network = 1,

		/// <summary>
		///   Indicates that the log entry is a client related network message.
		/// </summary>
		Client = 2,

		/// <summary>
		///   Indicates that the log entry is a server related network message.
		/// </summary>
		Server = 3,

		/// <summary>
		///   Indicates that the log entry is memory related.
		/// </summary>
		Memory = 4,

		/// <summary>
		///   Indicates that the log entry is file-system related.
		/// </summary>
		FileSystem = 5,

		/// <summary>
		///   Indicates that the log entry is asset related.
		/// </summary>
		Assets = 6,

		/// <summary>
		///   Indicates that the log entry is graphics related.
		/// </summary>
		Graphics = 7,

		/// <summary>
		///   Indicates that the log entry is platform related.
		/// </summary>
		Platform = 8,

		/// <summary>
		///   Indicates that the log entry originates from or is related to an external process.
		/// </summary>
		External = 9
	}
}