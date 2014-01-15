namespace Pegasus.Rendering.UserInterface
{
	using System;

	/// <summary>
	///     Provides extension methods for the text alignment enumeration.
	/// </summary>
	public static class TextAlignmentExtensions
	{
		/// <summary>
		///     Checks whether the given alignment specifies the given flag. We cannot use .Net's Enum.HasFlag method,
		///     as this method boxes the enumeration value each time it is called.
		/// </summary>
		/// <param name="alignment">The alignment value that should be checked.</param>
		/// <param name="flag">The flag that should be checked.</param>
		private static bool HasFlag(TextAlignment alignment, TextAlignment flag)
		{
			return (alignment & flag) == flag;
		}

		/// <summary>
		///     Checks whether the text should be left aligned.
		/// </summary>
		/// <param name="alignment">The alignment value that should be checked.</param>
		public static bool IsLeftAligned(this TextAlignment alignment)
		{
			return !alignment.IsRightAligned() && !alignment.IsHorizontallyCentered();
		}

		/// <summary>
		///     Checks whether the text should be right aligned.
		/// </summary>
		/// <param name="alignment">The alignment value that should be checked.</param>
		public static bool IsRightAligned(this TextAlignment alignment)
		{
			return HasFlag(alignment, TextAlignment.Right);
		}

		/// <summary>
		///     Checks whether the text should be horizontally centered.
		/// </summary>
		/// <param name="alignment">The alignment value that should be checked.</param>
		public static bool IsHorizontallyCentered(this TextAlignment alignment)
		{
			return HasFlag(alignment, TextAlignment.Centered);
		}

		/// <summary>
		///     Checks whether the text should be top aligned.
		/// </summary>
		/// <param name="alignment">The alignment value that should be checked.</param>
		public static bool IsTopAligned(this TextAlignment alignment)
		{
			return !alignment.IsBottomAligned() && !alignment.IsVerticallyCentered();
		}

		/// <summary>
		///     Checks whether the text should be bottom aligned.
		/// </summary>
		/// <param name="alignment">The alignment value that should be checked.</param>
		public static bool IsBottomAligned(this TextAlignment alignment)
		{
			return HasFlag(alignment, TextAlignment.Bottom);
		}

		/// <summary>
		///     Checks whether the text should be vertically centered.
		/// </summary>
		/// <param name="alignment">The alignment value that should be checked.</param>
		public static bool IsVerticallyCentered(this TextAlignment alignment)
		{
			return HasFlag(alignment, TextAlignment.Middle);
		}
	}
}