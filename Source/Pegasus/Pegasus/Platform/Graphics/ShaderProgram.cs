namespace Pegasus.Platform.Graphics
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents a combination of different shader programs that control the various pipeline stages of the GPU.
	/// </summary>
	public sealed unsafe class ShaderProgram : GraphicsObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="vertexShader">The vertex shader that the shader program should use.</param>
		/// <param name="fragmentShader">The fragment shader that the shader program should use.</param>
		public ShaderProgram(GraphicsDevice graphicsDevice, VertexShader vertexShader, FragmentShader fragmentShader)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(vertexShader);
			Assert.ArgumentNotNull(fragmentShader);

			NativeObject = DeviceInterface->InitializeShaderProgram(vertexShader.NativeObject, fragmentShader.NativeObject);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceInterface->FreeShaderProgram(NativeObject);
		}

		/// <summary>
		///     Binds the shaders of the shader program to the various pipeline stages.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			if (DeviceState.Change(ref DeviceState.ShaderProgram, this))
				DeviceInterface->BindShaderProgram(NativeObject);
		}
	}
}