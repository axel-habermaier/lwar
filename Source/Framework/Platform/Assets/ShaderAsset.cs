using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Graphics;

	/// <summary>
	///   Represents a shader asset.
	/// </summary>
	internal abstract class ShaderAsset<T> : Asset
		where T : Shader
	{
		/// <summary>
		///   The shader that is managed by this asset instance.
		/// </summary>
		public T Shader { get; protected set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override sealed void OnDisposing()
		{
			Shader.SafeDispose();
		}

		/// <summary>
		///   Extracts the graphics API-dependent shader code from the buffer.
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
					throw new InvalidOperationException("Unsupported Graphics API.");
			}
		}
	}
}