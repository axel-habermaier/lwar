namespace Pegasus.Platform.Graphics
{
	using System;
	using Logging;
	using Memory;

	/// <summary>
	///     Represents a shader that controls a programmable stage of the graphics pipeline.
	/// </summary>
	public abstract unsafe class Shader : GraphicsObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		protected Shader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///     Gets the function that should be used to set the debug name of the native object.
		/// </summary>
		protected override SetNameDelegate SetNameFunction
		{
			get { return DeviceInterface->SetShaderName; }
		}

		/// <summary>
		///     Loads the shader from the given buffer.
		/// </summary>
		/// <param name="shaderType">The type of the shader that should be loaded.</param>
		/// <param name="buffer">The buffer the fragment shader should be read from.</param>
		protected void Load(ShaderType shaderType, ref BufferReader buffer)
		{
			byte* shaderCode;
			int length;
			ExtractShaderCode(ref buffer, out shaderCode, out length);

			DeviceInterface->FreeShader(NativeObject);
			NativeObject = DeviceInterface->InitializeShader((int)shaderType, shaderCode, length);

			SetName();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceInterface->FreeShader(NativeObject);
		}

		/// <summary>
		///     Extracts the graphics API-dependent shader code from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer that should be used to load the shader.</param>
		/// <param name="shaderCode">The extracted shader source code.</param>
		/// <param name="length">The length of the extracted shader code in bytes.</param>
		private void ExtractShaderCode(ref BufferReader buffer, out byte* shaderCode, out int length)
		{
			var containsD3D11Shader = buffer.ReadBoolean();
			var containsGL3Shader = buffer.ReadBoolean();

			switch (GraphicsDevice.GraphicsApi)
			{
				case GraphicsApi.Direct3D11:
					if (!containsD3D11Shader)
						Log.Die("The HLSL version of the shader cannot be loaded as it was not compiled into the asset bundle.");

					if (containsGL3Shader)
						buffer.Skip(buffer.ReadInt32());

					length = buffer.ReadInt32();
					shaderCode = buffer.Pointer;
					buffer.Skip(length);
					break;
				case GraphicsApi.OpenGL3:
					if (!containsGL3Shader)
						Log.Die("The OpenGL 3 version of the shader cannot be loaded as it was not compiled into the asset bundle.");

					length = buffer.ReadInt32();
					shaderCode = buffer.Pointer;
					buffer.Skip(length);

					if (containsD3D11Shader)
						buffer.Skip(buffer.ReadInt32());
					break;
				default:
					throw new InvalidOperationException("Unsupported graphics API.");
			}
		}
	}
}