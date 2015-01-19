namespace Pegasus.Rendering
{
	using System;
	using Platform.Graphics;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents a combination of shaders that can be set on the GPU to create a rendering effect.
	/// </summary>
	public abstract class EffectTechnique : DisposableObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used by the technique.</param>
		/// <param name="vertexShader">The vertex shader that should be used by the technique.</param>
		/// <param name="fragmentShader">The fragment shader that should be used by the technique.</param>
		protected EffectTechnique(GraphicsDevice graphicsDevice, VertexShader vertexShader, FragmentShader fragmentShader)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(vertexShader);
			Assert.ArgumentNotNull(fragmentShader);

			ShaderProgram = new ShaderProgram(graphicsDevice, vertexShader, fragmentShader);
		}

		/// <summary>
		///     Gets the shader program that is used by the technique.
		/// </summary>
		protected ShaderProgram ShaderProgram { get; private set; }

		/// <summary>
		///     Binds the shaders, textures, and constant buffers used by the technique.
		/// </summary>
		public abstract void Bind();

		/// <summary>
		///     Unbinds the textures used by the technique.
		/// </summary>
		public abstract void Unbind();

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			ShaderProgram.SafeDispose();
		}
	}
}