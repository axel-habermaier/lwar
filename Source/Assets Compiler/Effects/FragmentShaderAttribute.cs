namespace Pegasus.AssetsCompiler.Effects
{
	using System;
	using Platform.Graphics;

	/// <summary>
	///     When applied to a method, indicates that the method should be cross-compiled into a fragment shader.
	/// </summary>
	public class FragmentShaderAttribute : ShaderAttribute
	{
		/// <summary>
		///     Gets the type of the shader.
		/// </summary>
		internal override ShaderType ShaderType
		{
			get { return ShaderType.FragmentShader; }
		}
	}
}