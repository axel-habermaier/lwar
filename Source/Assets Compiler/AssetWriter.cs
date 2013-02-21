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
	public sealed class AssetWriter : DisposableObject
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
		///   The name of the asset.
		/// </summary>
		private readonly string _assetName;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="path">The path of the compiled asset.</param>
		public AssetWriter(string path)
		{
			_assetName = path;
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
			using (var stream = new FileStream(_assetName, FileMode.Create))
				stream.Write(Buffer, 0, Writer.Count);

			Writer.SafeDispose();
		}
	}
}