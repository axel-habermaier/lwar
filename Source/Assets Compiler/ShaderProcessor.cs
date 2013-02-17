using System;

namespace Pegasus.AssetsCompiler
{
	using System.IO;
	using Framework;
	using Framework.Platform;
	using SharpDX.D3DCompiler;

	public abstract class ShaderProcessor : AssetProcessor
	{
		/// <summary>
		///   Writes an GLSL shader into the buffer.
		/// </summary>
		/// <param name="source">The path of the shader source code.</param>
		/// <param name="writer">The buffer the shader should be written into.</param>
		protected void WriteGlslShader(string source, BufferWriter writer)
		{
			var shader = "#version 330\n#extension GL_ARB_shading_language_420pack : enable\n" +
						 "#extension GL_ARB_separate_shader_objects : enable\n" + File.ReadAllText(source + ".glsl");

			writer.WriteString(shader);
			writer.WriteByte(0);
		}

		/// <summary>
		///   Compiles an HLSL shader of the given profile.
		/// </summary>
		/// <param name="source">The path of the shader source code.</param>
		/// <param name="profile">The profile that should be used to compile the shader.</param>
		protected ShaderBytecode CompileHlslShader(string source, string profile)
		{
			var flags = ShaderFlags.EnableStrictness;
#if DEBUG
			flags |= ShaderFlags.Debug;
#endif
			source += ".hlsl";
			var result = ShaderBytecode.Compile(File.ReadAllText(source), "Main", profile, flags, EffectFlags.None, source);
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
				action();
			}
			catch (DllNotFoundException)
			{
			}
		}
	}
}