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
		//public static string Truncate(this string s, int maxUtf8Size)
		//{
		//	Assert.ArgumentNotNull(s, () => s);
		//	Assert.ArgumentInRange(maxUtf8Size, () => maxUtf8Size, 1, Int32.MaxValue);

		//	if (Encoding.UTF8.GetByteCount(s) > maxUtf8Size)
		//	{
		//		do
		//		{
		//			s = s.Substring(0, maxUtf8Size);
		//		} while (Encoding.UTF8.GetByteCount(s) > maxUtf8Size);

		//		Log.Warn("Your player name '{0}' is too long and has been truncated to '{1}'.", name, playerName);
		//	}
		//}
	}
}