using System;

namespace Pegasus.AssetsCompiler.DDS
{
	using System.Collections.Generic;
	using System.IO;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;

	/// <summary>
	///   Implements a subset of the DX10 DDS file specification based on the sample provided by Microsoft at
	///   http://msdn.microsoft.com/en-us/library/windows/apps/jj651550.aspx.
	/// </summary>
	public class DirectDrawSurface : DisposableObject
	{
		/// <summary>
		///   The magic DDS file code "DDS ".
		/// </summary>
		private const int MagicCode = 0x20534444;

		/// <summary>
		///   The texture description for the image.
		/// </summary>
		private readonly TextureDescription _description;

		/// <summary>
		///   The pointer to the data buffer.
		/// </summary>
		private readonly BufferPointer _pointer;

		/// <summary>
		///   The surfaces of the image.
		/// </summary>
		private readonly Surface[] _surfaces;

		/// <summary>
		///   The DDS file header.
		/// </summary>
		private Header _header;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="buffer">The buffer that contains the contents of the DDS file.</param>
		public unsafe DirectDrawSurface(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			if (buffer.BufferSize < sizeof(uint) + sizeof(Header))
				Log.Die("Invalid DDS file: Header information is incomplete.");

			if (buffer.ReadUInt32() != MagicCode)
				Log.Die("Not a DDS file.");

			_header = new Header(buffer);
			if (_header.Size != 124 || _header.PixelFormat.Size != sizeof(PixelFormat))
				Log.Die("DDS file is corrupt.");

			if (!_header.PixelFormat.Flags.HasFlag(PixelFormatFlags.FourCC) ||
				_header.PixelFormat.FourCC != MakeFourCC('D', 'X', '1', '0'))
				Log.Die("DDS file is not in DX10 format.");

			if (_header.ArraySize == 0)
				Log.Die("An array size of 0 is invalid.");

			_description.Width = _header.Width;
			_description.Height = Math.Max(_header.Height, 1);
			_description.Depth = Math.Max(_header.Depth, 1);
			_description.Format = ToSurfaceFormat(_header.Format);
			_description.ArraySize = _header.ArraySize;
			_description.Type = GetTextureType();
			_description.Mipmaps = (Mipmaps)((int)Mipmaps.None + _header.MipMapCount);

			var faces = _description.ArraySize;
			if (_description.Type == TextureType.CubeMap)
				faces *= 6;

			_description.SurfaceCount = faces * _header.MipMapCount;
			_surfaces = new Surface[_description.SurfaceCount];
			_pointer = buffer.GetPointer();
			var data = _pointer.Pointer;

			for (int i = 0, index = 0; i < faces; ++i)
			{
				var width = _description.Width;
				var height = _description.Height;
				var depth = _description.Depth;

				for (var j = 0; j < _header.MipMapCount; ++j, ++index)
				{
					uint size, stride;
					GetSurfaceInfo(width, height, _header.Format, out size, out stride);

					_surfaces[index] = new Surface
					{
						Width = width,
						Height = height,
						Depth = depth,
						Size = size,
						Stride = stride,
						Data = data,
					};

					var surfaceSize = depth * size;
					data += surfaceSize;
					buffer.Skip((int)surfaceSize);

					width = Math.Max(width >> 1, 1);
					height = Math.Max(height >> 1, 1);
					depth = Math.Max(depth >> 1, 1);
				}
			}

			Assert.That(buffer.EndOfBuffer, "Failed to process entire DDS file.");
		}

		/// <summary>
		///   Gets the texture description for the image.
		/// </summary>
		public TextureDescription Description
		{
			get { return _description; }
		}

		/// <summary>
		///   Gets the surfaces of the image.
		/// </summary>
		public IEnumerable<Surface> Surfaces
		{
			get { return _surfaces; }
		}

		/// <summary>
		///   Converts the DDS data format to the corresponding surface format.
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
		///   Saves the DDS file to disk.
		/// </summary>
		/// <param name="path">The path of the file in which the DDS should be stored.</param>
		public unsafe void Save(string path)
		{
			Assert.ArgumentNotNullOrWhitespace(path, () => path);

			var buffer = new byte[64 * 1024 * 1024];
			using (var writer = BufferWriter.Create(buffer))
			{
				writer.WriteUInt32(MagicCode);
				_header.Write(writer);

				foreach (var surface in _surfaces)
				{
					for (var i = 0; i < surface.Size * surface.Depth; ++i)
						writer.WriteByte(surface.Data[i]);
				}
			}

			File.WriteAllBytes(path, buffer);
		}

		/// <summary>
		///   Converts the DDS resource dimension to the corresponding texture type.
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
		/// <param name="size">The total number of bytes that are required to store the surface.</param>
		/// <param name="stride">The total number of bytes that are required to store a row of the surface.</param>
		private static void GetSurfaceInfo(uint width, uint height, Format format, out uint size, out uint stride)
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

			uint numRows;
			if (blockCompressed)
			{
				uint numBlocksWide = 0;
				uint numBlocksHigh = 0;

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
				stride = (uint)(width * bpp + 7) / 8;
				numRows = height;
			}

			size = stride * numRows;
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

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_pointer.SafeDispose();
		}
	}
}