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
		///   Gets a description of the asset type that the compiler supports.
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

			WriteGlslShader(glsl);
			IfD3DSupported(() =>
				{
					using (var byteCode = CompileHlslShader(Asset, hlsl, "ps_4_0"))
						Buffer.WriteByteArray(byteCode);
				});
		}
	}
}