using System;

namespace Pegasus.AssetsCompiler
{
	using System.Drawing;
	using System.IO;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Compiles 2D textures.
	/// </summary>
	public sealed class Texture2DCompiler : TextureCompiler
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		public Texture2DCompiler(string asset)
			: base(asset)
		{
		}

		/// <summary>
		///   Gets a description of the type of the asset that is compiled by the compiler.
		/// </summary>
		internal override string AssetType
		{
			get { return "2D Textures"; }
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		protected override void CompileCore()
		{
			using (var bitmap = (Bitmap)Image.FromFile(Asset.SourcePath))
			{
				if (bitmap.Width < 1 || bitmap.Width > Int16.MaxValue || !IsPowerOfTwo(bitmap.Width))
					Log.Die("Invalid texture width '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Width, Int16.MaxValue);
				if (bitmap.Height < 1 || bitmap.Height > Int16.MaxValue || !IsPowerOfTwo(bitmap.Height))
					Log.Die("Invalid texture height '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Height, Int16.MaxValue);

				var outFile = Asset.TempPathWithoutExtension + ".dds";
				var format = ChooseCompression(bitmap.PixelFormat);
				ExternalTool.NvCompress(Asset.SourcePath, outFile, format);

				using (var buffer = BufferReader.Create(File.ReadAllBytes(outFile)))
				{
					var ddsImage = new DirectDrawSurface(buffer);
					ddsImage.Write(Buffer);
				}
			}
		}
	}
}