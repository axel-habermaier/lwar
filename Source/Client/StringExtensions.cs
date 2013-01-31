using System;

namespace Lwar.Client
{
	using System.Text;
	using Pegasus.Framework;

	/// <summary>
	///   Provides extension methods for strings.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		///   Truncates the string to the given maximum allowed UTF8 size.
		/// </summary>
		/// <param name="s">The string that should be truncated.</param>
		/// <param name="maxSize">The maximum UTF8 size of the string in bytes.</param>
		public static string TruncateUtf8(this string s, int maxSize)
		{
			Assert.ArgumentNotNull(s, () => s);
			Assert.ArgumentInRange(maxSize, () => maxSize, 1, Int32.MaxValue);

			while (Encoding.UTF8.GetByteCount(s) > maxSize)
				s = s.Substring(0, maxSize);

			return s;
		}
	}
}