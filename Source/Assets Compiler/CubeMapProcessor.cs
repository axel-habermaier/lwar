using System;

namespace Pegasus.AssetsCompiler
{
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Runtime.InteropServices;
	using Framework.Platform;
	using Framework.Platform.Graphics;

	/// <summary>
	///   Processes cubemap textures, converting them to a premultiplied format.
	/// </summary>
	public sealed class CubeMapProcessor : TextureProcessor
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
			width /= 6;
			writer.WriteInt32(width);
			writer.WriteInt32(height);
			writer.WriteInt32((int)format);

			var componentCount = ComponentCount(format);
			var buffer = new byte[width * height * componentCount];

			for (var i = 0; i < 6; ++i)
			{
				for (var j = 0; j < height; ++j)
					Array.Copy(data, i * width * componentCount + j * width * componentCount * 6, buffer, j * width * componentCount, width * componentCount);

				WriteMipmaps(buffer, width, height, format, writer);
			}
		}
	}
}