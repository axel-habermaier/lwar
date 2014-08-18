namespace Pegasus.Assets
{
	using System;
	using Platform;
	using Platform.Memory;

	/// <summary>
	///     Represents the header of a compiled asset file.
	/// </summary>
	internal static class AssetHeader
	{
		/// <summary>
		///     Reads and validates the asset file header in the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the asset file header should be read from.</param>
		/// <param name="assetType">The type of the asset that is expected to follow in the buffer.</param>
		public static void Validate(BufferReader buffer, byte assetType)
		{
			Assert.ArgumentNotNull(buffer);

			if (!buffer.CanRead(5))
				throw new InvalidOperationException("Asset is corrupted: Header information missing.");

			if (buffer.ReadByte() != 'p' || buffer.ReadByte() != 'g')
				throw new InvalidOperationException("Asset is corrupted: Application identifier mismatch in asset file header.");

			if (buffer.ReadUInt16() != PlatformInfo.AssetFileVersion)
				throw new InvalidOperationException("Asset is stored in an outdated version of the compiled asset format and must be re-compiled.");

			var actualType = buffer.ReadByte();
			if (actualType != assetType)
				throw new InvalidOperationException("Unexpected asset type stored in asset file.");
		}
	}
}