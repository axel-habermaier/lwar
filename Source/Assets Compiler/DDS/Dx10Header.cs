using System;

namespace Pegasus.AssetsCompiler.DDS
{
	using Framework.Platform;

	/// <summary>
	///   Represents the DirectX 10 header of the DDS file.
	/// </summary>
	public struct Dx10Header
	{
		/// <summary>
		///   Initializes a new instance from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be initialized.</param>
		public Dx10Header(BufferReader buffer)
			: this()
		{
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
	}
}