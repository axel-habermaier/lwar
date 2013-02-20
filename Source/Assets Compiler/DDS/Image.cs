using System;

namespace Pegasus.AssetsCompiler.DDS
{
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Implements a subset of the DX10 DDS file specification based on the sample provided by Microsoft at
	///   http://msdn.microsoft.com/en-us/library/windows/apps/jj651550.aspx.
	/// </summary>
	public class Image
	{
		/// <summary>
		///   The magic DDS file code "DDS ".
		/// </summary>
		private const int MagicCode = 0x20534444;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="buffer">The buffer that contains the contents of the DDS file.</param>
		public unsafe Image(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			if (buffer.BufferSize < sizeof(uint) + sizeof(Header) + sizeof(Dx10Header))
				Log.Die("Invalid DDS file: Header information is incomplete.");

			if (buffer.ReadUInt32() != MagicCode)
				Log.Die("Not a DDS file.");

			var header = new Header(buffer);
			var dx10Header = new Dx10Header(buffer);

			if (header.Size != sizeof(Header) || header.PixelFormat.Size != sizeof(PixelFormat))
				Log.Die("DDS file is corrupt.");

			if (!header.PixelFormat.Flags.HasFlag(PixelFormatFlags.FourCC) ||
				header.PixelFormat.FourCC != MakeFourCC('D', 'X', '1', '0'))
				Log.Die("DDS file is not in DX10 format.");

			if (dx10Header.ArraySize == 0)
				Log.Die("An array size of 0 is invalid.");
		}

		/// <summary>
		///   Creates a FourCC from the given values.
		/// </summary>
		private static uint MakeFourCC(uint ch0, uint ch1, uint ch2, uint ch3)
		{
			return ((byte)(ch0) | ((uint)(byte)(ch1) << 8) | ((uint)(byte)(ch2) << 16) | ((uint)(byte)(ch3) << 24));
		}

		/// <summary>
		///   Gets the surface information for a particular data format.
		/// </summary>
		/// <param name="width">The width of the surface.</param>
		/// <param name="height">The height of the surface.</param>
		/// <param name="format">The data format of the surface.</param>
		/// <param name="numBytes">The total number of bytes that are required to store the surface.</param>
		/// <param name="rowBytes">The total number of bytes that are required to store a row of the surface.</param>
		/// <param name="numRows">The total number of rows contained in the surface.</param>
		private static void GetSurfaceInfo(uint width, uint height, Format format,
										   out uint numBytes, out uint rowBytes, out uint numRows)
		{
			var blockCompressed = false;
			var packed = false;
			uint bytesPerBlock = 0;

			switch (format)
			{
				case Format.BC1_Typeless:
				case Format.BC1_UNorm:
				case Format.BC1_UNorm_SRgb:
				case Format.BC4_Typeless:
				case Format.BC4_UNorm:
				case Format.BC4_SNorm:
					blockCompressed = true;
					bytesPerBlock = 8;
					break;
				case Format.BC2_Typeless:
				case Format.BC2_UNorm:
				case Format.BC2_UNorm_SRgb:
				case Format.BC3_Typeless:
				case Format.BC3_UNorm:
				case Format.BC3_UNorm_SRgb:
				case Format.BC5_Typeless:
				case Format.BC5_UNorm:
				case Format.BC5_SNorm:
				case Format.BC6H_Typeless:
				case Format.BC6H_Uf16:
				case Format.BC6H_Sf16:
				case Format.BC7_Typeless:
				case Format.BC7_UNorm:
				case Format.BC7_UNorm_SRgb:
					blockCompressed = true;
					bytesPerBlock = 16;
					break;
				case Format.R8G8_B8G8_UNorm:
				case Format.G8R8_G8B8_UNorm:
					packed = true;
					break;
			}

			if (blockCompressed)
			{
				uint numBlocksWide = 0;
				uint numBlocksHigh = 0;

				if (width > 0)
					numBlocksWide = Math.Max(1, (width + 3) / 4);
				if (height > 0)
					numBlocksHigh = Math.Max(1, (height + 3) / 4);

				rowBytes = numBlocksWide * bytesPerBlock;
				numRows = numBlocksHigh;
			}
			else if (packed)
			{
				rowBytes = ((width + 1) >> 1) * 4;
				numRows = height;
			}
			else
			{
				var bpp = BitsPerPixel(format);
				rowBytes = (uint)(width * bpp + 7) / 8; // round up to nearest byte
				numRows = height;
			}

			numBytes = rowBytes * numRows;
		}

		/// <summary>
		///   Returns the number of bits per pixel for the given data format.
		/// </summary>
		/// <param name="format">The data format for which the number of bits per pixel should be returned.</param>
		private static int BitsPerPixel(Format format)
		{
			switch (format)
			{
				case Format.R32G32B32A32_Typeless:
				case Format.R32G32B32A32_Float:
				case Format.R32G32B32A32_UInt:
				case Format.R32G32B32A32_SInt:
					return 128;
				case Format.R32G32B32_Typeless:
				case Format.R32G32B32_Float:
				case Format.R32G32B32_UInt:
				case Format.R32G32B32_SInt:
					return 96;
				case Format.R16G16B16A16_Typeless:
				case Format.R16G16B16A16_Float:
				case Format.R16G16B16A16_UNorm:
				case Format.R16G16B16A16_UInt:
				case Format.R16G16B16A16_SNorm:
				case Format.R16G16B16A16_SInt:
				case Format.R32G32_Typeless:
				case Format.R32G32_Float:
				case Format.R32G32_UInt:
				case Format.R32G32_SInt:
				case Format.R32G8X24_Typeless:
				case Format.D32_Float_S8X24_UInt:
				case Format.R32_Float_X8X24_Typeless:
				case Format.X32_Typeless_G8X24_UInt:
					return 64;
				case Format.R10G10B10A2_Typeless:
				case Format.R10G10B10A2_UNorm:
				case Format.R10G10B10A2_UInt:
				case Format.R11G11B10_Float:
				case Format.R8G8B8A8_Typeless:
				case Format.R8G8B8A8_UNorm:
				case Format.R8G8B8A8_UNorm_SRgb:
				case Format.R8G8B8A8_UInt:
				case Format.R8G8B8A8_SNorm:
				case Format.R8G8B8A8_SInt:
				case Format.R16G16_Typeless:
				case Format.R16G16_Float:
				case Format.R16G16_UNorm:
				case Format.R16G16_UInt:
				case Format.R16G16_SNorm:
				case Format.R16G16_SInt:
				case Format.R32_Typeless:
				case Format.D32_Float:
				case Format.R32_Float:
				case Format.R32_UInt:
				case Format.R32_SInt:
				case Format.R24G8_Typeless:
				case Format.D24_UNorm_S8_UInt:
				case Format.R24_UNorm_X8_Typeless:
				case Format.X24_Typeless_G8_UInt:
				case Format.R9G9B9E5_Sharedexp:
				case Format.R8G8_B8G8_UNorm:
				case Format.G8R8_G8B8_UNorm:
				case Format.B8G8R8A8_UNorm:
				case Format.B8G8R8X8_UNorm:
				case Format.R10G10B10_Xr_Bias_A2_UNorm:
				case Format.B8G8R8A8_Typeless:
				case Format.B8G8R8A8_UNorm_SRgb:
				case Format.B8G8R8X8_Typeless:
				case Format.B8G8R8X8_UNorm_SRgb:
					return 32;
				case Format.R8G8_Typeless:
				case Format.R8G8_UNorm:
				case Format.R8G8_UInt:
				case Format.R8G8_SNorm:
				case Format.R8G8_SInt:
				case Format.R16_Typeless:
				case Format.R16_Float:
				case Format.D16_UNorm:
				case Format.R16_UNorm:
				case Format.R16_UInt:
				case Format.R16_SNorm:
				case Format.R16_SInt:
				case Format.B5G6R5_UNorm:
				case Format.B5G5R5A1_UNorm:
					return 16;
				case Format.R8_Typeless:
				case Format.R8_UNorm:
				case Format.R8_UInt:
				case Format.R8_SNorm:
				case Format.R8_SInt:
				case Format.A8_UNorm:
					return 8;
				case Format.R1_UNorm:
					return 1;
				case Format.BC1_Typeless:
				case Format.BC1_UNorm:
				case Format.BC1_UNorm_SRgb:
				case Format.BC4_Typeless:
				case Format.BC4_UNorm:
				case Format.BC4_SNorm:
					return 4;
				case Format.BC2_Typeless:
				case Format.BC2_UNorm:
				case Format.BC2_UNorm_SRgb:
				case Format.BC3_Typeless:
				case Format.BC3_UNorm:
				case Format.BC3_UNorm_SRgb:
				case Format.BC5_Typeless:
				case Format.BC5_UNorm:
				case Format.BC5_SNorm:
				case Format.BC6H_Typeless:
				case Format.BC6H_Uf16:
				case Format.BC6H_Sf16:
				case Format.BC7_Typeless:
				case Format.BC7_UNorm:
				case Format.BC7_UNorm_SRgb:
					return 8;
				default:
					throw new InvalidOperationException("Unknown DDS format.");
			}
		}
	}
}