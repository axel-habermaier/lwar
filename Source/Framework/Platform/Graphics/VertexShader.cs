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
		public VertexShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice, ShaderType.VertexShader)
		{
		}
	}
}