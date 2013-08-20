using System;

namespace Pegasus.Platform.Graphics
{
	/// <summary>
	///   Describes the type of a shader.
	/// </summary>
	public enum ShaderType
	{
		/// <summary>
		///   Indicates that a shader is a vertex shader.
		/// </summary>
		VertexShader = 2601,

		/// <summary>
		///   Indicates that a shader is a fragment shader.
		/// </summary>
		FragmentShader = 2602,

		/// <summary>
		///   Indicates that a shader is a geometry shader.
		/// </summary>
		GeometryShader = 2603
	}
}