using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.IO;
	using System.Linq;
	using System.Text;
	using Assets;
	using Framework;
	using Framework.Platform;
	using SharpDX.D3DCompiler;

	/// <summary>
	///   Provides common methods required for the compilation of shaders.
	/// </summary>
	/// <typeparam name="TShader">The type of the shader that is compiled.</typeparam>
	internal abstract class ShaderCompiler<TShader> : AssetCompiler<TShader>
		where TShader : Asset
	{
		/// <summary>
		///   Extracts the GLSL and HLSL shader code from the given shader asset.
		/// </summary>
		/// <param name="asset">The asset from which the shader code should be extracted.</param>
		/// <param name="glsl">The extracted GLSL shader code.</param>
		/// <param name="hlsl">The extracted HLSL shader code.</param>
		protected static void ExtractShaderCode(Asset asset, out string glsl, out string hlsl)
		{
			var split = File.ReadAllText(asset.SourcePath)
							.Split(new[] { Configuration.ShaderSeparator }, StringSplitOptions.RemoveEmptyEntries);
			if (split.Length != 2)
				Log.Die("GLSL and HLSL shader code must be separated by '{0}'.", Configuration.ShaderSeparator);

			glsl = split[0];
			hlsl = split[1];
		}

		/// <summary>
		///   Writes an GLSL shader into the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		/// <param name="source">The GLSL shader source code.</param>
		protected static void WriteGlslShader(BufferWriter buffer, string source)
		{
			var shader = "#version 330\n#extension GL_ARB_shading_language_420pack : enable\n" +
						 "#extension GL_ARB_separate_shader_objects : enable\n" + source;

			var glslCode = Encoding.UTF8.GetBytes(shader);
			buffer.WriteInt32(glslCode.Length + 1);
			buffer.Copy(glslCode);
			buffer.WriteByte(0);
		}

		/// <summary>
		///   Compiles an HLSL shader of the given profile.
		/// </summary>
		/// <param name="asset">The asset that contains the shader source code.</param>
		/// <param name="source">The HLSL shader source code.</param>
		/// <param name="profile">The profile that should be used to compile the shader.</param>
		protected ShaderBytecode CompileHlslShader(Asset asset, string source, string profile)
		{
			var hlslFile = asset.TempPathWithoutExtension + ".hlsl";
			File.WriteAllText(hlslFile, source);

			var byteCode = asset.TempPathWithoutExtension + ".fxo";
			ExternalTool.Fxc(hlslFile, byteCode, profile);

			return new ShaderBytecode(File.ReadAllBytes(byteCode));
		}
	}
}