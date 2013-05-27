using System;

namespace Pegasus.Framework.Platform.Logging
{
	using System.Linq;
	using System.Text;

	/// <summary>
	///   Provides extension methods for logging-related types.
	/// </summary>
	public static class LogExtensions
	{
		/// <summary>
		///   The number of characters of the longest category literal.
		/// </summary>
		private static readonly int MaxCategoryLength = Enum.GetNames(typeof(LogCategory))
															.Where(c => c != LogCategory.Unclassified.ToString())
															.Max(c => c.Length);

		/// <summary>
		///   The number of characters of the longest type literal.
		/// </summary>
		private static readonly int MaxTypeLength = Enum.GetNames(typeof(LogType)).Max(c => c.Length);

		/// <summary>
		///   A cached string builder instance that is used to construct the strings.
		/// </summary>
		private static readonly StringBuilder Builder = new StringBuilder();

		/// <summary>
		///   Generates a display string for the given category. The length of the string is always the same, regardless of the
		///   category type.
		/// </summary>
		/// <param name="category">The category for which the display string should be returned.</param>
		public static string ToDisplayString(this LogCategory category)
		{
			Assert.InRange(category);

			var name = category.ToString();
			if (category == LogCategory.Unclassified)
				name = "General";

			return NormalizeLength(name, MaxCategoryLength);
		}

		/// <summary>
		///   Generates a display string for the given log type. The length of the string is always the same, regardless of the log
		///   type.
		/// </summary>
		/// <param name="type">The type for which the display string should be returned.</param>
		public static string ToDisplayString(this LogType type)
		{
			Assert.InRange(type);
			return NormalizeLength(type.ToString(), MaxTypeLength);
		}

		/// <summary>
		///   Appends spaces to the given string until the string length matches the desired length.
		/// </summary>
		/// <param name="content">The content whose length should be normalized.</param>
		/// <param name="length">The desired length of the string.</param>
		private static string NormalizeLength(string content, int length)
		{
			Builder.Clear();
			Builder.Append(content);

			for (var i = 0; i < length - content.Length; ++i)
				Builder.Append(" ");

			return Builder.ToString();
		}
	}
}