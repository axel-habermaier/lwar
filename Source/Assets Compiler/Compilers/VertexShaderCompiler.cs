using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using Assets;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Compiles vertex shaders.
	/// </summary>
	[UsedImplicitly]
	internal class VertexShaderCompiler : ShaderCompiler<VertexShaderAsset>
	{
		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void CompileCore(VertexShaderAsset asset, BufferWriter buffer)
		{
			string glsl, hlsl;
			ExtractShaderCode(asset, out glsl, out hlsl);

			buffer.WriteByte((byte)asset.ShaderInputs.Length);
			foreach (var input in asset.ShaderInputs)
			{
				buffer.WriteByte((byte)input.Format);
				buffer.WriteByte((byte)input.Semantics);
			}

			CompileGlslShader(buffer, glsl);
			if (Configuration.CompileHlsl)
				CompileHlslShader(asset, buffer, hlsl, "vs_4_0");
		}
	}
}