namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes the type of a shader.
	/// </summary>
	public enum ShaderType
	{
		/// <summary>
		///     Indicates that a shader is a vertex shader.
		/// </summary>
		VertexShader,

		/// <summary>
		///     Indicates that a shader is a fragment shader.
		/// </summary>
		FragmentShader,

		/// <summary>
		///     Indicates that a shader is a geometry shader.
		/// </summary>
		GeometryShader
	}
}