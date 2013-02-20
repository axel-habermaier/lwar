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
		}

		/// <summary>
		///   Creates a FourCC from the given values.
		/// </summary>
		private static uint MakeFourCC(uint ch0, uint ch1, uint ch2, uint ch3)
		{
			return ((byte)(ch0) | ((uint)(byte)(ch1) << 8) | ((uint)(byte)(ch2) << 16) | ((uint)(byte)(ch3) << 24));
		}
	}
}