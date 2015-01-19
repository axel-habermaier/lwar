namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Drawing;
	using System.Xml.Linq;
	using Textures;
	using Utilities;

	/// <summary>
	///     Represents a 2D texture that requires compilation.
	/// </summary>
	internal class Texture2DAsset : TextureAsset
	{
		/// <summary>
		///     The image data.
		/// </summary>
		private byte[] _data;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="metadata">The metadata of the asset.</param>
		/// <param name="basePath">Overrides the default base path of the asset.</param>
		public Texture2DAsset(XElement metadata, string basePath = null)
			: base(metadata, basePath)
		{
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public override byte AssetType
		{
			get { return 2; }
		}

		/// <summary>
		///     Gets the runtime type of the asset.
		/// </summary>
		public override string RuntimeType
		{
			get { return "Pegasus.Platform.Graphics.Texture2D"; }
		}

		/// <summary>
		///     Loads the texture data.
		/// </summary>
		public void Load()
		{
			Bitmap = (Bitmap)Image.FromFile(AbsoluteSourcePath);

			uint componentCount;
			Description = new TextureDescription
			{
				Width = (uint)Bitmap.Width,
				Height = (uint)Bitmap.Height,
				Depth = 1,
				Format = ToSurfaceFormat(Bitmap.PixelFormat, out componentCount),
				ArraySize = 1,
				Type = TextureType.Texture2D,
				Mipmaps = 1,
				SurfaceCount = 1
			};

			_data = GetBitmapData(Bitmap);
			Surfaces = new[]
			{
				new Surface
				{
					Width = Description.Width,
					Height = Description.Height,
					Depth = 1,
					Size = Description.Width * Description.Height * componentCount,
					Stride = Description.Width * componentCount,
					Data = _data
				}
			};
		}

		/// <summary>
		///     Saves the texture to a file.
		/// </summary>
		/// <param name="fileName">The file name to the texture file.</param>
		public void Save(string fileName)
		{
			Assert.NotNull(Bitmap);
			Assert.NotNull(_data);

			ToPremultipliedAlpha(Bitmap);
			Bitmap.Save(fileName);
		}
	}
}