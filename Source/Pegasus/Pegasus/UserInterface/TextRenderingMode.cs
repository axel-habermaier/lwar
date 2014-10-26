namespace Pegasus.UserInterface
{
	using System;

	/// <summary>
	///     Defines the supported rendering modes for text.
	/// </summary>
	public enum TextRenderingMode
	{
		/// <summary>
		///     Indicates that text is rendered anti-aliased.
		/// </summary>
		ClearType = 0,

		/// <summary>
		///     Indicates that text is rendered without any anti-aliasing.
		/// </summary>
		Aliased
	}
}