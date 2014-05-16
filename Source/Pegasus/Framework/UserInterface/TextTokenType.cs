namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Identifies the type of a text token.
	/// </summary>
	internal enum TextTokenType
	{
		/// <summary>
		///     Indicates that the token represents of a word, including digits and special characters.
		/// </summary>
		Word,

		/// <summary>
		///     Indicates that the token represents a space character.
		/// </summary>
		Space,

		/// <summary>
		///     Indicates that the token represents a space character that should be replaced by a new line.
		/// </summary>
		WrappedSpace,

		/// <summary>
		///     Indicates that the token represents a new line marker.
		/// </summary>
		NewLine,

		/// <summary>
		///     Indicates that the token represents a wrap marker.
		/// </summary>
		Wrap,

		/// <summary>
		///     Indicates that the sequence represents the end of the text.
		/// </summary>
		EndOfText
	}
}