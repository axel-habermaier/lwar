using System;

namespace Pegasus.Framework.Platform.Assets
{
	using System.IO;

	/// <summary>
	///   Reads compiled asset files.
	/// </summary>
	public sealed class AssetReader : DisposableObject
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="path">The path of the file that should be read.</param>
		public AssetReader(string path)
		{
			using (var stream = new FileStream(path + PlatformInfo.AssetExtension, FileMode.Open, FileAccess.Read))
			{
				Data = new byte[stream.Length];
				stream.Read(Data, 0, Data.Length);
			}

			Reader = BufferReader.Create(Data);
		}

		/// <summary>
		///   Gets the buffer containing all the data of the asset.
		/// </summary>
		public byte[] Data { get; private set; }

		/// <summary>
		///   Gets the reader that can be used to read the data of the asset.
		/// </summary>
		public BufferReader Reader { get; private set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Reader.SafeDispose();
		}
	}
}