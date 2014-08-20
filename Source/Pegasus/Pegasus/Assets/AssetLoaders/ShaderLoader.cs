namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using Platform;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///     Loads shader assets.
	/// </summary>
	public abstract class ShaderLoader : AssetLoader
	{
		/// <summary>
		///     Extracts the graphics API-dependent shader code from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer that should be used to load the shader.</param>
		/// <param name="shaderCode">The extracted shader source code.</param>
		/// <param name="length">The length of the extracted shader code in bytes.</param>
		protected static unsafe void ExtractShaderCode(BufferReader buffer, out byte* shaderCode, out int length)
		{
			switch (NativeLibrary.GraphicsApi)
			{
				case GraphicsApi.Direct3D11:
					buffer.Skip(buffer.ReadInt32());
					if (buffer.EndOfBuffer)
						Log.Die("The HLSL version of the shader cannot be loaded as it was not compiled into the shader asset file.");

					shaderCode = buffer.Pointer;
					length = buffer.BufferSize - buffer.Count;
					break;
				case GraphicsApi.OpenGL3:
					length = buffer.ReadInt32();
					shaderCode = buffer.Pointer;
					break;
				default:
					throw new InvalidOperationException("Unsupported graphics API.");
			}
		}
	}
}