using System;

namespace Pegasus.AssetsCompiler.DDS
{
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Represents the header of the DDS file.
	/// </summary>
	public unsafe struct Header
	{
		/// <summary>
		///   The pitch or linear size.
		/// </summary>
		private uint _pitchOrLinearSize;

		/// <summary>
		///   Unused data that is only required to ensure that the Header struct has the correct unmanaged size.
		/// </summary>
		private fixed uint _unused [15];

		/// <summary>
		///   Writes the header into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the header should be written into.</param>
		public void Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			buffer.WriteUInt32(Size);
			buffer.WriteUInt32((uint)Flags);
			buffer.WriteUInt32(Height);
			buffer.WriteUInt32(Width);
			buffer.WriteUInt32(_pitchOrLinearSize);
			buffer.WriteUInt32(Depth);
			buffer.WriteUInt32(MipMapCount);

			for (var i = 0; i < 11; ++i)
				buffer.WriteUInt32(0);

			PixelFormat.Write(buffer);
			buffer.WriteUInt32((uint)SurfaceFlags);
			buffer.WriteUInt32((uint)CubeMapFlags);

			for (var i = 0; i < 3; ++i)
				buffer.WriteUInt32(0);

			buffer.WriteUInt32((uint)Format);
			buffer.WriteUInt32((uint)ResourceDimension);
			buffer.WriteUInt32((uint)MiscFlags);
			buffer.WriteUInt32(ArraySize);
			buffer.WriteUInt32(0);
		}

		/// <summary>
		///   Initializes a new instance from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be initialized.</param>
		public Header(BufferReader buffer)
			: this()
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			Size = buffer.ReadUInt32();
			Flags = (HeaderFlags)buffer.ReadUInt32();
			Height = buffer.ReadUInt32();
			Width = buffer.ReadUInt32();
			_pitchOrLinearSize = buffer.ReadUInt32();
			Depth = buffer.ReadUInt32();
			MipMapCount = buffer.ReadUInt32();

			for (var i = 0; i < 11; ++i)
				buffer.ReadUInt32();

			PixelFormat = new PixelFormat(buffer);
			SurfaceFlags = (SurfaceFlags)buffer.ReadUInt32();
			CubeMapFlags = (CubemapFlags)buffer.ReadUInt32();

			for (var i = 0; i < 3; ++i)
				buffer.ReadUInt32();

			Format = (Format)buffer.ReadUInt32();
			ResourceDimension = (ResourceDimension)buffer.ReadUInt32();
			MiscFlags = (ResourceOptionFlags)buffer.ReadUInt32();
			ArraySize = buffer.ReadUInt32();
			buffer.ReadUInt32();
		}

		/// <summary>
		///   Gets the size of the texture array.
		/// </summary>
		public uint ArraySize { get; private set; }

		/// <summary>
		///   Gets the texture format.
		/// </summary>
		public Format Format { get; private set; }

		/// <summary>
		///   Gets the texture dimension.
		/// </summary>
		public ResourceDimension ResourceDimension { get; private set; }

		/// <summary>
		///   Gets the miscellaneous flags.
		/// </summary>
		public ResourceOptionFlags MiscFlags { get; private set; }

		/// <summary>
		///   Gets the cube map flags.
		/// </summary>
		public CubemapFlags CubeMapFlags { get; private set; }

		/// <summary>
		///   Gets the pixel format.
		/// </summary>
		public PixelFormat PixelFormat { get; private set; }

		/// <summary>
		///   Gets the depth of the texture.
		/// </summary>
		public uint Depth { get; private set; }

		public HeaderFlags Flags { get; private set; }

		/// <summary>
		///   Gets the height of the texture.
		/// </summary>
		public uint Height { get; private set; }

		/// <summary>
		///   Gets the number of mipmaps.
		/// </summary>
		public uint MipMapCount { get; private set; }

		/// <summary>
		///   Gets the size of the header in bytes.
		/// </summary>
		public uint Size { get; private set; }

		/// <summary>
		///   Gets the surface flags.
		/// </summary>
		public SurfaceFlags SurfaceFlags { get; private set; }

		/// <summary>
		///   Gets the width of the texture.
		/// </summary>
		public uint Width { get; private set; }
	}
}