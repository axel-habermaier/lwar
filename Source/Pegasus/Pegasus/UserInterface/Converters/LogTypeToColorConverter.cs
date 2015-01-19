namespace Pegasus.UserInterface.Converters
{
	using System;
	using Platform.Graphics;
	using Platform.Logging;
	using Rendering;
	using Utilities;

	/// <summary>
	///     Converts a log type to a color value.
	/// </summary>
	internal class LogTypeToColorConverter : ValueConverter<LogTypeToColorConverter, LogType, Color>
	{
		/// <summary>
		///     The display color of error messages.
		/// </summary>
		private static readonly Color ErrorColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

		/// <summary>
		///     The display color of warnings.
		/// </summary>
		private static readonly Color WarningColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);

		/// <summary>
		///     The display color of normal messages.
		/// </summary>
		private static readonly Color InfoColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		/// <summary>
		///     The display color of debug messages.
		/// </summary>
		private static readonly Color DebugInfoColor = new Color(1.0f, 0.0f, 1.0f, 1.0f);

		/// <summary>
		///     Converts the given value to the target type.
		/// </summary>
		/// <param name="type">The value that should be converted.</param>
		public override Color ConvertToTarget(LogType type)
		{
			switch (type)
			{
				case LogType.Fatal:
				case LogType.Error:
					return ErrorColor;
				case LogType.Warning:
					return WarningColor;
				case LogType.Info:
					return InfoColor;
				case LogType.Debug:
					return DebugInfoColor;
				default:
					Assert.NotReached("Unknown log type.");
					return Colors.White;
			}
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override LogType ConvertToSource(Color value)
		{
			Assert.NotReached("Unsupported conversion.");
			return LogType.Fatal;
		}
	}
}