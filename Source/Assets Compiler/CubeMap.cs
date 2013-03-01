using System;

namespace Pegasus.AssetsCompiler
{
	using System.Drawing;
	using Assets;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;

	/// <summary>
	///   Represents an uncompressed cube map that can be stored in the Pegasus texture format.
	/// </summary>
	internal class CubeMap : Image
	{
		/// <summary>
		///   The image data.
		/// </summary>
		private readonly BufferPointer[] _data = new BufferPointer[6];

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="cubemap">The cube map asset that the image should represent.</param>
		public CubeMap(CubeMapAsset cubemap)
		{
			Assert.ArgumentNotNull(cubemap, () => cubemap);
			Assert.ArgumentSatisfies(!cubemap.Mipmaps, () => cubemap, "Mipmap generation is not supported for uncompressed cube maps.");
			Assert.ArgumentSatisfies(cubemap.Uncompressed, () => cubemap, "Texture compression is not supported.");

			using (var bitmap = (Bitmap)System.Drawing.Image.FromFile(cubemap.SourcePath))
			{
				uint componentCount;
				Description = new TextureDescription
				{
					Width = (uint)bitmap.Width / 6,
					Height = (uint)bitmap.Height,
					Depth = 1,
					Format = ToSurfaceFormat(bitmap.PixelFormat, out componentCount),
					ArraySize = 1,
					Type = TextureType.CubeMap,
					Mipmaps = Mipmaps.One,
					SurfaceCount = 6
				};

				LoadCubemapSurfaces(bitmap);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_data.SafeDisposeAll();
		}

		/// <summary>
		///   Loads and converts the image data.
		/// </summary>
		/// <param name="bitmap">The bitmap from which the data should be loaded.</param>
		private unsafe void LoadCubemapSurfaces(Bitmap bitmap)
		{
			var faces = ExtractFaces(bitmap);
			for (var i = 0; i < 6; ++i)
				_data[i] = GetBitmapData(faces[i]);

			uint componentCount;
			ToSurfaceFormat(bitmap.PixelFormat, out componentCount);

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
		/// Extracts the cube map faces from the given cube map.
		/// </summary>
		/// <param name="cubeMap">The cube map whose faces should be extracted.</param>
		public static Bitmap[] ExtractFaces(Bitmap cubeMap)
		{
			var width = cubeMap.Width / 6;
			var negativeZ = cubeMap.Clone(new Rectangle(0, 0, width, cubeMap.Height), cubeMap.PixelFormat);
			var negativeX = cubeMap.Clone(new Rectangle(width, 0, width, cubeMap.Height), cubeMap.PixelFormat);
			var positiveZ = cubeMap.Clone(new Rectangle(2 * width, 0, width, cubeMap.Height), cubeMap.PixelFormat);
			var positiveX = cubeMap.Clone(new Rectangle(3 * width, 0, width, cubeMap.Height), cubeMap.PixelFormat);
			var negativeY = cubeMap.Clone(new Rectangle(4 * width, 0, width, cubeMap.Height), cubeMap.PixelFormat);
			var positiveY = cubeMap.Clone(new Rectangle(5 * width, 0, width, cubeMap.Height), cubeMap.PixelFormat);

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