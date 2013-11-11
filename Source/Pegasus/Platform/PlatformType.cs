namespace Pegasus.Platform
{
	using System;

	/// <summary>
	///   Indicates the platform on which the application is running.
	/// </summary>
	public enum PlatformType
	{
		/// <summary>
		///   Indicates that the application is running as a Windows desktop application.
		/// </summary>
		Windows = 1,

		/// <summary>
		///   Indicates that the application is running as a Linux application.
		/// </summary>
		Linux = 2
	}
}