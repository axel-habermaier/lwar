using System;

namespace Pegasus.AssetsCompiler.FreeType
{
	using Framework.Platform.Memory;

	/// <summary>
	///   Represents a freetype library object.
	/// </summary>
	internal class FreeTypeLibrary : DisposableObject
	{
		/// <summary>
		///   The native freetype library object.
		/// </summary>
		private IntPtr _library;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public FreeTypeLibrary()
		{
			NativeMethods.Invoke(() => NativeMethods.Initialize(out _library));
		}

		/// <summary>
		///   Creates a new font instance.
		/// </summary>
		/// <param name="fileName">The path to the font file.</param>
		/// <param name="faceIndex">The zero-based index of the face within the font.</param>
		/// <returns></returns>
		public FontFace CreateFont(string fileName, int faceIndex)
		{
			var font = IntPtr.Zero;
			NativeMethods.Invoke(() => NativeMethods.NewFace(_library, fileName, faceIndex, out font));

			return new FontFace(font);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.Invoke(() => NativeMethods.DisposeLibrary(_library));
		}
	}
}