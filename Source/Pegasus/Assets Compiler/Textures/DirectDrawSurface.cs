﻿namespace Pegasus.AssetsCompiler.Textures
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Utilities;

	/// <summary>
	///     Implements a subset of the DX10 DDS file specification based on the sample provided by Microsoft at
	///     http://msdn.microsoft.com/en-us/library/windows/apps/jj651550.aspx.
	/// </summary>
	internal class DirectDrawSurface
	{
		/// <summary>
		///     The magic DDS file code "DDS ".
		/// </summary>
		private const int MagicCode = 0x20534444;

		/// <summary>
		///     The texture description for the image.
		/// </summary>
		private readonly TextureDescription _description;

		/// <summary>
		///     The surfaces of the image.
		/// </summary>
		private readonly Surface[] _surfaces;

		/// <summary>
		///     The DDS file header.
		/// </summary>
		private Header _header;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="buffer">The buffer that contains the contents of the DDS file.</param>
		public unsafe DirectDrawSurface(byte[] buffer)
		{
			Assert.ArgumentNotNull(buffer);

			var reader = new BinaryReader(new MemoryStream(buffer));
			if (reader.ReadUInt32() != MagicCode)
				Log.Die("Not a DDS file.");

			_header = new Header(reader);
			if (_header.Size != 124 || _header.PixelFormat.Size != sizeof(PixelFormat))
				Log.Die("DDS file is corrupt.");

			if (!_header.PixelFormat.Flags.HasFlag(PixelFormatFlags.FourCC) ||
				_header.PixelFormat.FourCC != MakeFourCharacterCode('D', 'X', '1', '0'))
				Log.Die("DDS file is not in DX10 format.");

			if (_header.ArraySize == 0)
				Log.Die("An array size of 0 is invalid.");

			_description.Width = _header.Width;
			_description.Height = Math.Max(_header.Height, 1);
			_description.Depth = Math.Max(_header.Depth, 1);
			_description.Format = ToSurfaceFormat(_header.Format);
			_description.ArraySize = _header.ArraySize;
			_description.Type = GetTextureType();
			_description.Mipmaps = _header.MipMapCount;

			var faces = _description.ArraySize;
			if (_description.Type == TextureType.CubeMap)
				faces *= 6;

			_description.SurfaceCount = faces * _header.MipMapCount;
			_surfaces = new Surface[_description.SurfaceCount];

			for (int i = 0, index = 0; i < faces; ++i)
			{
				var width = _description.Width;
				var height = _description.Height;
				var depth = _description.Depth;

				for (var j = 0; j < _header.MipMapCount; ++j, ++index)
				{
					int size, stride;
					GetSurfaceInfo(width, height, _header.Format, out size, out stride);

					_surfaces[index] = new Surface
					{
						Width = width,
						Height = height,
						Depth = depth,
						Size = size,
						Stride = stride,
						Data = reader.ReadBytes((int)(depth * size))
					};

					width = Math.Max(width >> 1, 1);
					height = Math.Max(height >> 1, 1);
					depth = Math.Max(depth >> 1, 1);
				}
			}
		}

		/// <summary>
		///     Gets the texture description for the image.
		/// </summary>
		public TextureDescription Description
		{
			get { return _description; }
		}

		/// <summary>
		///     Gets the surfaces of the image.
		/// </summary>
		public IEnumerable<Surface> Surfaces
		{
			get { return _surfaces; }
		}

		/// <summary>
		///     Serializes the DDS image into the given buffer.
		/// </summary>
		/// <param name="writer">The writer the DDS image should be serialized into.</param>
		internal void Write(AssetWriter writer)
		{
			writer.WriteInt32(Description.Width);
			writer.WriteInt32(Description.Height);
			writer.WriteInt32(Description.Depth);
			writer.WriteInt32(Description.ArraySize);
			writer.WriteInt32((int)Description.Type);
			writer.WriteInt32((int)Description.Format);
			writer.WriteInt32(Description.Mipmaps);
			writer.WriteInt32(Description.SurfaceCount);

			foreach (var surface in Surfaces)
			{
				writer.WriteInt32(surface.Width);
				writer.WriteInt32(surface.Height);
				writer.WriteInt32(surface.Depth);
				writer.WriteInt32(surface.Size);
				writer.WriteInt32(surface.Stride);

				for (var i = 0; i < surface.Size * surface.Depth; ++i)
					writer.WriteByte(surface.Data[i]);
			}
		}

		/// <summary>
		///     Converts the DDS data format to the corresponding surface format.
		/// </summary>
		/// <param name="format">The data format that should be converted.</param>
		private static SurfaceFormat ToSurfaceFormat(Format format)
		{
			switch (format)
			{
				case Format.BC1_Typeless:
					return SurfaceFormat.Bc1;
				case Format.BC2_Typeless:
					return SurfaceFormat.Bc2;
				case Format.BC3_Typeless:
					return SurfaceFormat.Bc3;
				case Format.BC4_Typeless:
					return SurfaceFormat.Bc4;
				case Format.BC5_Typeless:
					return SurfaceFormat.Bc5;
				case Format.R8G8B8A8_UNorm:
					return SurfaceFormat.Rgba8;
				default:
					throw new InvalidOperationException("Unsupported DDS data format.");
			}
		}

		/// <summary>
		///     Converts the DDS resource dimension to the corresponding texture type.
		/// </summary>
		private TextureType GetTextureType()
		{
			if (_header.ResourceDimension == ResourceDimension.Texture2D && _header.MiscFlags.HasFlag(ResourceOptionFlags.TextureCube))
				return TextureType.CubeMap;

			switch (_header.ResourceDimension)
			{
				case ResourceDimension.Texture1D:
					return TextureType.Texture1D;
				case ResourceDimension.Texture2D:
					return TextureType.Texture2D;
				case ResourceDimension.Texture3D:
					return TextureType.Texture3D;
				default:
					throw new InvalidOperationException("Unsupported resource dimension.");
			}
		}

		/// <summary>
		///     Creates a four-character code from the given values.
		/// </summary>
		private static uint MakeFourCharacterCode(uint ch0, uint ch1, uint ch2, uint ch3)
		{
			return ((byte)(ch0) | ((uint)(byte)(ch1) << 8) | ((uint)(byte)(ch2) << 16) | ((uint)(byte)(ch3) << 24));
		}

		/// <summary>
		///     Gets the surface information for a particular data format.
		/// </summary>
		/// <param name="width">The width of the surface.</param>
		/// <param name="height">The height of the surface.</param>
		/// <param name="format">The data format of the surface.</param>
		/// <param name="size">The total number of bytes that are required to store the surface.</param>
		/// <param name="stride">The total number of bytes that are required to store a row of the surface.</param>
		private static void GetSurfaceInfo(int width, int height, Format format, out int size, out int stride)
		{
			var blockCompressed = false;
			var packed = false;
			var bytesPerBlock = 0;

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

			int numRows;
			if (blockCompressed)
			{
				var numBlocksWide = 0;
				var numBlocksHigh = 0;

				if (width > 0)
					numBlocksWide = Math.Max(1, (width + 3) / 4);
				if (height > 0)
					numBlocksHigh = Math.Max(1, (height + 3) / 4);

				stride = numBlocksWide * bytesPerBlock;
				numRows = numBlocksHigh;
			}
			else if (packed)
			{
				stride = ((width + 1) >> 1) * 4;
				numRows = height;
			}
			else
			{
				var bpp = BitsPerPixel(format);
				stride = (width * bpp + 7) / 8;
				numRows = height;
			}

			size = stride * numRows;
		}

		/// <summary>
		///     Returns the number of bits per pixel for the given data format.
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

		/// <summary>
		///     The cubemap flags.
		/// </summary>
		[Flags]
		private enum CubemapFlags
		{
			CubeMap = 0x00000200,
			Volume = 0x00200000,
			PositiveX = 0x00000600,
			NegativeX = 0x00000a00,
			PositiveY = 0x00001200,
			NegativeY = 0x00002200,
			PositiveZ = 0x00004200,
			NegativeZ = 0x00008200,

			AllFaces = PositiveX | NegativeX | PositiveY | NegativeY | PositiveZ | NegativeZ,
		} // ReSharper restore InconsistentNaming
		// ReSharper disable InconsistentNaming

		/// <summary>
		///     Indicates in which DXGI format the data in the DDS file is stored.
		/// </summary>
		private enum Format
		{
			Unknown = 0,

			R32G32B32A32_Typeless = 1,

			R32G32B32A32_Float = 2,
			R32G32B32A32_UInt = 3,
			R32G32B32A32_SInt = 4,
			R32G32B32_Typeless = 5,
			R32G32B32_Float = 6,
			R32G32B32_UInt = 7,
			R32G32B32_SInt = 8,
			R16G16B16A16_Typeless = 9,
			R16G16B16A16_Float = 10,
			R16G16B16A16_UNorm = 11,
			R16G16B16A16_UInt = 12,
			R16G16B16A16_SNorm = 13,
			R16G16B16A16_SInt = 14,
			R32G32_Typeless = 15,
			R32G32_Float = 16,
			R32G32_UInt = 17,
			R32G32_SInt = 18,
			R32G8X24_Typeless = 19,
			D32_Float_S8X24_UInt = 20,
			R32_Float_X8X24_Typeless = 21,
			X32_Typeless_G8X24_UInt = 22,
			R10G10B10A2_Typeless = 23,
			R10G10B10A2_UNorm = 24,
			R10G10B10A2_UInt = 25,
			R11G11B10_Float = 26,
			R8G8B8A8_Typeless = 27,
			R8G8B8A8_UNorm = 28,
			R8G8B8A8_UNorm_SRgb = 29,
			R8G8B8A8_UInt = 30,
			R8G8B8A8_SNorm = 31,
			R8G8B8A8_SInt = 32,
			R16G16_Typeless = 33,
			R16G16_Float = 34,
			R16G16_UNorm = 35,
			R16G16_UInt = 36,
			R16G16_SNorm = 37,
			R16G16_SInt = 38,
			R32_Typeless = 39,
			D32_Float = 40,
			R32_Float = 41,
			R32_UInt = 42,
			R32_SInt = 43,
			R24G8_Typeless = 44,
			D24_UNorm_S8_UInt = 45,
			R24_UNorm_X8_Typeless = 46,
			X24_Typeless_G8_UInt = 47,
			R8G8_Typeless = 48,
			R8G8_UNorm = 49,
			R8G8_UInt = 50,
			R8G8_SNorm = 51,
			R8G8_SInt = 52,
			R16_Typeless = 53,
			R16_Float = 54,
			D16_UNorm = 55,
			R16_UNorm = 56,
			R16_UInt = 57,
			R16_SNorm = 58,
			R16_SInt = 59,
			R8_Typeless = 60,
			R8_UNorm = 61,
			R8_UInt = 62,
			R8_SNorm = 63,
			R8_SInt = 64,
			A8_UNorm = 65,
			R1_UNorm = 66,
			R9G9B9E5_Sharedexp = 67,
			R8G8_B8G8_UNorm = 68,
			G8R8_G8B8_UNorm = 69,
			BC1_Typeless = 70,
			BC1_UNorm = 71,
			BC1_UNorm_SRgb = 72,
			BC2_Typeless = 73,
			BC2_UNorm = 74,
			BC2_UNorm_SRgb = 75,
			BC3_Typeless = 76,
			BC3_UNorm = 77,
			BC3_UNorm_SRgb = 78,
			BC4_Typeless = 79,
			BC4_UNorm = 80,
			BC4_SNorm = 81,
			BC5_Typeless = 82,
			BC5_UNorm = 83,
			BC5_SNorm = 84,
			B5G6R5_UNorm = 85,
			B5G5R5A1_UNorm = 86,
			B8G8R8A8_UNorm = 87,
			B8G8R8X8_UNorm = 88,
			R10G10B10_Xr_Bias_A2_UNorm = 89,
			B8G8R8A8_Typeless = 90,
			B8G8R8A8_UNorm_SRgb = 91,
			B8G8R8X8_Typeless = 92,
			B8G8R8X8_UNorm_SRgb = 93,
			BC6H_Typeless = 94,
			BC6H_Uf16 = 95,
			BC6H_Sf16 = 96,
			BC7_Typeless = 97,
			BC7_UNorm = 98,
			BC7_UNorm_SRgb = 99,
		}

		/// <summary>
		///     Represents the header of the DDS file.
		/// </summary>
		private unsafe struct Header
		{
			/// <summary>
			///     Unused data that is only required to ensure that the Header struct has the correct unmanaged size.
			/// </summary>
			[UsedImplicitly]
			private fixed uint _unused [16];

			/// <summary>
			///     Initializes a new instance from the given buffer.
			/// </summary>
			/// <param name="reader">The buffer from which the instance should be initialized.</param>
			public Header(BinaryReader reader)
				: this()
			{
				Assert.ArgumentNotNull(reader);

				Size = reader.ReadInt32();
				Flags = (HeaderFlags)reader.ReadInt32();
				Height = reader.ReadInt32();
				Width = reader.ReadInt32();
				reader.ReadUInt32();
				Depth = reader.ReadInt32();
				MipMapCount = reader.ReadInt32();

				for (var i = 0; i < 11; ++i)
					reader.ReadUInt32();

				PixelFormat = new PixelFormat(reader);
				SurfaceFlags = (SurfaceFlags)reader.ReadUInt32();
				CubeMapFlags = (CubemapFlags)reader.ReadUInt32();

				for (var i = 0; i < 3; ++i)
					reader.ReadUInt32();

				Format = (Format)reader.ReadUInt32();
				ResourceDimension = (ResourceDimension)reader.ReadUInt32();
				MiscFlags = (ResourceOptionFlags)reader.ReadUInt32();
				ArraySize = reader.ReadInt32();
				reader.ReadUInt32();
			}

			/// <summary>
			///     Gets the size of the texture array.
			/// </summary>
			public int ArraySize { get; private set; }

			/// <summary>
			///     Gets the texture format.
			/// </summary>
			public Format Format { get; private set; }

			/// <summary>
			///     Gets the texture dimension.
			/// </summary>
			public ResourceDimension ResourceDimension { get; private set; }

			/// <summary>
			///     Gets the miscellaneous flags.
			/// </summary>
			public ResourceOptionFlags MiscFlags { get; private set; }

			/// <summary>
			///     Gets the cube map flags.
			/// </summary>
			public CubemapFlags CubeMapFlags { get; private set; }

			/// <summary>
			///     Gets the pixel format.
			/// </summary>
			public PixelFormat PixelFormat { get; private set; }

			/// <summary>
			///     Gets the depth of the texture.
			/// </summary>
			public int Depth { get; private set; }

			public HeaderFlags Flags { get; private set; }

			/// <summary>
			///     Gets the height of the texture.
			/// </summary>
			public int Height { get; private set; }

			/// <summary>
			///     Gets the number of mipmaps.
			/// </summary>
			public int MipMapCount { get; private set; }

			/// <summary>
			///     Gets the size of the header in bytes.
			/// </summary>
			public int Size { get; private set; }

			/// <summary>
			///     Gets the surface flags.
			/// </summary>
			public SurfaceFlags SurfaceFlags { get; private set; }

			/// <summary>
			///     Gets the width of the texture.
			/// </summary>
			public int Width { get; private set; }
		}

		/// <summary>
		///     The header flags.
		/// </summary>
		[Flags]
		private enum HeaderFlags
		{
			Texture = 0x00001007,
			Mipmap = 0x00020000,
			Volume = 0x00800000,
			Pitch = 0x00000008,
			LinearSize = 0x00080000,
			Height = 0x00000002,
			Width = 0x00000004,
		}

		/// <summary>
		///     Provides information about the pixel format of the DDS file.
		/// </summary>
		private struct PixelFormat
		{
			/// <summary>
			///     Initializes a new instance from the given buffer.
			/// </summary>
			/// <param name="reader">The buffer from which the instance should be initialized.</param>
			public PixelFormat(BinaryReader reader)
				: this()
			{
				Assert.ArgumentNotNull(reader);

				Size = reader.ReadUInt32();
				Flags = (PixelFormatFlags)reader.ReadUInt32();
				FourCC = reader.ReadUInt32();
				RgbBitCount = reader.ReadUInt32();
				RBitMask = reader.ReadUInt32();
				GBitMask = reader.ReadUInt32();
				BBitMask = reader.ReadUInt32();
				ABitMask = reader.ReadUInt32();
			}

			/// <summary>
			///     Gets the alpha channel bit mask.
			/// </summary>
			public uint ABitMask { get; private set; }

			/// <summary>
			///     Gets the blue channel bit mask.
			/// </summary>
			public uint BBitMask { get; private set; }

			/// <summary>
			///     Gets the green channel bit mask.
			/// </summary>
			public uint GBitMask { get; private set; }

			/// <summary>
			///     Gets the red channel bit mask.
			/// </summary>
			public uint RBitMask { get; private set; }

			/// <summary>
			///     Gets the RGB bit count.
			/// </summary>
			public uint RgbBitCount { get; private set; }

			/// <summary>
			///     Gets the pixel format flags.
			/// </summary>
			public PixelFormatFlags Flags { get; private set; }

			/// <summary>
			///     Gets the four-character code.
			/// </summary>
			public uint FourCC { get; private set; }

			/// <summary>
			///     Gets the size of the pixel format structure in bytes.
			/// </summary>
			public uint Size { get; private set; }
		}

		/// <summary>
		///     The pixel format flags.
		/// </summary>
		[Flags]
		private enum PixelFormatFlags
		{
			FourCC = 0x00000004,
			Rgb = 0x00000040,
			Rgba = 0x00000041,
			Luminance = 0x00020000,
			LuminanceAlpha = 0x00020001,
			Alpha = 0x00000002,
			Pal8 = 0x00000020,
		}

		/// <summary>
		///     The resource dimension.
		/// </summary>
		private enum ResourceDimension
		{
			Unknown = 0,
			Buffer = 1,
			Texture1D = 2,
			Texture2D = 3,
			Texture3D = 4,
		}

		/// <summary>
		///     The resource option flags.
		/// </summary>
		[Flags]
		private enum ResourceOptionFlags
		{
			None = 0,
			GenerateMipMaps = 1,
			Shared = 2,
			TextureCube = 4,
			DrawIndirectArguments = 16,
			BufferAllowRawViews = 32,
			BufferStructured = 64,
			ResourceClamp = 128,
			SharedKeyedmutex = 256,
			GdiCompatible = 512,
		}

		/// <summary>
		///     The surface flags.
		/// </summary>
		[Flags]
		private enum SurfaceFlags
		{
			Texture = 0x00001000,
			Mipmap = 0x00400008,
			Cubemap = 0x00000008,
		}
	}
}