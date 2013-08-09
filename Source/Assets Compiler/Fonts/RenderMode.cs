using System;

namespace Pegasus.AssetsCompiler.Fonts
{
	/// <summary>
	///   Determines the glyph rendering quality.
	/// </summary>
	internal enum RenderMode
	{
		/// <summary>
		///   Indicates that the glyphs are rendered with applying an anti-aliasing technique.
		/// </summary>
		Antialiased = 0,

		/// <summary>
		///   Indicates that the glyphs are rendered without applying any anti-aliasing techniques.
		/// </summary>
		Aliased = 2,
	}
}