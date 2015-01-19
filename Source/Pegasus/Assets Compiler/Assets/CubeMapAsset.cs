namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Drawing;
	using System.Xml.Linq;
	using Textures;

	/// <summary>
	///     Represents a cube map asset that requires compilation.
	/// </summary>
	internal class CubeMapAsset : TextureAsset
	{
		/// <summary>
		///     The image data.
		/// </summary>
		private readonly byte[][] _data = new byte[6][];

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="metadata">The metadata of the asset.</param>
		public CubeMapAsset(XElement metadata)
			: base(metadata)
		{
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public override byte AssetType
		{
			get { return 1; }
		}

		/// <summary>
		///     Gets the runtime type of the asset.
		/// </summary>
		public override string RuntimeType
		{
			get { return "Pegasus.Platform.Graphics.CubeMap"; }
		}

		/// <summary>
		///     Loads the cube map data.
		/// </summary>
		public void Load()
		{
			Bitmap = (Bitmap)Image.FromFile(AbsoluteSourcePath);
			uint componentCount;

			Description = new TextureDescription
			{
				Width = (uint)Bitmap.Width / 6,
				Height = (uint)Bitmap.Height,
				Depth = 1,
				Format = ToSurfaceFormat(Bitmap.PixelFormat, out componentCount),
				ArraySize = 1,
				Type = TextureType.CubeMap,
				Mipmaps = 1,
				SurfaceCount = 6
			};

			LoadCubemapSurfaces();
		}

		/// <summary>
		///     Loads and converts the image data.
		/// </summary>
		private void LoadCubemapSurfaces()
		{
			var faces = ExtractFaces();

			for (var i = 0; i < 6; ++i)
				_data[i] = GetBitmapData(faces[i]);

			foreach (var face in faces)
				face.Dispose();

			uint componentCount;
			ToSurfaceFormat(Bitmap.PixelFormat, out componentCount);

			Surfaces = new Surface[6];
			for (var i = 0; i < 6; ++i)
			{
				Surfaces[i] = new Surface
				{
					Width = Description.Width,
					Height = Description.Height,
					Depth = 1,
					Size = Description.Width * Description.Height * componentCount,
					Stride = Description.Width * componentCount,
					Data = _data[i]
				};
			}
		}

		/// <summary>
		///     Extracts the cube map faces from the given cube map.
		/// </summary>
		public Bitmap[] ExtractFaces()
		{
			var width = Bitmap.Width / 6;
			var negativeZ = Bitmap.Clone(new Rectangle(0, 0, width, Bitmap.Height), Bitmap.PixelFormat);
			var negativeX = Bitmap.Clone(new Rectangle(width, 0, width, Bitmap.Height), Bitmap.PixelFormat);
			var positiveZ = Bitmap.Clone(new Rectangle(2 * width, 0, width, Bitmap.Height), Bitmap.PixelFormat);
			var positiveX = Bitmap.Clone(new Rectangle(3 * width, 0, width, Bitmap.Height), Bitmap.PixelFormat);
			var negativeY = Bitmap.Clone(new Rectangle(4 * width, 0, width, Bitmap.Height), Bitmap.PixelFormat);
			var positiveY = Bitmap.Clone(new Rectangle(5 * width, 0, width, Bitmap.Height), Bitmap.PixelFormat);

			ToPremultipliedAlpha(negativeZ);
			ToPremultipliedAlpha(negativeX);
			ToPremultipliedAlpha(positiveZ);
			ToPremultipliedAlpha(positiveX);
			ToPremultipliedAlpha(negativeY);
			ToPremultipliedAlpha(positiveY);

			return new[]
			{
				negativeZ,
				negativeX,
				positiveZ,
				positiveX,
				negativeY,
				positiveY
			};
		}
	}
}