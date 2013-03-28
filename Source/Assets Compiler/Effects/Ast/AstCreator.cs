using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using Compilation;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Creates an abstract syntax tree for the shader code from the syntax tree of a C# method.
	/// </summary>
	internal partial class AstCreator : IAstVisitor<IAstNode>
	{
		/// <summary>
		///   The context of the compilation.
		/// </summary>
		private CompilationContext _context;

		/// <summary>
		///   The effect class the shader method belongs to.
		/// </summary>
		private EffectClass _effect;

		/// <summary>
		///   The C# shader method that is cross-compiled.
		/// </summary>
		private ShaderMethod _shader;

		/// <summary>
		///   Creates an abstract syntax tree for the shader code from the syntax tree of the C# shader method.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		/// <param name="effect">The effect class the shader method belongs to.</param>
		/// <param name="shader">The C# shader method that should be cross-compiled.</param>
		public IAstNode CreateAst(CompilationContext context, EffectClass effect, ShaderMethod shader)
		{
			Assert.ArgumentNotNull(context, () => context);
			Assert.ArgumentNotNull(effect, () => effect);
			Assert.ArgumentNotNull(shader, () => shader);

			_context = context;
			_effect = effect;
			_shader = shader;

			return shader.ShaderCode.AcceptVisitor(this);
		}
	}
}