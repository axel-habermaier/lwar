namespace Pegasus.Framework.UserInterface
{
	using System;
	using Platform.Graphics;
	using Rendering.UserInterface;

	/// <summary>
	///     Provides a method to search for a font based on certain font settings.
	/// </summary>
	public interface IFontLoader
	{
		/// <summary>
		///     Sets the next font loader that is used to load the font if the current loader fails to
		///     load an appropriate font.
		/// </summary>
		IFontLoader Next { set; }

		/// <summary>
		///     Gets the font matching the given font settings.
		/// </summary>
		/// <param name="fontFamily">The family of the font that should be returned.</param>
		/// <param name="size">The size of the font that should be returned.</param>
		/// <param name="bold">Indicates whether the font should be bold.</param>
		/// <param name="italic">Indicates whether the font should be italic.</param>
		/// <param name="aliased">Indicates whether the font should be aliased.</param>
		Font LoadFont(string fontFamily, int size, bool bold, bool italic, bool aliased);
	}
}