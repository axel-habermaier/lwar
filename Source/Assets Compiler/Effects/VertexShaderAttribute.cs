using System;

namespace Pegasus.AssetsCompiler.Effects
{
	using Framework.Platform.Graphics;

	/// <summary>
	///   When applied to a method, indicates that the method should be cross-compiled into a vertex shader.
	/// </summary>
	public class VertexShaderAttribute : ShaderAttribute
	{
		/// <summary>
		///   Gets the type of the shader.
		/// </summary>
		internal override ShaderType ShaderType
		{
			get { return ShaderType.VertexShader; }
		}
	}
}