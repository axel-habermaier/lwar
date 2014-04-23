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
		///     Writes the asset file header into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the asset file header should be written to.</param>
		/// <param name="assetType">The type of the asset that will subsequently be written into the buffer.</param>
		public static void Write(BufferWriter buffer, AssetType assetType)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentInRange(assetType);

			buffer.WriteByte((byte)'p');
			buffer.WriteByte((byte)'g');
			buffer.WriteUInt16(PlatformInfo.AssetFileVersion);
			buffer.WriteByte((byte)assetType);
		}

		/// <summary>
		///     Reads and validates the asset file header in the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the asset file header should be read from.</param>
		/// <param name="assetType">The type of the asset that is expected to follow in the buffer.</param>
		public static void Validate(BufferReader buffer, AssetType assetType)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentInRange(assetType);

			if (!buffer.CanRead(3))
				throw new InvalidOperationException("Asset is corrupted: Header information missing.");

			if (buffer.ReadByte() != 'p' || buffer.ReadByte() != 'g')
				throw new InvalidOperationException("Asset is corrupted: Application identifier mismatch in asset file header.");

			if (buffer.ReadUInt16() != PlatformInfo.AssetFileVersion)
				throw new InvalidOperationException("Asset is stored in an outdated version of the compiled asset format and must be re-compiled.");

			var actualType = (AssetType)buffer.ReadByte();
			if (actualType != assetType)
				throw new InvalidOperationException(String.Format("Asset is of type '{0}', but is loaded as type '{1}'.", actualType, assetType));
		}
	}
}