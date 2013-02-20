using System;

namespace Pegasus.AssetsCompiler.DDS
{
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Provides information about the pixel format of the DDS file.
	/// </summary>
	public struct PixelFormat
	{
		/// <summary>
		///   Initializes a new instance from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be initialized.</param>
		public PixelFormat(BufferReader buffer)
			: this()
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			Size = buffer.ReadUInt32();
			Flags = (PixelFormatFlags)buffer.ReadUInt32();
			FourCC = buffer.ReadUInt32();
			RgbBitCount = buffer.ReadUInt32();
			RBitMask = buffer.ReadUInt32();
			GBitMask = buffer.ReadUInt32();
			BBitMask = buffer.ReadUInt32();
			ABitMask = buffer.ReadUInt32();
		}

		/// <summary>
		///   Writes the pixel format into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the pixel format should be written into.</param>
		public void Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			buffer.WriteUInt32(Size);
			buffer.WriteUInt32((uint)Flags);
			buffer.WriteUInt32(FourCC);
			buffer.WriteUInt32(RgbBitCount);
			buffer.WriteUInt32(RBitMask);
			buffer.WriteUInt32(GBitMask);
			buffer.WriteUInt32(BBitMask);
			buffer.WriteUInt32(ABitMask);
		}

		/// <summary>
		///   Gets the alpha channel bit mask.
		/// </summary>
		public uint ABitMask { get; private set; }

		/// <summary>
		///   Gets the blue channel bit mask.
		/// </summary>
		public uint BBitMask { get; private set; }

		/// <summary>
		///   Gets the green channel bit mask.
		/// </summary>
		public uint GBitMask { get; private set; }

		/// <summary>
		///   Gets the red channel bit mask.
		/// </summary>
		public uint RBitMask { get; private set; }

		/// <summary>
		///   Gets the RGB bit count.
		/// </summary>
		public uint RgbBitCount { get; private set; }

		/// <summary>
		///   Gets the pixel format flags.
		/// </summary>
		public PixelFormatFlags Flags { get; private set; }

		/// <summary>
		///   Gets the four-character code.
		/// </summary>
		public uint FourCC { get; private set; }

		/// <summary>
		///   Gets the size of the pixel format structure in bytes.
		/// </summary>
		public uint Size { get; private set; }
	};
}