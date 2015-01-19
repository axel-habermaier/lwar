namespace Pegasus.Scripting
{
	using System;
	using Utilities;

	/// <summary>
	///     Describes the update mode of a cvar.
	/// </summary>
	public enum UpdateMode
	{
		/// <summary>
		///     Indicates that the value of the cvar should be updated immediately.
		/// </summary>
		Immediate,

		/// <summary>
		///     Indicates that the value of the cvar should be updated the next time the graphics subsystem is restarted.
		/// </summary>
		OnGraphicsRestart,

		/// <summary>
		///     Indicates that the value of the cvar should be updated the next time the application is restarted.
		/// </summary>
		OnAppRestart
	}

	/// <summary>
	///     Provides extension methods for the UpdateMode enumeration.
	/// </summary>
	internal static class UpdateModeExtensions
	{
		/// <summary>
		///     Gets the display string for the given mode.
		/// </summary>
		/// <param name="mode">The mode the update string should be returned for.,</param>
		public static string ToDisplayString(this UpdateMode mode)
		{
			switch (mode)
			{
				case UpdateMode.Immediate:
					return "Immediate";
				case UpdateMode.OnGraphicsRestart:
					return "A restart of the graphics subsystem is required.";
				case UpdateMode.OnAppRestart:
					return "A restart of the application is required.";
				default:
					Assert.NotReached("Unsupported update mode.");
					return "";
			}
		}
	}
}