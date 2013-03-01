using System;

namespace Pegasus.AssetsCompiler
{
	using System.Drawing;
	using Assets;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;

	/// <summary>
	///   Represents an uncompressed two-dimensional image that can be stored in the Pegasus texture format.
	/// </summary>
	internal class Image2D : Image
	{
		/// <summary>
		///   The image data.
		/// </summary>
		private readonly BufferPointer _data;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="texture">The texture asset that the image should represent.</param>
		public unsafe Image2D(Texture2DAsset texture)
		{
			Assert.ArgumentNotNull(texture, () => texture);
			Assert.ArgumentSatisfies(!texture.Mipmaps, () => texture, "Mipmap generation is not supported for uncompressed textures.");
			Assert.ArgumentSatisfies(texture.Uncompressed, () => texture, "Texture compression is not supported.");

			using (var bitmap = (Bitmap)System.Drawing.Image.FromFile(texture.SourcePath))
			{
				uint componentCount;
				Description = new TextureDescription
				{
					Width = (uint)bitmap.Width,
					Height = (uint)bitmap.Height,
					Depth = 1,
					Format = ToSurfaceFormat(bitmap.PixelFormat, out componentCount),
					ArraySize = 1,
					Type = TextureType.Texture2D,
					Mipmaps = Mipmaps.One,
					SurfaceCount = 1
				};

				_data = GetBitmapData(bitmap);
				Surfaces = new[]
				{
					new Surface
					{
						Width = Description.Width,
						Height = Description.Height,
						Depth = 1,
						Size = Description.Width * Description.Height * componentCount,
						Stride = Description.Width * componentCount,
						Data = _data.Pointer
					}
				};
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_data.SafeDispose();
		}
	}
}