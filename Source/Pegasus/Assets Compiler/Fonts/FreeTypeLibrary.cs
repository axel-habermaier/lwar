namespace Pegasus.AssetsCompiler.Fonts
{
	using System;

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
		public IntPtr CreateFont(string fileName)
		{
			var font = IntPtr.Zero;
			FreeType.Invoke(() => FreeType.NewFace(_library, fileName, 0, out font));

			return font;
		}
	}
}