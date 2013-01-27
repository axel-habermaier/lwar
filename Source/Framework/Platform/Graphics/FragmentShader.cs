using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   A fragment shader is a program that controls the fragment-shader stage.
	/// </summary>
	public sealed class FragmentShader : Shader
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="shaderData">The shader source data.</param>
		public FragmentShader(GraphicsDevice graphicsDevice, byte[] shaderData)
			: base(graphicsDevice, ShaderType.FragmentShader, shaderData)
		{
		}

		/// <summary>
		///   Binds the fragment shader to the pipeline.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			if (DeviceState.FragmentShader == this)
				return;

			DeviceState.FragmentShader = this;
			BindShader();
		}
	}
}