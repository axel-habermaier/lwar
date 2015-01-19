namespace Pegasus.AssetsCompiler.Effects
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Represents a shader input.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct ShaderInput
	{
		/// <summary>
		///     Gets or sets the data format of the shader input.
		/// </summary>
		public VertexDataFormat Format { get; set; }

		/// <summary>
		///     Gets or sets the semantics of the shader input.
		/// </summary>
		public DataSemantics Semantics { get; set; }

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="format">The data format of the shader input.</param>
		/// <param name="semantics">The semantics of the shader input.</param>
		public ShaderInput(VertexDataFormat format, DataSemantics semantics)
			: this()
		{
			Format = format;
			Semantics = semantics;
		}

		/// <summary>
		///     Returns a string representation for this instance.
		/// </summary>
		public override string ToString()
		{
			return String.Format("Format: {0}, Semantics: {1}", Format, Semantics);
		}
	}
}