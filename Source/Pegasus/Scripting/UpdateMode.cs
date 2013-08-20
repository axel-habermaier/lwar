using System;

namespace Pegasus.Scripting
{
	using System.ComponentModel;
	using System.Linq;

	/// <summary>
	///   Describes the update mode of a cvar.
	/// </summary>
	public enum UpdateMode
	{
		/// <summary>
		///   Indicates that the value of the cvar should be updated immediately.
		/// </summary>
		[Description("Immediate")]
		Immediate = 0,

		/// <summary>
		///   Indicates that the value of the cvar should be updated the next time the graphics subsystem is restarted.
		/// </summary>
		[Description("A restart of the graphics subsystem is required.")]
		OnGraphicsRestart
	}

	/// <summary>
	///   Provides extension methods for the UpdateMode enumeration.
	/// </summary>
	internal static class UpdateModeExtensions
	{
		/// <summary>
		///   Gets the display string for the given mode.
		/// </summary>
		/// <param name="mode">The mode the update string should be returned for.,</param>
		public static string ToDisplayString(this UpdateMode mode)
		{
			return typeof(UpdateMode)
				.GetField(mode.ToString())
				.GetCustomAttributes(typeof(DescriptionAttribute), false)
				.Cast<DescriptionAttribute>()
				.First()
				.Description;
		}
	}
}