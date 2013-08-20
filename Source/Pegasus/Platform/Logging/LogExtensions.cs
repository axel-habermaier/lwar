using System;

namespace Pegasus.Framework.Platform.Logging
{
	using System.Diagnostics;
	using System.Linq;

	/// <summary>
	///   Provides extension methods for logging-related types.
	/// </summary>
	public static class LogExtensions
	{
		/// <summary>
		///   The number of characters of the longest type literal.
		/// </summary>
		private static readonly int MaxTypeLength = Enum.GetNames(typeof(LogType)).Max(c => c.Length);

		/// <summary>
		///   The display strings of the log types, all of the same length.
		/// </summary>
		private static readonly string[] TypeDisplayStrings =
			Enum.GetNames(typeof(LogType))
				.Select(category => NormalizeLength(category, MaxTypeLength))
				.ToArray();

		/// <summary>
		///   Initializes the type.
		/// </summary>
		static LogExtensions()
		{
			ValidateEnumeration(typeof(LogType), 1);
		}

		/// <summary>
		///   Appends spaces to the given string until the string length matches the desired length.
		/// </summary>
		/// <param name="content">The content whose length should be normalized.</param>
		/// <param name="length">The desired length of the string.</param>
		private static string NormalizeLength(string content, int length)
		{
			var normalized = content;

			for (var i = 0; i < length - content.Length; ++i)
				normalized += " ";

			return normalized;
		}

		/// <summary>
		///   Validates the assumptions made about a logging enumeration.
		/// </summary>
		/// <param name="enumeration">The type of the enumeration that should be validated.</param>
		/// <param name="lowestValue">The lowest value that must be defined by the enumeration.</param>
		[Conditional("DEBUG")]
		private static void ValidateEnumeration(Type enumeration, int lowestValue)
		{
			var values = Enum.GetValues(enumeration) as int[];
			Assert.That(values != null, "Expected the values to be of type 'int'.");

			var maxValue = values.Max();
			var minValue = values.Min();

			Assert.That(minValue == lowestValue, "The lowest value must be {0}.", lowestValue);
			Assert.That(maxValue == values.Length - 1 + lowestValue,
				"The highest value must match the number of literals declared by the enumeration.");
		}

		/// <summary>
		///   Converts the given log type to a string with a normalized length.
		/// </summary>
		/// <param name="type">The log type that should be converted to a string.</param>
		public static string ToDisplayString(this LogType type)
		{
			return TypeDisplayStrings[(int)type - 1];
		}
	}
}