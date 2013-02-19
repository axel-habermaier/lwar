using System;

namespace Pegasus.AssetsCompiler
{
	using System.Drawing.Imaging;
	using Framework;
	using Framework.Platform.Graphics;

	public abstract class TextureProcessor : AssetProcessor
	{
		/// <summary>
		///   The path to the nvcompress executable.
		/// </summary>
		protected const string NvCompressPath = "../../Dependencies/nvcompress.exe";

		/// <summary>
		///   The path to the nvassemble executable.
		/// </summary>
		protected const string NvAssemblePath = "../../Dependencies/nvassemble.exe";

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
		///   Gets the number of components from the surface format.
		/// </summary>
		/// <param name="format">The surface format.</param>
		protected static byte ComponentCount(SurfaceFormat format)
		{
			switch (format)
			{
				case SurfaceFormat.Rgba8:
					return 4;
				case SurfaceFormat.Rgb8:
					return 3;
				case SurfaceFormat.R8:
					return 1;
				default:
					throw new InvalidOperationException("Unsupported surface format.");
			}
		}

		/// <summary>
		///   Converts the pixel format into a surface format.
		/// </summary>
		/// <param name="format">The pixel format that should be converted.</param>
		protected static SurfaceFormat Convert(PixelFormat format)
		{
			switch (format)
			{
				case PixelFormat.Format32bppArgb:
					return SurfaceFormat.Rgba8;
				case PixelFormat.Format24bppRgb:
					return SurfaceFormat.Rgb8;
				case PixelFormat.Format8bppIndexed:
					return SurfaceFormat.Rgba8;
				default:
					throw new InvalidOperationException(String.Format("Unsupported texture format: {0}.", format));
			}
		}
	}
}