namespace Pegasus.AssetsCompiler.Fonts
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Utilities;

	/// <summary>
	///     Represents a freetype library object.
	/// </summary>
	internal class FreeTypeLibrary : IDisposable
	{
		/// <summary>
		///     The native freetype library object.
		/// </summary>
		private IntPtr _library;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public FreeTypeLibrary()
		{
			FreeType.Invoke(() => FreeType.Initialize(out _library));
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			FreeType.Invoke(() => FreeType.DisposeLibrary(_library));
		}

		/// <summary>
		///     Creates a new font instance.
		/// </summary>
		/// <param name="fileName">The path to the font file.</param>
		/// <param name="size">The size (in pixels) of the characters.</param>
		/// <param name="bold">Indicates whether the font weight should be bold.</param>
		/// <param name="italic">Indicates whether the font should be italic.</param>
		/// <param name="renderMode">Indicates whether anti-aliasing should be used when rendering the glyphs.</param>
		/// <param name="characters">The range of characters supported by the font.</param>
		/// <param name="invalidChar">The character that is shown for invalid or unsupported characters.</param>
		public Font CreateFont(string fileName, int size, bool bold, bool italic, RenderMode renderMode, IEnumerable<char> characters, char invalidChar)
		{
			var font = IntPtr.Zero;
			FreeType.Invoke(() => FreeType.NewFace(_library, fileName, 0, out font));

			return new Font(font, size, bold, italic, renderMode, characters, invalidChar);
		}
	}
}
