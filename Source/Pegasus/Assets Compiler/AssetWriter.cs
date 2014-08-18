namespace Pegasus.AssetsCompiler
{
	using System;
	using System.IO;
	using Assets;
	using Platform.Logging;

	/// <summary>
	///     Writes a compiled asset file. The endianess of the compiled file always matches the endianess of the target
	///     platform, assuming that the compiler is always running on a little endian platform.
	/// </summary>
	internal sealed class AssetWriter : IDisposable
	{
		/// <summary>
		///     The target path of the compiled asset.
		/// </summary>
		private readonly string _targetPath;

		/// <summary>
		///     The temp path of the compiled asset.
		/// </summary>
		private readonly string _tempPath;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="asset">The asset that should be written.</param>
		public AssetWriter(Asset asset)
		{
			Assert.ArgumentNotNull(asset);

			_tempPath = asset.TempPath;
			_targetPath = asset.TargetPath;
			Writer = new BufferWriter();
		}

		/// <summary>
		///     Gets the writer that can be used to write the asset data.
		/// </summary>
		public BufferWriter Writer { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			File.WriteAllBytes(_tempPath, Writer.ToArray());
			File.WriteAllBytes(_targetPath, Writer.ToArray());

			Writer.Dispose();
		}
	}
}