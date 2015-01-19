namespace Pegasus.Platform.Graphics
{
	using System;
	using Memory;
	using Utilities;

	/// <summary>
	///     A vertex shader is a program that controls the vertex-shader stage.
	/// </summary>
	public sealed class VertexShader : Shader
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		private VertexShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///     Loads a vertex shader from the given buffer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device the vertex shader should be created for.</param>
		/// <param name="buffer">The buffer the vertex shader should be read from.</param>
		public static VertexShader Create(GraphicsDevice graphicsDevice, ref BufferReader buffer)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			var shader = new VertexShader(graphicsDevice);
			shader.Load(ref buffer);
			return shader;
		}

		/// <summary>
		///     Loads the vertex shader from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the vertex shader should be read from.</param>
		public void Load(ref BufferReader buffer)
		{
			Load(ShaderType.VertexShader, ref buffer);
		}
	}
}