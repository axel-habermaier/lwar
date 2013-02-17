using System;

namespace Pegasus.AssetsCompiler
{
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
			writer.WriteInt32(width);
			writer.WriteInt32(height);
			writer.WriteInt32((int)format);
			writer.Copy(data);
		}
	}
}