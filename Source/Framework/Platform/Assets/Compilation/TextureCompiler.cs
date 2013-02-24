using System;

namespace Pegasus.Framework.Platform.Assets.Compilation
{
	using System.Drawing.Imaging;
	using Graphics;

	/// <summary>
	///   Provides common methods required for the compilation of textures.
	/// </summary>
	public abstract class TextureCompiler : AssetCompiler
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		protected TextureCompiler(string asset)
			: base(asset)
		{
		}

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