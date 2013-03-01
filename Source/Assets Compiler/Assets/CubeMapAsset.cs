using System;

namespace Pegasus.AssetsCompiler.Assets
{
	using System.Drawing;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;

	/// <summary>
	///   Represents a cube map asset that requires compilation.
	/// </summary>
	public class CubeMapAsset : TextureAsset
	{
		/// <summary>
		///   The image data.
		/// </summary>
		private readonly BufferPointer[] _data = new BufferPointer[6];

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public CubeMapAsset(string relativePath)
			: base(relativePath)
		{
		}

		/// <summary>
		///   Loads the cube map data.
		/// </summary>
		public void Load()
		{
			Bitmap = (Bitmap)Image.FromFile(SourcePath);
			uint componentCount;

			Description = new TextureDescription
			{
				Width = (uint)Bitmap.Width / 6,
				Height = (uint)Bitmap.Height,
				Depth = 1,
				Format = ToSurfaceFormat(Bitmap.PixelFormat, out componentCount),
				ArraySize = 1,
				Type = TextureType.CubeMap,
				Mipmaps = Framework.Platform.Graphics.Mipmaps.One,
				SurfaceCount = 6
			};

			LoadCubemapSurfaces();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_data.SafeDisposeAll();
			base.OnDisposing();
		}

		/// <summary>
		///   Loads and converts the image data.
		/// </summary>
		private unsafe void LoadCubemapSurfaces()
		{
			var faces = ExtractFaces();
			for (var i = 0; i < 6; ++i)
				_data[i] = GetBitmapData(faces[i]);
			faces.SafeDisposeAll();

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
					Data = _data[i].Pointer
				};
			}
		}

		/// <summary>
		///   Extracts the cube map faces from the given cube map.
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

			return new[]
			{
				negativeZ,
				negativeX,
				positiveZ,
				positiveX,
				negativeY,
				positiveY,
			};
		}
	}
}