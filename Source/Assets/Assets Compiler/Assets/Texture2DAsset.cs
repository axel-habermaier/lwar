namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Drawing;
	using Pegasus.Assets;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///     Represents a 2D texture that requires compilation.
	/// </summary>
	public class Texture2DAsset : TextureAsset
	{
		/// <summary>
		///     The image data.
		/// </summary>
		private BufferPointer _data;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public Texture2DAsset(string relativePath)
			: base(relativePath)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		/// <param name="sourceDirectory">The source directory of the asset.</param>
		protected Texture2DAsset(string relativePath, string sourceDirectory)
			: base(relativePath, sourceDirectory)
		{
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public override byte AssetType
		{
			get { return (byte)Pegasus.Assets.AssetType.Texture2D; }
		}

		/// <summary>
		///     The identifier type that should be used for the asset when generating the asset identifier list. If null is
		///     returned, no asset identifier is generated for this asset instance.
		/// </summary>
		public override string IdentifierType
		{
			get { return "Pegasus.Platform.Graphics.Texture2D"; }
		}

		/// <summary>
		///     The name that should be used for the asset identifier. If null is returned, no asset identifier is generated for
		///     this asset instance.
		/// </summary>
		public override string IdentifierName
		{
			get { return FileNameWithoutExtension; }
		}

		/// <summary>
		///     Loads the texture data.
		/// </summary>
		public unsafe void Load()
		{
			Bitmap = (Bitmap)Image.FromFile(SourcePath);

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
					Data = _data.Pointer
				}
			};
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_data.SafeDispose();
		}
	}
}