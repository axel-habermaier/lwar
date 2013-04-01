using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using Assets;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Compiles fragment shaders.
	/// </summary>
	[UsedImplicitly]
	internal class FragmentShaderCompiler : ShaderCompiler<FragmentShaderAsset>
	{
		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void CompileCore(FragmentShaderAsset asset, BufferWriter buffer)
		{
			string glsl, hlsl;
			ExtractShaderCode(asset, out glsl, out hlsl);

			CompileGlslShader(buffer, glsl);
			if (Configuration.CompileHlsl)
				CompileHlslShader(asset, buffer, hlsl, "ps_4_0");
		}
	}
}