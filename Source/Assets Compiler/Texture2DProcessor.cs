using System;

namespace Pegasus.AssetsCompiler
{
	using System.Drawing;
	using System.Linq;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;

	/// <summary>
	///   Processes 2D textures, converting them to a premultiplied format.
	/// </summary>
	public sealed class Texture2DProcessor : TextureProcessor
	{
		/// <summary>
		///   Processes the texture.
		/// </summary>
		/// <param name="data">The textue data.</param>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		/// <param name="writer">The writer that should be used to write the compiled asset file.</param>
		protected override void Process(byte[] data, int width, int height, SurfaceFormat format, BufferWriter writer)
		{
			if (width < 1 || width > Int16.MaxValue || !IsPowerOfTwo(width))
				Log.Die("Invalid texture width '{0}' (must be power-of-two and between 0 and {1}).", width, Int16.MaxValue);
			if (height < 1 || height > Int16.MaxValue || !IsPowerOfTwo(height))
				Log.Die("Invalid texture height '{0}' (must be power-of-two and between 0 and {1}).", height, Int16.MaxValue);

			writer.WriteInt32(width);
			writer.WriteInt32(height);
			writer.WriteInt32((int)format);
			writer.WriteByteArray(data);

			var mipmaps = ComputeMipmaps(width, height).ToArray();
			writer.WriteInt32(mipmaps.Length);
			foreach (var mipmap in mipmaps)
			{
				var scale = new Size(width / mipmap.Width, height / mipmap.Height);
				writer.WriteByteArray(GenerateMipmap(data, mipmap, scale, format));
			}
		}
	}
}