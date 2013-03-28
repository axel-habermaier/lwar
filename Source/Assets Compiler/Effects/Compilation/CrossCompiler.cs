using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	/// <summary>
	///   Cross-compiles a C# shader method into a GLSL/HLSL shader file.
	/// </summary>
	internal class CrossCompiler
	{
		/// <summary>
		///   The C# shader method that is cross-compiled.
		/// </summary>
		private readonly ShaderMethod _shader;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="shader">The C# shader method that should be cross-compiled.</param>
		public CrossCompiler(ShaderMethod shader)
		{
			_shader = shader;
		}

		/// <summary>
		///   Generates the code for the shader.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		/// <param name="effect">The effect the shader belongs to.</param>
		public void GenerateCode(CompilationContext context, EffectClass effect)
		{
			var writer = new CodeWriter();

			writer.AppendLine("GLSL");
			writer.AppendLine("---");
			writer.AppendLine("HLSL");

			writer.WriteToFile(_shader.Asset.SourcePath);
		}
	}
}