using System;

namespace Pegasus.AssetsCompiler
{
	using System.IO;
	using Framework;
	using SharpDX.D3DCompiler;

	/// <summary>
	///   Provides common methods required for the compilation of shaders.
	/// </summary>
	internal abstract class ShaderCompiler : AssetCompiler
	{
		/// <summary>
		///   The object used for thread synchronization.
		/// </summary>
		private static readonly object SyncObject = new object();

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

			Buffer.WriteString(shader);
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
			//var hlslFile = asset.TempPath + ".hlsl";
			//File.WriteAllText(hlslFile, source);

			//var byteCode = asset.TempPath + ".bytecode";
			//ExternalTool.Fxc(hlslFile, byteCode, profile);

			//return new ShaderBytecode(File.ReadAllBytes(byteCode));

			var flags = ShaderFlags.EnableStrictness;
#if DEBUG
			flags |= ShaderFlags.Debug;
#endif

			var result = ShaderBytecode.Compile(source, "Main", profile, flags, EffectFlags.None, source);
			if (result.HasErrors)
				throw new InvalidOperationException(result.Message);

			return result.Bytecode;
		}

		/// <summary>
		///   Executes the given action only if the platform supports Direct3D.
		/// </summary>
		/// <param name="action">The action that should be executed.</param>
		protected void IfD3DSupported(Action action)
		{
			Assert.ArgumentNotNull(action, () => action);

			try
			{
				lock (SyncObject)
					action();
			}
			catch (DllNotFoundException)
			{
			}
		}
	}
}