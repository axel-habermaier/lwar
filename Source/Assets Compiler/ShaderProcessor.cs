using System;

namespace Pegasus.AssetsCompiler
{
	using System.IO;
	using Framework;
	using Framework.Platform;
	using SharpDX.D3DCompiler;

	public abstract class ShaderProcessor : AssetProcessor
	{
		protected void WriteGlslShader(string source, BufferWriter writer)
		{
			var shader = "#version 330\n#extension GL_ARB_shading_language_420pack : enable\n" +
						 "#extension GL_ARB_separate_shader_objects : enable\n" + File.ReadAllText(source + ".glsl");

			writer.WriteString(shader);
			writer.WriteByte(0);
		}

		protected ShaderBytecode CompileHlslShader(string source, string profile)
		{
			var flags = ShaderFlags.EnableStrictness;
#if DEBUG
			flags |= ShaderFlags.Debug;
#endif
			var result = ShaderBytecode.Compile(File.ReadAllText(source + ".hlsl"), "Main", profile, flags, EffectFlags.None, source);
			if (result.HasErrors)
				throw new InvalidOperationException(result.Message);

			return result.Bytecode;
		}

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