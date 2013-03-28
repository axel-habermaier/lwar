using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Cross-compiles a C# shader method.
	/// </summary>
	internal abstract class CrossCompiler : DepthFirstAstVisitor
	{
		/// <summary>
		///   The C# shader method that is cross-compiled.
		/// </summary>
		protected ShaderMethod Shader { get; private set; }

		/// <summary>
		///   The context of the compilation.
		/// </summary>
		protected CompilationContext Context { get; private set; }

		/// <summary>
		///   The effect class the shader method belongs to.
		/// </summary>
		protected EffectClass Effect { get; private set; }

		/// <summary>
		///   The code writer the generated code should be written to.
		/// </summary>
		protected CodeWriter Writer { get; private set; }

		/// <summary>
		///   Cross-compiles the C# shader method.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		/// <param name="effect">The effect class the shader method belongs to.</param>
		/// <param name="shader">The C# shader method that should be cross-compiled.</param>
		/// <param name="writer">The code writer the generated code should be written to.</param>
		public void GenerateCode(CompilationContext context, EffectClass effect, ShaderMethod shader, CodeWriter writer)
		{
			Assert.ArgumentNotNull(context, () => context);
			Assert.ArgumentNotNull(effect, () => effect);
			Assert.ArgumentNotNull(shader, () => shader);
			Assert.ArgumentNotNull(writer, () => writer);

			Context = context;
			Effect = effect;
			Shader = shader;
			Writer = writer;

			foreach (var literal in effect.Literals)
				GenerateLiteral(literal);

			Writer.Newline();
			foreach (var constantBuffer in effect.ConstantBuffers)
				GenerateConstantBuffer(constantBuffer);

			foreach (var texture in effect.Textures)
				GenerateTextureObject(texture);

			shader.ShaderCode.AcceptVisitor(this);
		}

		/// <summary>
		///   Generates the shader code for shader literals.
		/// </summary>
		/// <param name="literal">The shader literal that should be generated.</param>
		protected abstract void GenerateLiteral(ShaderLiteral literal);

		/// <summary>
		///   Generates the shader code for shader constant buffers.
		/// </summary>
		/// <param name="constantBuffer">The constant buffer that should be generated.</param>
		protected abstract void GenerateConstantBuffer(ConstantBuffer constantBuffer);

		/// <summary>
		///   Generates the shader code for texture objects.
		/// </summary>
		/// <param name="texture">The shader texture that should be generated.</param>
		protected abstract void GenerateTextureObject(ShaderTexture texture);
	}
}