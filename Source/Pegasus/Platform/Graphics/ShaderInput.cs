using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a shader input.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct ShaderInput
	{
		/// <summary>
		///   Gets or sets the data format of the shader input.
		/// </summary>
		public VertexDataFormat Format { get; set; }

		/// <summary>
		///   Gets or sets the semantics of the shader input.
		/// </summary>
		public DataSemantics Semantics { get; set; }
	}
}