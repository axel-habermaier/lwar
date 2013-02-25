using System;

namespace Pegasus.AssetsCompiler
{
	using System.IO;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Writes a compiled asset file. The endianess of the compiled file always matches the endianess of the target
	///   platform, assuming that the compiler is always running on a little endian platform.
	/// </summary>
	internal sealed class AssetWriter : DisposableObject
	{
		/// <summary>
		///   The maximum asset size in megabytes.
		/// </summary>
		private const int MaxAssetSize = 32;

		/// <summary>
		///   The buffer that stores the asset's data.
		/// </summary>
		private static readonly byte[] Buffer = new byte[MaxAssetSize * 1024 * 1024];

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
		/// <param name="tempPath">The temp path of the compiled asset.</param>
		/// <param name="targetPath">The target path of the compiled asset.</param>
		public AssetWriter(string tempPath, string targetPath)
		{
			Assert.ArgumentNotNullOrWhitespace(tempPath, () => tempPath);
			Assert.ArgumentNotNullOrWhitespace(targetPath, () => targetPath);

			_tempPath = tempPath;
			_targetPath = targetPath;
			Writer = BufferWriter.Create(Buffer);
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
				stream.Write(Buffer, 0, Writer.Count);

			using (var stream = new FileStream(_targetPath, FileMode.Create))
				stream.Write(Buffer, 0, Writer.Count);

			Writer.SafeDispose();
		}
	}
}