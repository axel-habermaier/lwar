namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents a shader signature.
	/// </summary>
	public struct ShaderSignature
	{
		/// <summary>
		///     The compiled shader signature byte code.
		/// </summary>
		public readonly byte[] ByteCode;

		/// <summary>
		///     The inputs of the shader.
		/// </summary>
		public readonly ShaderInput[] Inputs;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="inputs">The inputs of the shader.</param>
		public ShaderSignature(ShaderInput[] inputs)
		{
			Assert.ArgumentNotNull(inputs);
			Assert.ArgumentSatisfies(inputs.Length > 0, "Expected at least one input.");

			Inputs = inputs;
			ByteCode = null;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="inputs">The inputs of the shader.</param>
		/// <param name="byteCode">The compiled shader signature byte code.</param>
		public ShaderSignature(ShaderInput[] inputs, byte[] byteCode)
		{
			Assert.ArgumentNotNull(inputs);
			Assert.ArgumentNotNull(byteCode);
			Assert.ArgumentSatisfies(inputs.Length > 0, "Expected at least one input.");

			Inputs = inputs;
			ByteCode = byteCode;
		}
	}
}