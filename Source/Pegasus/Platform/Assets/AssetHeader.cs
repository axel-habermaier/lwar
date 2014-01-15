namespace Pegasus.Platform.Assets
{
	using System;
	using Logging;
	using Memory;

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
		/// <param name="assetName">The name of the asset that should be used in case of errors.</param>
		public static void Validate(BufferReader buffer, AssetType assetType, string assetName)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentInRange(assetType);
			Assert.ArgumentNotNull(assetName);

			if (!buffer.CanRead(3))
				Log.Die("Asset '{0}' is corrupted: Header information missing.", assetName);

			if (buffer.ReadByte() != 'p' || buffer.ReadByte() != 'g')
				Log.Die("Asset '{0}' is corrupted: Application identifier mismatch in asset file header.", assetName);

			if (buffer.ReadUInt16() != PlatformInfo.AssetFileVersion)
				Log.Die("Asset '{0}' is stored in an outdated version of the compiled asset format and must be re-compiled.", assetName);

			var actualType = (AssetType)buffer.ReadByte();
			if (actualType != assetType)
				Log.Die("Asset '{0}' is of type '{1}', but is loaded as type '{2}'.", assetName, actualType, assetType);
		}
	}
}