using System;

namespace Pegasus.AssetsCompiler
{
	using System.IO;
	using Assets;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Memory;

	/// <summary>
	///   Writes a compiled asset file. The endianess of the compiled file always matches the endianess of the target
	///   platform, assuming that the compiler is always running on a little endian platform.
	/// </summary>
	internal sealed class AssetWriter : DisposableObject
	{
		/// <summary>
		///   The maximum asset size in megabytes.
		/// </summary>
		private const int MaxAssetSize = 128;

		/// <summary>
		///   The buffer that stores the asset's data.
		/// </summary>
		private readonly byte[] _buffer = new byte[MaxAssetSize * 1024 * 1024];

		/// <summary>
		///   The target path of the compiled asset.
		/// </summary>
		private readonly string _targetPath;

		/// <summary>
		///   The temp path of the compiled asset.
		/// </summary>
		private readonly string _tempPath;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="asset">The asset that should be written.</param>
		public AssetWriter(Asset asset)
		{
			Assert.ArgumentNotNull(asset, () => asset);

			_tempPath = asset.TempPath;
			_targetPath = asset.TargetPath;
			Writer = BufferWriter.Create(_buffer);
		}

		/// <summary>
		///   Gets the writer that can be used to write the asset data.
		/// </summary>
		public BufferWriter Writer { get; private set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			using (var stream = new FileStream(_tempPath, FileMode.Create))
				stream.Write(_buffer, 0, Writer.Count);

			using (var stream = new FileStream(_targetPath, FileMode.Create))
				stream.Write(_buffer, 0, Writer.Count);

			Writer.SafeDispose();
		}
	}
}