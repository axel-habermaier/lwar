namespace Pegasus.AssetsCompiler.Effects
{
	using System;

	/// <summary>
	///     Represents a combination of shaders that are set on the GPU to create a rendering effect.
	/// </summary>
	public sealed class Technique
	{
		/// <summary>
		///     Gets or sets the vertex shader that should be bound.
		/// </summary>
		public string VertexShader { get; set; }

		/// <summary>
		///     Gets or sets the fragment shader that should be bound.
		/// </summary>
		public string FragmentShader { get; set; }
	}
}