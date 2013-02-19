// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// The following code is a port of DirectXTex http://directxtex.codeplex.com
// -----------------------------------------------------------------------------
// Microsoft Public License (Ms-PL)
//
// This license governs use of the accompanying software. If you use the 
// software, you accept this license. If you do not accept the license, do not
// use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and 
// "distribution" have the same meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to 
// the software.
// A "contributor" is any person that distributes its contribution under this 
// license.
// "Licensed patents" are a contributor's patent claims that read directly on 
// its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the 
// license conditions and limitations in section 3, each contributor grants 
// you a non-exclusive, worldwide, royalty-free copyright license to reproduce
// its contribution, prepare derivative works of its contribution, and 
// distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license
// conditions and limitations in section 3, each contributor grants you a 
// non-exclusive, worldwide, royalty-free license under its licensed patents to
// make, have made, use, sell, offer for sale, import, and/or otherwise dispose
// of its contribution in the software or derivative works of the contribution 
// in the software.
//
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any 
// contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that 
// you claim are infringed by the software, your patent license from such 
// contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all 
// copyright, patent, trademark, and attribution notices that are present in the
// software.
// (D) If you distribute any portion of the software in source code form, you 
// may do so only under this license by including a complete copy of this 
// license with your distribution. If you distribute any portion of the software
// in compiled or object code form, you may only do so under a license that 
// complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The
// contributors give no express warranties, guarantees or conditions. You may
// have additional consumer rights under your local laws which this license 
// cannot change. To the extent permitted under your local laws, the 
// contributors exclude the implied warranties of merchantability, fitness for a
// particular purpose and non-infringement.

using System;

#pragma warning disable 0675

namespace Pegasus.AssetsCompiler.DDS
{
	using System.Diagnostics;
	using System.IO;
	using System.Runtime.InteropServices;
	using Framework.Platform.Graphics;
	using SharpDX;
	using SharpDX.DXGI;
	using SharpDX.Direct3D11;
	using SharpDX.Multimedia;

	internal class DDSReader
	{
		[Flags]
		public enum ConversionFlags
		{
			None = 0x0,
			Expand = 0x1, // Conversion requires expanded pixel size
			NoAlpha = 0x2, // Conversion requires setting alpha to known value
			Swizzle = 0x4, // BGR/RGB order swizzling required
			Pal8 = 0x8, // Has an 8-bit palette
			Format888 = 0x10, // Source is an 8:8:8 (24bpp) format
			Format565 = 0x20, // Source is a 5:6:5 (16bpp) format
			Format5551 = 0x40, // Source is a 5:5:5:1 (16bpp) format
			Format4444 = 0x80, // Source is a 4:4:4:4 (16bpp) format
			Format44 = 0x100, // Source is a 4:4 (8bpp) format
			Format332 = 0x200, // Source is a 3:3:2 (8bpp) format
			Format8332 = 0x400, // Source is a 8:3:3:2 (16bpp) format
			FormatA8P8 = 0x800, // Has an 8-bit palette with an alpha channel
			CopyMemory = 0x1000, // The content of the memory passed to the DDS Loader is copied to another internal buffer.
			DX10 = 0x10000, // Has the 'DX10' extension header
		};

		/// <summary>
		///   Magic code to identify DDS header
		/// </summary>
		public const uint MagicHeader = 0x20534444; // "DDS "

		private static readonly LegacyMap[] LegacyMaps = new[]
		{
			new LegacyMap(Format.BC1_UNorm, ConversionFlags.None, DDSPixelFormat.DXT1), // D3DFMT_DXT1
			new LegacyMap(Format.BC2_UNorm, ConversionFlags.None, DDSPixelFormat.DXT3), // D3DFMT_DXT3
			new LegacyMap(Format.BC3_UNorm, ConversionFlags.None, DDSPixelFormat.DXT5), // D3DFMT_DXT5

			new LegacyMap(Format.BC2_UNorm, ConversionFlags.None, DDSPixelFormat.DXT2), // D3DFMT_DXT2 (ignore premultiply)
			new LegacyMap(Format.BC3_UNorm, ConversionFlags.None, DDSPixelFormat.DXT4), // D3DFMT_DXT4 (ignore premultiply)

			new LegacyMap(Format.BC4_UNorm, ConversionFlags.None, DDSPixelFormat.BC4_UNorm),
			new LegacyMap(Format.BC4_SNorm, ConversionFlags.None, DDSPixelFormat.BC4_SNorm),
			new LegacyMap(Format.BC5_UNorm, ConversionFlags.None, DDSPixelFormat.BC5_UNorm),
			new LegacyMap(Format.BC5_SNorm, ConversionFlags.None, DDSPixelFormat.BC5_SNorm),
			new LegacyMap(Format.BC4_UNorm, ConversionFlags.None,
						  new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('A', 'T', 'I', '1'), 0, 0, 0, 0, 0)),
			new LegacyMap(Format.BC5_UNorm, ConversionFlags.None,
						  new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('A', 'T', 'I', '2'), 0, 0, 0, 0, 0)),
			new LegacyMap(Format.R8G8_B8G8_UNorm, ConversionFlags.None, DDSPixelFormat.R8G8_B8G8), // D3DFMT_R8G8_B8G8
			new LegacyMap(Format.G8R8_G8B8_UNorm, ConversionFlags.None, DDSPixelFormat.G8R8_G8B8), // D3DFMT_G8R8_G8B8

			new LegacyMap(Format.B8G8R8A8_UNorm, ConversionFlags.None, DDSPixelFormat.A8R8G8B8),
			// D3DFMT_A8R8G8B8 (uses DXGI 1.1 format)
			new LegacyMap(Format.B8G8R8X8_UNorm, ConversionFlags.None, DDSPixelFormat.X8R8G8B8),
			// D3DFMT_X8R8G8B8 (uses DXGI 1.1 format)
			new LegacyMap(Format.R8G8B8A8_UNorm, ConversionFlags.None, DDSPixelFormat.A8B8G8R8), // D3DFMT_A8B8G8R8
			new LegacyMap(Format.R8G8B8A8_UNorm, ConversionFlags.NoAlpha, DDSPixelFormat.X8B8G8R8), // D3DFMT_X8B8G8R8
			new LegacyMap(Format.R16G16_UNorm, ConversionFlags.None, DDSPixelFormat.G16R16), // D3DFMT_G16R16

			new LegacyMap(Format.R10G10B10A2_UNorm, ConversionFlags.Swizzle,
						  new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 32, 0x000003ff, 0x000ffc00, 0x3ff00000, 0xc0000000)),
			// D3DFMT_A2R10G10B10 (D3DX reversal issue workaround)
			new LegacyMap(Format.R10G10B10A2_UNorm, ConversionFlags.None,
						  new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 32, 0x3ff00000, 0x000ffc00, 0x000003ff, 0xc0000000)),
			// D3DFMT_A2B10G10R10 (D3DX reversal issue workaround)

			new LegacyMap(Format.R8G8B8A8_UNorm, ConversionFlags.Expand
												 | ConversionFlags.NoAlpha
												 | ConversionFlags.Format888, DDSPixelFormat.R8G8B8), // D3DFMT_R8G8B8

			new LegacyMap(Format.B5G6R5_UNorm, ConversionFlags.Format565, DDSPixelFormat.R5G6B5), // D3DFMT_R5G6B5
			new LegacyMap(Format.B5G5R5A1_UNorm, ConversionFlags.Format5551, DDSPixelFormat.A1R5G5B5), // D3DFMT_A1R5G5B5
			new LegacyMap(Format.B5G5R5A1_UNorm, ConversionFlags.Format5551
												 | ConversionFlags.NoAlpha, new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 16, 0x7c00, 0x03e0, 0x001f, 0x0000)),
			// D3DFMT_X1R5G5B5
     
			new LegacyMap(Format.R8G8B8A8_UNorm, ConversionFlags.Expand
												 | ConversionFlags.Format8332, new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 16, 0x00e0, 0x001c, 0x0003, 0xff00)),
			// D3DFMT_A8R3G3B2
			new LegacyMap(Format.B5G6R5_UNorm, ConversionFlags.Expand
											   | ConversionFlags.Format332, new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 8, 0xe0, 0x1c, 0x03, 0x00)),
			// D3DFMT_R3G3B2
  
			new LegacyMap(Format.R8_UNorm, ConversionFlags.None, DDSPixelFormat.L8), // D3DFMT_L8
			new LegacyMap(Format.R16_UNorm, ConversionFlags.None, DDSPixelFormat.L16), // D3DFMT_L16
			new LegacyMap(Format.R8G8_UNorm, ConversionFlags.None, DDSPixelFormat.A8L8), // D3DFMT_A8L8

			new LegacyMap(Format.A8_UNorm, ConversionFlags.None, DDSPixelFormat.A8), // D3DFMT_A8

			new LegacyMap(Format.R16G16B16A16_UNorm, ConversionFlags.None,
						  new DDSPixelFormat(PixelFormatFlags.FourCC, 36, 0, 0, 0, 0, 0))
			, // D3DFMT_A16B16G16R16
			new LegacyMap(Format.R16G16B16A16_SNorm, ConversionFlags.None,
						  new DDSPixelFormat(PixelFormatFlags.FourCC, 110, 0, 0, 0, 0, 0)), // D3DFMT_Q16W16V16U16
			new LegacyMap(Format.R16_Float, ConversionFlags.None, new DDSPixelFormat(PixelFormatFlags.FourCC, 111, 0, 0, 0, 0, 0)),
			// D3DFMT_R16F
			new LegacyMap(Format.R16G16_Float, ConversionFlags.None, new DDSPixelFormat(PixelFormatFlags.FourCC, 112, 0, 0, 0, 0, 0)),
			// D3DFMT_G16R16F
			new LegacyMap(Format.R16G16B16A16_Float, ConversionFlags.None,
						  new DDSPixelFormat(PixelFormatFlags.FourCC, 113, 0, 0, 0, 0, 0)), // D3DFMT_A16B16G16R16F
			new LegacyMap(Format.R32_Float, ConversionFlags.None, new DDSPixelFormat(PixelFormatFlags.FourCC, 114, 0, 0, 0, 0, 0)),
			// D3DFMT_R32F
			new LegacyMap(Format.R32G32_Float, ConversionFlags.None, new DDSPixelFormat(PixelFormatFlags.FourCC, 115, 0, 0, 0, 0, 0)),
			// D3DFMT_G32R32F
			new LegacyMap(Format.R32G32B32A32_Float, ConversionFlags.None,
						  new DDSPixelFormat(PixelFormatFlags.FourCC, 116, 0, 0, 0, 0, 0)), // D3DFMT_A32B32G32R32F

			new LegacyMap(Format.R32_Float, ConversionFlags.None,
						  new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 32, 0xffffffff, 0x00000000, 0x00000000, 0x00000000)),
			// D3DFMT_R32F (D3DX uses FourCC 114 instead)

			new LegacyMap(Format.R8G8B8A8_UNorm, ConversionFlags.Expand
												 | ConversionFlags.Pal8
												 | ConversionFlags.FormatA8P8, new DDSPixelFormat(PixelFormatFlags.Pal8, 0, 16, 0, 0, 0, 0)), // D3DFMT_A8P8
			new LegacyMap(Format.R8G8B8A8_UNorm, ConversionFlags.Expand
												 | ConversionFlags.Pal8, new DDSPixelFormat(PixelFormatFlags.Pal8, 0, 8, 0, 0, 0, 0)), // D3DFMT_P8
#if DIRECTX11_1
    new LegacyMap( DXGI.Format.B4G4R4A4_UNorm,     ConversionFlags.Format4444,        DDS.PixelFormat.A4R4G4B4 ), // D3DFMT_A4R4G4B4 (uses DXGI 1.2 format)
    new LegacyMap( DXGI.Format.B4G4R4A4_UNorm,     ConversionFlags.NoAlpha
                                      | ConversionFlags.Format4444,      new DDS.PixelFormat(DDS.PixelFormatFlags.Rgb,       0, 16, 0x0f00,     0x00f0,     0x000f,     0x0000     ) ), // D3DFMT_X4R4G4B4 (uses DXGI 1.2 format)
    new LegacyMap( DXGI.Format.B4G4R4A4_UNorm,     ConversionFlags.Expand
                                      | ConversionFlags.Format44,        new DDS.PixelFormat(DDS.PixelFormatFlags.Luminance, 0,  8, 0x0f,       0x00,       0x00,       0xf0       ) ), // D3DFMT_A4L4 (uses DXGI 1.2 format)
#else
			// !DXGI_1_2_FORMATS
			new LegacyMap(Format.R8G8B8A8_UNorm, ConversionFlags.Expand
												 | ConversionFlags.Format4444, DDSPixelFormat.A4R4G4B4), // D3DFMT_A4R4G4B4
			new LegacyMap(Format.R8G8B8A8_UNorm, ConversionFlags.Expand
												 | ConversionFlags.NoAlpha
												 | ConversionFlags.Format4444, new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 16, 0x0f00, 0x00f0, 0x000f, 0x0000)),
			// D3DFMT_X4R4G4B4
			new LegacyMap(Format.R8G8B8A8_UNorm, ConversionFlags.Expand
												 | ConversionFlags.Format44, new DDSPixelFormat(PixelFormatFlags.Luminance, 0, 8, 0x0f, 0x00, 0x00, 0xf0)),
			// D3DFMT_A4L4
#endif
		};

		// Note that many common DDS reader/writers (including D3DX) swap the
		// the RED/BLUE masks for 10:10:10:2 formats. We assumme
		// below that the 'backwards' header mask is being used since it is most
		// likely written by D3DX. The more robust solution is to use the 'DX10'
		// header extension and specify the Format.R10G10B10A2_UNorm format directly

		// We do not support the following legacy Direct3D 9 formats:
		//      BumpDuDv D3DFMT_V8U8, D3DFMT_Q8W8V8U8, D3DFMT_V16U16, D3DFMT_A2W10V10U10
		//      BumpLuminance D3DFMT_L6V5U5, D3DFMT_X8L8V8U8
		//      FourCC "UYVY" D3DFMT_UYVY
		//      FourCC "YUY2" D3DFMT_YUY2
		//      FourCC 117 D3DFMT_CxV8U8
		//      ZBuffer D3DFMT_D16_LOCKABLE
		//      FourCC 82 D3DFMT_D32F_LOCKABLE
		private static Format GetDXGIFormat(ref DDSPixelFormat pixelFormat, DDSFlags flags, out ConversionFlags conversionFlags)
		{
			conversionFlags = ConversionFlags.None;

			int index = 0;
			for (index = 0; index < LegacyMaps.Length; ++index)
			{
				var entry = LegacyMaps[index];

				if ((pixelFormat.Flags & entry.PixelFormat.Flags) != 0)
				{
					if ((entry.PixelFormat.Flags & PixelFormatFlags.FourCC) != 0)
					{
						if (pixelFormat.FourCC == entry.PixelFormat.FourCC)
							break;
					}
					else if ((entry.PixelFormat.Flags & PixelFormatFlags.Pal8) != 0)
					{
						if (pixelFormat.RGBBitCount == entry.PixelFormat.RGBBitCount)
							break;
					}
					else if (pixelFormat.RGBBitCount == entry.PixelFormat.RGBBitCount)
					{
						// RGB, RGBA, ALPHA, LUMINANCE
						if (pixelFormat.RBitMask == entry.PixelFormat.RBitMask
							&& pixelFormat.GBitMask == entry.PixelFormat.GBitMask
							&& pixelFormat.BBitMask == entry.PixelFormat.BBitMask
							&& pixelFormat.ABitMask == entry.PixelFormat.ABitMask)
							break;
					}
				}
			}

			if (index >= LegacyMaps.Length)
				return Format.Unknown;

			conversionFlags = LegacyMaps[index].ConversionFlags;
			var format = LegacyMaps[index].Format;

			if ((conversionFlags & ConversionFlags.Expand) != 0 && (flags & DDSFlags.NoLegacyExpansion) != 0)
				return Format.Unknown;

			if ((format == Format.R10G10B10A2_UNorm) && (flags & DDSFlags.NoR10B10G10A2Fixup) != 0)
			{
				conversionFlags ^= ConversionFlags.Swizzle;
			}

			return format;
		}

		/// <summary>
		///   Decodes DDS header including optional DX10 extended header
		/// </summary>
		/// <param name="headerPtr">Pointer to the DDS header.</param>
		/// <param name="size">Size of the DDS content.</param>
		/// <param name="flags">Flags used for decoding the DDS header.</param>
		/// <param name="description">Output texture description.</param>
		/// <param name="convFlags">Output conversion flags.</param>
		/// <exception cref="ArgumentException">If the argument headerPtr is null</exception>
		/// <exception cref="InvalidOperationException">If the DDS header contains invalid datas.</exception>
		/// <returns>True if the decoding is successfull, false if this is not a DDS header.</returns>
		private static unsafe bool DecodeDDSHeader(IntPtr headerPtr, int size, DDSFlags flags, out ImageDescription description,
												   out ConversionFlags convFlags)
		{
			description = new ImageDescription();
			convFlags = ConversionFlags.None;

			if (headerPtr == IntPtr.Zero)
				throw new ArgumentException("Pointer to DDS header cannot be null", "headerPtr");

			if (size < (Utilities.SizeOf<Header>() + sizeof(uint)))
				return false;

			// DDS files always start with the same magic number ("DDS ")
			if (*(uint*)(headerPtr) != MagicHeader)
				return false;

			var header = *(Header*)((byte*)headerPtr + sizeof(int));

			// Verify header to validate DDS file
			if (header.Size != Utilities.SizeOf<Header>() || header.PixelFormat.Size != Utilities.SizeOf<DDSPixelFormat>())
				return false;

			// Setup MipLevels
			description.MipLevels = header.MipMapCount;
			if (description.MipLevels == 0)
				description.MipLevels = 1;

			// Check for DX10 extension
			if ((header.PixelFormat.Flags & PixelFormatFlags.FourCC) != 0 &&
				(new FourCC('D', 'X', '1', '0') == header.PixelFormat.FourCC))
			{
				// Buffer must be big enough for both headers and magic value
				if (size < (Utilities.SizeOf<Header>() + sizeof(uint) + Utilities.SizeOf<HeaderDXT10>()))
					return false;

				var headerDX10 = *(HeaderDXT10*)((byte*)headerPtr + sizeof(int) + Utilities.SizeOf<Header>());
				convFlags |= ConversionFlags.DX10;

				description.ArraySize = headerDX10.ArraySize;
				if (description.ArraySize == 0)
					throw new InvalidOperationException("Unexpected ArraySize == 0 from DDS HeaderDX10 ");

				description.Format = headerDX10.DXGIFormat;
				if (!FormatHelper.IsValid(description.Format))
					throw new InvalidOperationException("Invalid Format from DDS HeaderDX10 ");

				switch (headerDX10.ResourceDimension)
				{
					case ResourceDimension.Texture1D:

						// D3DX writes 1D textures with a fixed Height of 1
						if ((header.Flags & HeaderFlags.Height) != 0 && header.Height != 1)
							throw new InvalidOperationException("Unexpected Height != 1 from DDS HeaderDX10 ");

						description.Width = header.Width;
						description.Height = 1;
						description.Depth = 1;
						description.Dimension = TextureType.Texture1D;
						break;

					case ResourceDimension.Texture2D:
						if ((headerDX10.MiscFlags & ResourceOptionFlags.TextureCube) != 0)
						{
							description.ArraySize *= 6;
							description.Dimension = TextureType.CubeMap;
						}
						else
						{
							description.Dimension = TextureType.Texture2D;
						}

						description.Width = header.Width;
						description.Height = header.Height;
						description.Depth = 1;
						break;

					case ResourceDimension.Texture3D:
						if ((header.Flags & HeaderFlags.Volume) == 0)
							throw new InvalidOperationException("Texture3D missing HeaderFlags.Volume from DDS HeaderDX10");

						if (description.ArraySize > 1)
							throw new InvalidOperationException("Unexpected ArraySize > 1 for Texture3D from DDS HeaderDX10");

						description.Width = header.Width;
						description.Height = header.Height;
						description.Depth = header.Depth;
						description.Dimension = TextureType.Texture3D;
						break;

					default:
						throw new InvalidOperationException(string.Format("Unexpected dimension [{0}] from DDS HeaderDX10",
																		  headerDX10.ResourceDimension));
				}
			}
			else
			{
				description.ArraySize = 1;

				if ((header.Flags & HeaderFlags.Volume) != 0)
				{
					description.Width = header.Width;
					description.Height = header.Height;
					description.Depth = header.Depth;
					description.Dimension = TextureType.Texture3D;
				}
				else
				{
					if ((header.CubemapFlags & CubemapFlags.CubeMap) != 0)
					{
						// We require all six faces to be defined
						if ((header.CubemapFlags & CubemapFlags.AllFaces) != CubemapFlags.AllFaces)
							throw new InvalidOperationException("Unexpected CubeMap, expecting all faces from DDS Header");

						description.ArraySize = 6;
						description.Dimension = TextureType.CubeMap;
					}
					else
					{
						description.Dimension = TextureType.Texture2D;
					}

					description.Width = header.Width;
					description.Height = header.Height;
					description.Depth = 1;
					// Note there's no way for a legacy Direct3D 9 DDS to express a '1D' texture
				}

				description.Format = GetDXGIFormat(ref header.PixelFormat, flags, out convFlags);

				if (description.Format == Format.Unknown)
					throw new InvalidOperationException("Unsupported PixelFormat from DDS Header");
			}

			// Special flag for handling BGR DXGI 1.1 formats
			if ((flags & DDSFlags.ForceRgb) != 0)
			{
				switch (description.Format)
				{
					case Format.B8G8R8A8_UNorm:
						description.Format = Format.R8G8B8A8_UNorm;
						convFlags |= ConversionFlags.Swizzle;
						break;

					case Format.B8G8R8X8_UNorm:
						description.Format = Format.R8G8B8A8_UNorm;
						convFlags |= ConversionFlags.Swizzle | ConversionFlags.NoAlpha;
						break;

					case Format.B8G8R8A8_Typeless:
						description.Format = Format.R8G8B8A8_Typeless;
						convFlags |= ConversionFlags.Swizzle;
						break;

					case Format.B8G8R8A8_UNorm_SRgb:
						description.Format = Format.R8G8B8A8_UNorm_SRgb;
						convFlags |= ConversionFlags.Swizzle;
						break;

					case Format.B8G8R8X8_Typeless:
						description.Format = Format.R8G8B8A8_Typeless;
						convFlags |= ConversionFlags.Swizzle | ConversionFlags.NoAlpha;
						break;

					case Format.B8G8R8X8_UNorm_SRgb:
						description.Format = Format.R8G8B8A8_UNorm_SRgb;
						convFlags |= ConversionFlags.Swizzle | ConversionFlags.NoAlpha;
						break;
				}
			}

			// Pass DDSFlags copy memory to the conversion flags
			if ((flags & DDSFlags.CopyMemory) != 0)
				convFlags |= ConversionFlags.CopyMemory;

			// Special flag for handling 16bpp formats
			if ((flags & DDSFlags.No16Bpp) != 0)
			{
				switch (description.Format)
				{
					case Format.B5G6R5_UNorm:
					case Format.B5G5R5A1_UNorm:
#if DIRECTX11_1
                    case Format.B4G4R4A4_UNorm:
#endif
						description.Format = Format.R8G8B8A8_UNorm;
						convFlags |= ConversionFlags.Expand;
						if (description.Format == Format.B5G6R5_UNorm)
							convFlags |= ConversionFlags.NoAlpha;
						break;
				}
			}
			return true;
		}

		// Converts an image row with optional clearing of alpha value to 1.0
		/// <summary>
		/// </summary>
		/// <param name="pDestination"></param>
		/// <param name="outSize"></param>
		/// <param name="outFormat"></param>
		/// <param name="pSource"></param>
		/// <param name="inSize"></param>
		/// <param name="inFormat"></param>
		/// <param name="pal8"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		private static unsafe bool LegacyExpandScanline(IntPtr pDestination, int outSize, Format outFormat,
														IntPtr pSource, int inSize, TEXP_LEGACY_FORMAT inFormat,
														int* pal8, ScanlineFlags flags)
		{
			switch (inFormat)
			{
				case TEXP_LEGACY_FORMAT.R8G8B8:
					if (outFormat != Format.R8G8B8A8_UNorm)
						return false;

					// D3DFMT_R8G8B8 -> Format.R8G8B8A8_UNorm
				{
					var sPtr = (byte*)(pSource);
					var dPtr = (int*)(pDestination);

					for (int ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); icount += 3, ocount += 4)
					{
						// 24bpp Direct3D 9 files are actually BGR, so need to swizzle as well
						int t1 = (*(sPtr) << 16);
						int t2 = (*(sPtr + 1) << 8);
						int t3 = *(sPtr + 2);

						*(dPtr++) = (int)(t1 | t2 | t3 | 0xff000000);
						sPtr += 3;
					}
				}
					return true;

				case TEXP_LEGACY_FORMAT.R3G3B2:
					switch (outFormat)
					{
						case Format.R8G8B8A8_UNorm:
							// D3DFMT_R3G3B2 -> Format.R8G8B8A8_UNorm
						{
							var sPtr = (byte*)(pSource);
							var dPtr = (int*)(pDestination);

							for (int ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); ++icount, ocount += 4)
							{
								byte t = *(sPtr++);

								int t1 = (t & 0xe0) | ((t & 0xe0) >> 3) | ((t & 0xc0) >> 6);
								int t2 = ((t & 0x1c) << 11) | ((t & 0x1c) << 8) | ((t & 0x18) << 5);
								int t3 = ((t & 0x03) << 22) | ((t & 0x03) << 20) | ((t & 0x03) << 18) | ((t & 0x03) << 16);

								*(dPtr++) = (int)(t1 | t2 | t3 | 0xff000000);
							}
						}
							return true;

						case Format.B5G6R5_UNorm:
							// D3DFMT_R3G3B2 -> Format.B5G6R5_UNorm
						{
							var sPtr = (byte*)(pSource);
							var dPtr = (short*)(pDestination);

							for (int ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); ++icount, ocount += 2)
							{
								byte t = *(sPtr++);

								var t1 = (short)(((t & 0xe0) << 8) | ((t & 0xc0) << 5));
								var t2 = (short)(((t & 0x1c) << 6) | ((t & 0x1c) << 3));
								var t3 = (short)(((t & 0x03) << 3) | ((t & 0x03) << 1) | ((t & 0x02) >> 1));

								*(dPtr++) = (short)(t1 | t2 | t3);
							}
						}
							return true;
					}
					break;

				case TEXP_LEGACY_FORMAT.A8R3G3B2:
					if (outFormat != Format.R8G8B8A8_UNorm)
						return false;

					// D3DFMT_A8R3G3B2 -> Format.R8G8B8A8_UNorm
				{
					var sPtr = (short*)(pSource);
					var dPtr = (int*)(pDestination);

					for (int ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); icount += 2, ocount += 4)
					{
						short t = *(sPtr++);

						int t1 = (t & 0x00e0) | ((t & 0x00e0) >> 3) | ((t & 0x00c0) >> 6);
						int t2 = ((t & 0x001c) << 11) | ((t & 0x001c) << 8) | ((t & 0x0018) << 5);
						int t3 = ((t & 0x0003) << 22) | ((t & 0x0003) << 20) | ((t & 0x0003) << 18) | ((t & 0x0003) << 16);
						uint ta = ((flags & ScanlineFlags.SetAlpha) != 0 ? 0xff000000 : (uint)((t & 0xff00) << 16));

						*(dPtr++) = (int)(t1 | t2 | t3 | ta);
					}
				}
					return true;

				case TEXP_LEGACY_FORMAT.P8:
					if ((outFormat != Format.R8G8B8A8_UNorm) || pal8 == null)
						return false;

					// D3DFMT_P8 -> Format.R8G8B8A8_UNorm
				{
					byte* sPtr = (byte*)(pSource);
					int* dPtr = (int*)(pDestination);

					for (int ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); ++icount, ocount += 4)
					{
						byte t = *(sPtr++);

						*(dPtr++) = pal8[t];
					}
				}
					return true;

				case TEXP_LEGACY_FORMAT.A8P8:
					if ((outFormat != Format.R8G8B8A8_UNorm) || pal8 == null)
						return false;

					// D3DFMT_A8P8 -> Format.R8G8B8A8_UNorm
				{
					short* sPtr = (short*)(pSource);
					int* dPtr = (int*)(pDestination);

					for (int ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); icount += 2, ocount += 4)
					{
						short t = *(sPtr++);

						int t1 = pal8[t & 0xff];
						uint ta = ((flags & ScanlineFlags.SetAlpha) != 0 ? 0xff000000 : (uint)((t & 0xff00) << 16));

						*(dPtr++) = (int)(t1 | ta);
					}
				}
					return true;

				case TEXP_LEGACY_FORMAT.A4L4:
					switch (outFormat)
					{
#if DIRECTX11_1
                case Format.B4G4R4A4_UNorm :
                    // D3DFMT_A4L4 -> Format.B4G4R4A4_UNorm 
                    {
                        byte * sPtr = (byte*)(pSource);
                        short * dPtr = (short*)(pDestination);

                        for( int ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); ++icount, ocount += 2 )
                        {
                            byte t = *(sPtr++);

                            short t1 = (short)(t & 0x0f);
                            ushort ta = (flags & ScanlineFlags.SetAlpha ) != 0 ?  (ushort)0xf000 : (ushort)((t & 0xf0) << 8);

                            *(dPtr++) = (short)(t1 | (t1 << 4) | (t1 << 8) | ta);
                        }
                    }
                    return true;
#endif
							// DXGI_1_2_FORMATS

						case Format.R8G8B8A8_UNorm:
							// D3DFMT_A4L4 -> Format.R8G8B8A8_UNorm
						{
							byte* sPtr = (byte*)(pSource);
							int* dPtr = (int*)(pDestination);

							for (int ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); ++icount, ocount += 4)
							{
								byte t = *(sPtr++);

								int t1 = ((t & 0x0f) << 4) | (t & 0x0f);
								uint ta = ((flags & ScanlineFlags.SetAlpha) != 0 ? 0xff000000 : (uint)(((t & 0xf0) << 24) | ((t & 0xf0) << 20)));

								*(dPtr++) = (int)(t1 | (t1 << 8) | (t1 << 16) | ta);
							}
						}
							return true;
					}
					break;

#if !DIRECTX11_1
				case TEXP_LEGACY_FORMAT.B4G4R4A4:
					if (outFormat != Format.R8G8B8A8_UNorm)
						return false;

					// D3DFMT_A4R4G4B4 -> Format.R8G8B8A8_UNorm
				{
					short* sPtr = (short*)(pSource);
					int* dPtr = (int*)(pDestination);

					for (int ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); icount += 2, ocount += 4)
					{
						short t = *(sPtr++);

						int t1 = ((t & 0x0f00) >> 4) | ((t & 0x0f00) >> 8);
						int t2 = ((t & 0x00f0) << 8) | ((t & 0x00f0) << 4);
						int t3 = ((t & 0x000f) << 20) | ((t & 0x000f) << 16);
						uint ta = ((flags & ScanlineFlags.SetAlpha) != 0 ? 0xff000000 : (uint)(((t & 0xf000) << 16) | ((t & 0xf000) << 12)));

						*(dPtr++) = (int)(t1 | t2 | t3 | ta);
					}
				}
					return true;
#endif
			}
			return false;
		}

		/// <summary>
		///   Load a DDS file in memory
		/// </summary>
		/// <param name="pSource"></param>
		/// <param name="size"></param>
		/// <param name="handle"></param>
		/// <returns></returns>
		public static unsafe DDSImage LoadFromDDSMemory(IntPtr pSource, int size, bool makeACopy, GCHandle? handle)
		{
			var flags = makeACopy ? DDSFlags.CopyMemory : DDSFlags.None;

			ConversionFlags convFlags;
			ImageDescription mdata;
			// If the memory pointed is not a DDS memory, return null.
			if (!DecodeDDSHeader(pSource, size, flags, out mdata, out convFlags))
				return null;

			int offset = sizeof(uint) + Utilities.SizeOf<Header>();
			if ((convFlags & ConversionFlags.DX10) != 0)
				offset += Utilities.SizeOf<HeaderDXT10>();

			var pal8 = (int*)0;
			if ((convFlags & ConversionFlags.Pal8) != 0)
			{
				pal8 = (int*)((byte*)(pSource) + offset);
				offset += (256 * sizeof(uint));
			}

			if (size < offset)
				throw new InvalidOperationException();

			var image = CreateImageFromDDS(pSource, offset, size - offset, mdata,
										   (flags & DDSFlags.LegacyDword) != 0 ? DDSImage.PitchFlags.LegacyDword : DDSImage.PitchFlags.None, convFlags, pal8,
										   handle);
			return image;
		}

		/// <summary>
		///   Converts or copies image data from pPixels into scratch image data
		/// </summary>
		/// <param name="pDDS"></param>
		/// <param name="offset"></param>
		/// <param name="size"></param>
		/// <param name="metadata"></param>
		/// <param name="cpFlags"></param>
		/// <param name="convFlags"></param>
		/// <param name="pal8"></param>
		/// <param name="handle"></param>
		/// <returns></returns>
		private static unsafe DDSImage CreateImageFromDDS(IntPtr pDDS, int offset, int size, ImageDescription metadata,
													   DDSImage.PitchFlags cpFlags, ConversionFlags convFlags, int* pal8, GCHandle? handle)
		{
			if ((convFlags & ConversionFlags.Expand) != 0)
			{
				if ((convFlags & ConversionFlags.Format888) != 0)
					cpFlags |= DDSImage.PitchFlags.Bpp24;
				else if ((convFlags &
						  (ConversionFlags.Format565 | ConversionFlags.Format5551 | ConversionFlags.Format4444 | ConversionFlags.Format8332 |
						   ConversionFlags.FormatA8P8)) != 0)
					cpFlags |= DDSImage.PitchFlags.Bpp16;
				else if ((convFlags & (ConversionFlags.Format44 | ConversionFlags.Format332 | ConversionFlags.Pal8)) != 0)
					cpFlags |= DDSImage.PitchFlags.Bpp8;
			}

			// If source image == dest image and no swizzle/alpha is required, we can return it as-is
			var isCopyNeeded = (convFlags & (ConversionFlags.Expand | ConversionFlags.CopyMemory)) != 0 ||
							   ((cpFlags & DDSImage.PitchFlags.LegacyDword) != 0);

			var image = new DDSImage(metadata, pDDS, offset, handle, !isCopyNeeded, cpFlags);

			// Size must be inferior to destination size.
			Debug.Assert(size <= image.TotalSizeInBytes);

			if (!isCopyNeeded && (convFlags & (ConversionFlags.Swizzle | ConversionFlags.NoAlpha)) == 0)
				return image;

			var imageDst = isCopyNeeded ? new DDSImage(metadata, IntPtr.Zero, 0, null, false) : image;

			var images = image.PixelBuffer;
			var imagesDst = imageDst.PixelBuffer;

			ScanlineFlags tflags = (convFlags & ConversionFlags.NoAlpha) != 0 ? ScanlineFlags.SetAlpha : ScanlineFlags.None;
			if ((convFlags & ConversionFlags.Swizzle) != 0)
				tflags |= ScanlineFlags.Legacy;

			int index = 0;

			int checkSize = size;

			for (int arrayIndex = 0; arrayIndex < metadata.ArraySize; arrayIndex++)
			{
				int d = metadata.Depth;
				// Else we need to go through each mips/depth slice to convert all scanlines.
				for (int level = 0; level < metadata.MipLevels; ++level)
				{
					for (int slice = 0; slice < d; ++slice, ++index)
					{
						IntPtr pSrc = images[index].DataPointer;
						IntPtr pDest = imagesDst[index].DataPointer;
						checkSize -= images[index].BufferStride;
						if (checkSize < 0)
							throw new InvalidOperationException("Unexpected end of buffer");

						if (FormatHelper.IsCompressed(metadata.Format))
						{
							Utilities.CopyMemory(pDest, pSrc, Math.Min(images[index].BufferStride, imagesDst[index].BufferStride));
						}
						else
						{
							int spitch = images[index].RowStride;
							int dpitch = imagesDst[index].RowStride;

							for (int h = 0; h < images[index].Height; ++h)
							{
								if ((convFlags & ConversionFlags.Expand) != 0)
								{
#if DIRECTX11_1
                                if ((convFlags & (ConversionFlags.Format565 | ConversionFlags.Format5551 | ConversionFlags.Format4444)) != 0)
#else
									if ((convFlags & (ConversionFlags.Format565 | ConversionFlags.Format5551)) != 0)
#endif
									{
										ExpandScanline(pDest, dpitch, pSrc, spitch,
													   (convFlags & ConversionFlags.Format565) != 0 ? Format.B5G6R5_UNorm : Format.B5G5R5A1_UNorm, tflags);
									}
									else
									{
										var lformat = FindLegacyFormat(convFlags);
										LegacyExpandScanline(pDest, dpitch, metadata.Format, pSrc, spitch, lformat, pal8, tflags);
									}
								}
								else if ((convFlags & ConversionFlags.Swizzle) != 0)
								{
									SwizzleScanline(pDest, dpitch, pSrc, spitch, metadata.Format, tflags);
								}
								else
								{
									if (pSrc != pDest)
										CopyScanline(pDest, dpitch, pSrc, spitch, metadata.Format, tflags);
								}

								pSrc = (IntPtr)((byte*)pSrc + spitch);
								pDest = (IntPtr)((byte*)pDest + dpitch);
							}
						}
					}

					if (d > 1)
						d >>= 1;
				}
			}

			// Return the imageDst or the original image
			if (isCopyNeeded)
			{
				image.Dispose();
				image = imageDst;
			}
			return image;
		}

		private static TEXP_LEGACY_FORMAT FindLegacyFormat(ConversionFlags flags)
		{
			var lformat = TEXP_LEGACY_FORMAT.UNKNOWN;

			if ((flags & ConversionFlags.Pal8) != 0)
			{
				lformat = (flags & ConversionFlags.FormatA8P8) != 0 ? TEXP_LEGACY_FORMAT.A8P8 : TEXP_LEGACY_FORMAT.P8;
			}
			else if ((flags & ConversionFlags.Format888) != 0)
				lformat = TEXP_LEGACY_FORMAT.R8G8B8;
			else if ((flags & ConversionFlags.Format332) != 0)
				lformat = TEXP_LEGACY_FORMAT.R3G3B2;
			else if ((flags & ConversionFlags.Format8332) != 0)
				lformat = TEXP_LEGACY_FORMAT.A8R3G3B2;
			else if ((flags & ConversionFlags.Format44) != 0)
				lformat = TEXP_LEGACY_FORMAT.A4L4;
#if DIRECTX11_1
            else if ((flags & ConversionFlags.Format4444) != 0)
                lformat = TEXP_LEGACY_FORMAT.B4G4R4A4;
#endif
			return lformat;
		}

		/// <summary>
		///   Converts an image row with optional clearing of alpha value to 1.0
		/// </summary>
		/// <param name="pDestination"></param>
		/// <param name="outSize"></param>
		/// <param name="pSource"></param>
		/// <param name="inSize"></param>
		/// <param name="inFormat"></param>
		/// <param name="flags"></param>
		private static unsafe void ExpandScanline(IntPtr pDestination, int outSize, IntPtr pSource, int inSize, Format inFormat,
												  ScanlineFlags flags)
		{
			switch (inFormat)
			{
				case Format.B5G6R5_UNorm:
					// DXGI.Format.B5G6R5_UNorm -> DXGI.Format.R8G8B8A8_UNorm
				{
					var sPtr = (ushort*)(pSource);
					var dPtr = (uint*)(pDestination);

					for (uint ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); icount += 2, ocount += 4)
					{
						ushort t = *(sPtr++);

						uint t1 = (uint)(((t & 0xf800) >> 8) | ((t & 0xe000) >> 13));
						uint t2 = (uint)(((t & 0x07e0) << 5) | ((t & 0x0600) >> 5));
						uint t3 = (uint)(((t & 0x001f) << 19) | ((t & 0x001c) << 14));

						*(dPtr++) = t1 | t2 | t3 | 0xff000000;
					}
				}
					break;

				case Format.B5G5R5A1_UNorm:
					// DXGI.Format.B5G5R5A1_UNorm -> DXGI.Format.R8G8B8A8_UNorm
				{
					var sPtr = (ushort*)(pSource);
					var dPtr = (uint*)(pDestination);

					for (uint ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); icount += 2, ocount += 4)
					{
						ushort t = *(sPtr++);

						uint t1 = (uint)(((t & 0x7c00) >> 7) | ((t & 0x7000) >> 12));
						uint t2 = (uint)(((t & 0x03e0) << 6) | ((t & 0x0380) << 1));
						uint t3 = (uint)(((t & 0x001f) << 19) | ((t & 0x001c) << 14));
						uint ta = ((flags & ScanlineFlags.SetAlpha) != 0 ? 0xff000000 : (((t & 0x8000) != 0 ? 0xff000000 : 0)));

						*(dPtr++) = t1 | t2 | t3 | ta;
					}
				}
					break;

#if DIRECTX11_1
                case DXGI.Format.B4G4R4A4_UNorm:
                    // DXGI.Format.B4G4R4A4_UNorm -> DXGI.Format.R8G8B8A8_UNorm
                    {
                        var sPtr = (ushort*) (pSource);
                        var dPtr = (uint*) (pDestination);

                        for (uint ocount = 0, icount = 0; ((icount < inSize) && (ocount < outSize)); icount += 2, ocount += 4)
                        {
                            ushort t = *(sPtr++);

                            uint t1 = (uint) (((t & 0x0f00) >> 4) | ((t & 0x0f00) >> 8));
                            uint t2 = (uint) (((t & 0x00f0) << 8) | ((t & 0x00f0) << 4));
                            uint t3 = (uint) (((t & 0x000f) << 20) | ((t & 0x000f) << 16));
                            uint ta = (flags & ScanlineFlags.SetAlpha) != 0 ? 0xff000000 : (uint) ((((t & 0xf000) << 16) | ((t & 0xf000) << 12)));

                            *(dPtr++) = t1 | t2 | t3 | ta;
                        }
                    }
                    break;
#endif
					// DXGI_1_2_FORMATS
			}
		}

		/// <summary>
		///   Copies an image row with optional clearing of alpha value to 1.0.
		/// </summary>
		/// <remarks>
		///   This method can be used in place as well, otherwise copies the image row unmodified.
		/// </remarks>
		/// <param name="pDestination">The destination buffer.</param>
		/// <param name="outSize">The destination size.</param>
		/// <param name="pSource">The source buffer.</param>
		/// <param name="inSize">The source size.</param>
		/// <param name="format">
		///   The <see cref="Format" /> of the source scanline.
		/// </param>
		/// <param name="flags">Scanline flags used when copying the scanline.</param>
		internal static unsafe void CopyScanline(IntPtr pDestination, int outSize, IntPtr pSource, int inSize, Format format,
												 ScanlineFlags flags)
		{
			if ((flags & ScanlineFlags.SetAlpha) != 0)
			{
				switch (format)
				{
						//-----------------------------------------------------------------------------
					case Format.R32G32B32A32_Typeless:
					case Format.R32G32B32A32_Float:
					case Format.R32G32B32A32_UInt:
					case Format.R32G32B32A32_SInt:
					{
						uint alpha;
						if (format == Format.R32G32B32A32_Float)
							alpha = 0x3f800000;
						else if (format == Format.R32G32B32A32_SInt)
							alpha = 0x7fffffff;
						else
							alpha = 0xffffffff;

						if (pDestination == pSource)
						{
							var dPtr = (uint*)(pDestination);
							for (int count = 0; count < outSize; count += 16)
							{
								dPtr += 3;
								*(dPtr++) = alpha;
							}
						}
						else
						{
							var sPtr = (uint*)(pSource);
							var dPtr = (uint*)(pDestination);
							int size = Math.Min(outSize, inSize);
							for (int count = 0; count < size; count += 16)
							{
								*(dPtr++) = *(sPtr++);
								*(dPtr++) = *(sPtr++);
								*(dPtr++) = *(sPtr++);
								*(dPtr++) = alpha;
								sPtr++;
							}
						}
					}
						return;

						//-----------------------------------------------------------------------------
					case Format.R16G16B16A16_Typeless:
					case Format.R16G16B16A16_Float:
					case Format.R16G16B16A16_UNorm:
					case Format.R16G16B16A16_UInt:
					case Format.R16G16B16A16_SNorm:
					case Format.R16G16B16A16_SInt:
					{
						ushort alpha;
						if (format == Format.R16G16B16A16_Float)
							alpha = 0x3c00;
						else if (format == Format.R16G16B16A16_SNorm || format == Format.R16G16B16A16_SInt)
							alpha = 0x7fff;
						else
							alpha = 0xffff;

						if (pDestination == pSource)
						{
							var dPtr = (ushort*)(pDestination);
							for (int count = 0; count < outSize; count += 8)
							{
								dPtr += 3;
								*(dPtr++) = alpha;
							}
						}
						else
						{
							var sPtr = (ushort*)(pSource);
							var dPtr = (ushort*)(pDestination);
							int size = Math.Min(outSize, inSize);
							for (int count = 0; count < size; count += 8)
							{
								*(dPtr++) = *(sPtr++);
								*(dPtr++) = *(sPtr++);
								*(dPtr++) = *(sPtr++);
								*(dPtr++) = alpha;
								sPtr++;
							}
						}
					}
						return;

						//-----------------------------------------------------------------------------
					case Format.R10G10B10A2_Typeless:
					case Format.R10G10B10A2_UNorm:
					case Format.R10G10B10A2_UInt:
					case Format.R10G10B10_Xr_Bias_A2_UNorm:
					{
						if (pDestination == pSource)
						{
							var dPtr = (uint*)(pDestination);
							for (int count = 0; count < outSize; count += 4)
							{
								*dPtr |= 0xC0000000;
								++dPtr;
							}
						}
						else
						{
							var sPtr = (uint*)(pSource);
							var dPtr = (uint*)(pDestination);
							int size = Math.Min(outSize, inSize);
							for (int count = 0; count < size; count += 4)
							{
								*(dPtr++) = *(sPtr++) | 0xC0000000;
							}
						}
					}
						return;

						//-----------------------------------------------------------------------------
					case Format.R8G8B8A8_Typeless:
					case Format.R8G8B8A8_UNorm:
					case Format.R8G8B8A8_UNorm_SRgb:
					case Format.R8G8B8A8_UInt:
					case Format.R8G8B8A8_SNorm:
					case Format.R8G8B8A8_SInt:
					case Format.B8G8R8A8_UNorm:
					case Format.B8G8R8A8_Typeless:
					case Format.B8G8R8A8_UNorm_SRgb:
					{
						uint alpha = (format == Format.R8G8B8A8_SNorm || format == Format.R8G8B8A8_SInt) ? 0x7f000000 : 0xff000000;

						if (pDestination == pSource)
						{
							var dPtr = (uint*)(pDestination);
							for (int count = 0; count < outSize; count += 4)
							{
								uint t = *dPtr & 0xFFFFFF;
								t |= alpha;
								*(dPtr++) = t;
							}
						}
						else
						{
							var sPtr = (uint*)(pSource);
							var dPtr = (uint*)(pDestination);
							int size = Math.Min(outSize, inSize);
							for (int count = 0; count < size; count += 4)
							{
								uint t = *(sPtr++) & 0xFFFFFF;
								t |= alpha;
								*(dPtr++) = t;
							}
						}
					}
						return;

						//-----------------------------------------------------------------------------
					case Format.B5G5R5A1_UNorm:
					{
						if (pDestination == pSource)
						{
							var dPtr = (ushort*)(pDestination);
							for (int count = 0; count < outSize; count += 2)
							{
								*(dPtr++) |= 0x8000;
							}
						}
						else
						{
							var sPtr = (ushort*)(pSource);
							var dPtr = (ushort*)(pDestination);
							int size = Math.Min(outSize, inSize);
							for (int count = 0; count < size; count += 2)
							{
								*(dPtr++) = (ushort)(*(sPtr++) | 0x8000);
							}
						}
					}
						return;

						//-----------------------------------------------------------------------------
					case Format.A8_UNorm:
						Utilities.ClearMemory(pDestination, 0xff, outSize);
						return;

#if DIRECTX11_1
	//-----------------------------------------------------------------------------
                    case Format.B4G4R4A4_UNorm:
                        {
                            if (pDestination == pSource)
                            {
                                var dPtr = (ushort*) (pDestination);
                                for (int count = 0; count < outSize; count += 2)
                                {
                                    *(dPtr++) |= 0xF000;
                                }
                            }
                            else
                            {
                                var sPtr = (ushort*) (pSource);
                                var dPtr = (ushort*) (pDestination);
                                int size = Math.Min(outSize, inSize);
                                for (int count = 0; count < size; count += 2)
                                {
                                    *(dPtr++) = (ushort) (*(sPtr++) | 0xF000);
                                }
                            }
                        }
                        return;
#endif
						// DXGI_1_2_FORMATS
				}
			}

			// Fall-through case is to just use memcpy (assuming this is not an in-place operation)
			if (pDestination == pSource)
				return;

			Utilities.CopyMemory(pDestination, pSource, Math.Min(outSize, inSize));
		}

		/// <summary>
		///   Swizzles (RGB &lt;-&gt; BGR) an image row with optional clearing of alpha value to 1.0.
		/// </summary>
		/// <param name="pDestination">The destination buffer.</param>
		/// <param name="outSize">The destination size.</param>
		/// <param name="pSource">The source buffer.</param>
		/// <param name="inSize">The source size.</param>
		/// <param name="format">
		///   The <see cref="Format" /> of the source scanline.
		/// </param>
		/// <param name="flags">Scanline flags used when copying the scanline.</param>
		/// <remarks>
		///   This method can be used in place as well, otherwise copies the image row unmodified.
		/// </remarks>
		internal static unsafe void SwizzleScanline(IntPtr pDestination, int outSize, IntPtr pSource, int inSize, Format format,
													ScanlineFlags flags)
		{
			switch (format)
			{
					//---------------------------------------------------------------------------------
				case Format.R10G10B10A2_Typeless:
				case Format.R10G10B10A2_UNorm:
				case Format.R10G10B10A2_UInt:
				case Format.R10G10B10_Xr_Bias_A2_UNorm:
					if ((flags & ScanlineFlags.Legacy) != 0)
					{
						// Swap Red (R) and Blue (B) channel (used for D3DFMT_A2R10G10B10 legacy sources)
						if (pDestination == pSource)
						{
							var dPtr = (uint*)(pDestination);
							for (int count = 0; count < outSize; count += 4)
							{
								uint t = *dPtr;

								uint t1 = (t & 0x3ff00000) >> 20;
								uint t2 = (t & 0x000003ff) << 20;
								uint t3 = (t & 0x000ffc00);
								uint ta = (flags & ScanlineFlags.SetAlpha) != 0 ? 0xC0000000 : (t & 0xC0000000);

								*(dPtr++) = t1 | t2 | t3 | ta;
							}
						}
						else
						{
							var sPtr = (uint*)(pSource);
							var dPtr = (uint*)(pDestination);
							int size = Math.Min(outSize, inSize);
							for (int count = 0; count < size; count += 4)
							{
								uint t = *(sPtr++);

								uint t1 = (t & 0x3ff00000) >> 20;
								uint t2 = (t & 0x000003ff) << 20;
								uint t3 = (t & 0x000ffc00);
								uint ta = (flags & ScanlineFlags.SetAlpha) != 0 ? 0xC0000000 : (t & 0xC0000000);

								*(dPtr++) = t1 | t2 | t3 | ta;
							}
						}
						return;
					}
					break;

					//---------------------------------------------------------------------------------
				case Format.R8G8B8A8_Typeless:
				case Format.R8G8B8A8_UNorm:
				case Format.R8G8B8A8_UNorm_SRgb:
				case Format.B8G8R8A8_UNorm:
				case Format.B8G8R8X8_UNorm:
				case Format.B8G8R8A8_Typeless:
				case Format.B8G8R8A8_UNorm_SRgb:
				case Format.B8G8R8X8_Typeless:
				case Format.B8G8R8X8_UNorm_SRgb:
					// Swap Red (R) and Blue (B) channels (used to convert from DXGI 1.1 BGR formats to DXGI 1.0 RGB)
					if (pDestination == pSource)
					{
						var dPtr = (uint*)(pDestination);
						for (int count = 0; count < outSize; count += 4)
						{
							uint t = *dPtr;

							uint t1 = (t & 0x00ff0000) >> 16;
							uint t2 = (t & 0x000000ff) << 16;
							uint t3 = (t & 0x0000ff00);
							uint ta = (flags & ScanlineFlags.SetAlpha) != 0 ? 0xff000000 : (t & 0xFF000000);

							*(dPtr++) = t1 | t2 | t3 | ta;
						}
					}
					else
					{
						var sPtr = (uint*)(pSource);
						var dPtr = (uint*)(pDestination);
						int size = Math.Min(outSize, inSize);
						for (int count = 0; count < size; count += 4)
						{
							uint t = *(sPtr++);

							uint t1 = (t & 0x00ff0000) >> 16;
							uint t2 = (t & 0x000000ff) << 16;
							uint t3 = (t & 0x0000ff00);
							uint ta = (flags & ScanlineFlags.SetAlpha) != 0 ? 0xff000000 : (t & 0xFF000000);

							*(dPtr++) = t1 | t2 | t3 | ta;
						}
					}
					return;
			}

			// Fall-through case is to just use memcpy (assuming this is not an in-place operation)
			if (pDestination == pSource)
				return;

			Utilities.CopyMemory(pDestination, pSource, Math.Min(outSize, inSize));
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct LegacyMap
		{
			/// <summary>
			///   Initializes a new instance of the <see cref="LegacyMap" /> struct.
			/// </summary>
			/// <param name="format">The format.</param>
			/// <param name="conversionFlags">The conversion flags.</param>
			/// <param name="pixelFormat">The pixel format.</param>
			public LegacyMap(Format format, ConversionFlags conversionFlags, DDSPixelFormat pixelFormat)
			{
				Format = format;
				ConversionFlags = conversionFlags;
				PixelFormat = pixelFormat;
			}

			public Format Format;
			public ConversionFlags ConversionFlags;
			public DDSPixelFormat PixelFormat;
		};

		[Flags]
		internal enum ScanlineFlags
		{
			None = 0,
			SetAlpha = 0x1, // Set alpha channel to known opaque value
			Legacy = 0x2, // Enables specific legacy format conversion cases
		};

		private enum TEXP_LEGACY_FORMAT
		{
			UNKNOWN = 0,
			R8G8B8,
			R3G3B2,
			A8R3G3B2,
			P8,
			A8P8,
			A4L4,
			B4G4R4A4,
		};
	}
}