using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   A vertex shader is a program that controls the vertex-shader stage.
	/// </summary>
	public sealed class VertexShader : Shader
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="shaderData">The shader source data.</param>
		public VertexShader(GraphicsDevice graphicsDevice, byte[] shaderData)
			: base(graphicsDevice, ShaderType.VertexShader, shaderData)
		{
		}

		/// <summary>
		///   Binds the vertex shader to the pipeline.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			if (DeviceState.VertexShader == this)
				return;

			DeviceState.VertexShader = this;
			BindShader();
		}
	}
}