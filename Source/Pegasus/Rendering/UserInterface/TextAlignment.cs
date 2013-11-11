namespace Pegasus.Rendering.UserInterface
{
	using System;

	/// <summary>
	///   Describes the alignment of text.
	/// </summary>
	[Flags]
	public enum TextAlignment
	{
		/// <summary>
		///   Indicates that the text should be aligned to the left.
		/// </summary>
		Left = 1,

		/// <summary>
		///   Indicates that the text should be horizontally centered.
		/// </summary>
		Centered = 2,

		/// <summary>
		///   Indicates that the text should be aligned to the right.
		/// </summary>
		Right = 4,

		/// <summary>
		///   Indicates that the text should be aligned to the top.
		/// </summary>
		Top = 8,

		/// <summary>
		///   Indicates that the text should be vertically centered.
		/// </summary>
		Middle = 16,

		/// <summary>
		///   Indicates that the text should be aligned to the bottom.
		/// </summary>
		Bottom = 32
	}
}