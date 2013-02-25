using System;

namespace Pegasus.AssetsCompiler
{
	/// <summary>
	///   Compiles fragment shaders.
	/// </summary>
	internal class FragmentShaderCompiler : ShaderCompiler
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		public FragmentShaderCompiler(string asset)
			: base(asset)
		{
		}

		/// <summary>
		///   Gets a description of the type of the asset that is compiled by the compiler.
		/// </summary>
		internal override string AssetType
		{
			get { return "Fragment Shaders"; }
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		protected override void CompileCore()
		{
			string glsl, hlsl;
			ExtractShaderCode(Asset, out glsl, out hlsl);

			Buffer.WriteByte(0); // No shader inputs
			WriteGlslShader(glsl);
			if (CompileHlsl)
			{
				using (var byteCode = CompileHlslShader(Asset, hlsl, "ps_4_0"))
					Buffer.Copy(byteCode);
			}
		}
	}
}