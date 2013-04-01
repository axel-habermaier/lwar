using System;

namespace Pegasus.AssetsCompiler.Assets
{
	using Framework;
	using Framework.Platform.Graphics;

	/// <summary>
	///   Represents a vertex shader that requires compilation.
	/// </summary>
	public class VertexShaderAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public VertexShaderAsset(string relativePath)
			: base(relativePath)
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		/// <param name="sourceDirectory">The source directory of the asset.</param>
		/// <param name="shaderInput">Describes the vertex data input layout of the vertex shader.</param>
		public VertexShaderAsset(string relativePath, string sourceDirectory, ShaderInput[] shaderInput)
			: base(relativePath, sourceDirectory)
		{
			Assert.ArgumentNotNull(shaderInput, () => shaderInput);
			ShaderInputs = shaderInput;
		}

		/// <summary>
		///   Gets the input layout for the vertex shader.
		/// </summary>
		public ShaderInput[] ShaderInputs { get; private set; }
	}
}