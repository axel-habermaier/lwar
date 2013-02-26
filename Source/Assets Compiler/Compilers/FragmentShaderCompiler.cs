using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Threading.Tasks;
	using Assets;
	using Framework.Platform;

	/// <summary>
	///   Compiles fragment shaders.
	/// </summary>
	internal class FragmentShaderCompiler : ShaderCompiler<FragmentShaderAsset>
	{
		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override async Task CompileCore(Asset asset, BufferWriter buffer)
		{
			string glsl, hlsl;
			ExtractShaderCode(asset, out glsl, out hlsl);

			buffer.WriteByte(0); // No shader inputs
			WriteGlslShader(buffer, glsl);
			if (CompileHlsl)
			{
				using (var byteCode = await CompileHlslShader(asset, hlsl, "ps_4_0"))
					buffer.Copy(byteCode);
			}
		}
	}
}