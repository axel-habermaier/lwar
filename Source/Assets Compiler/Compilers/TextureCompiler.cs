using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Drawing.Imaging;
	using Assets;
	using Framework;
	using Framework.Platform.Graphics;

	/// <summary>
	///   Provides common methods required for the compilation of textures.
	/// </summary>
	/// <typeparam name="TTexture">The type of the texture that is compiled.</typeparam>
	internal abstract class TextureCompiler<TTexture> : AssetCompiler<TTexture>
		where TTexture : Asset
	{
		/// <summary>
		///   Checks whether the given number is a power of two.
		/// </summary>
		/// <param name="number">The number that should be checked.</param>
		protected static bool IsPowerOfTwo(int number)
		{
			Assert.ArgumentInRange(number, () => number, 1, Int32.MaxValue);
			return (number & (number - 1)) == 0;
		}

		/// <summary>
		///   Chooses a suitable compressed format for the given uncompressed format.
		/// </summary>
		/// <param name="format">The pixel format for which a suitable compressed format should be chosen.</param>
		protected static SurfaceFormat ChooseCompression(PixelFormat format)
		{
			switch (format)
			{
				case PixelFormat.Format8bppIndexed:
					return SurfaceFormat.Bc4;
				case PixelFormat.Format24bppRgb:
					return SurfaceFormat.Bc1;
				case PixelFormat.Format32bppArgb:
					return SurfaceFormat.Bc3;
				default:
					throw new InvalidOperationException("Unsupported uncompressed format.");
			}
		}
	}
}