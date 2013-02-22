using System;

namespace Pegasus.AssetsCompiler
{
	using Framework.Platform;

	/// <summary>
	///   Processes vertex shaders.
	/// </summary>
	public class FragmentShaderProcessor : ShaderProcessor
	{
		/// <summary>
		///   Processes the given file, writing the compiled output to the given target destination.
		/// </summary>
		/// <param name="asset">The asset that should be processed.</param>
		/// <param name="writer">The writer that should be used to write the compiled asset file.</param>
		public override void Process(Asset asset, BufferWriter writer)
		{
			string glsl, hlsl;
			ExtractShaderCode(asset, out glsl, out hlsl);

			WriteGlslShader(glsl, writer);
			IfD3DSupported(() =>
				{
					using (var byteCode = CompileHlslShader(asset, hlsl, "ps_4_0"))
						writer.WriteByteArray(byteCode);
				});
		}
	}
}