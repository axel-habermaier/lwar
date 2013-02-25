using System;

namespace Pegasus.AssetsCompiler
{
	using System.ComponentModel;
	using System.IO;
	using System.Text;
	using Framework;
	using Framework.Platform;
	using SharpDX.D3DCompiler;

	/// <summary>
	///   Provides common methods required for the compilation of shaders.
	/// </summary>
	internal abstract class ShaderCompiler : AssetCompiler
	{
		/// <summary>
		///   Indicates whether HLSL shaders should be compiled.
		/// </summary>
		protected static readonly bool CompileHlsl;

		/// <summary>
		///   Initializes the type.
		/// </summary>
		static ShaderCompiler()
		{
			try
			{
				ExternalProcess.Run("fxc", "/?");
				CompileHlsl = true;
			}
			catch (Win32Exception e)
			{
				if (e.NativeErrorCode == 2)
				{
					Log.Warn("HLSL shaders will not be compiled as fxc.exe could not be found.");
					switch (PlatformInfo.Platform)
					{
						case PlatformType.Linux:
							Log.Warn("HLSL shader compilation is not supported on Linux.");
							break;
						case PlatformType.Windows:
							Log.Warn("fxc.exe must be in the system path.");
							break;
						default:
							throw new InvalidOperationException("Unknown platform.");
					}
				}
				else
					Log.Error("Unable to invoke the HLSL compiler; HLSL shaders will not be compiled: {0}", e.Message);

				CompileHlsl = false;
			}
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		protected ShaderCompiler(string asset)
			: base(asset)
		{
		}

		/// <summary>
		///   Extracts the GLSL and HLSL shader code from the given shader asset.
		/// </summary>
		/// <param name="asset">The asset from which the shader code should be extracted.</param>
		/// <param name="glsl">The extracted GLSL shader code.</param>
		/// <param name="hlsl">The extracted HLSL shader code.</param>
		protected void ExtractShaderCode(Asset asset, out string glsl, out string hlsl)
		{
			var split = File.ReadAllText(asset.SourcePath).Split(new[] { "---" }, StringSplitOptions.RemoveEmptyEntries);
			if (split.Length != 2)
				Log.Die("GLSL and HLSL shader code must be separated by '---'.");

			glsl = split[0];
			hlsl = split[1];
		}

		/// <summary>
		///   Writes an GLSL shader into the buffer.
		/// </summary>
		/// <param name="source">The GLSL shader source code.</param>
		protected void WriteGlslShader(string source)
		{
			var shader = "#version 330\n#extension GL_ARB_shading_language_420pack : enable\n" +
						 "#extension GL_ARB_separate_shader_objects : enable\n" + source;

			Buffer.Copy(Encoding.UTF8.GetBytes(shader));
			Buffer.WriteByte(0);
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