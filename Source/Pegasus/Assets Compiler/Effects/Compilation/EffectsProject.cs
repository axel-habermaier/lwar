namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Collections.Generic;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using Utilities;

	/// <summary>
	///     Represents a C# project with effect declarations that have to be cross-compiled.
	/// </summary>
	internal class EffectsProject : CSharpProject<EffectFile>
	{
		/// <summary>
		///     The effect files contained in the project.
		/// </summary>
		private readonly List<EffectFile> _effectFiles = new List<EffectFile>();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="file">The file that should be compiled.</param>
		/// <param name="context">The compilation context of the effect.</param>
		public EffectsProject(CSharpFile file, CompilationContext context)
		{
			Assert.ArgumentNotNull(context);

			Context = context;
			CSharpFiles = new[] { file };
		}

		/// <summary>
		///     Gets the compilation context.
		/// </summary>
		public CompilationContext Context { get; private set; }

		/// <summary>
		///     Gets the effect files contained in the project.
		/// </summary>
		public IEnumerable<EffectFile> EffectFiles
		{
			get { return _effectFiles; }
		}

		/// <summary>
		///     Creates a code element representing the a file.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <param name="syntaxTree">The syntax tree of the file.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information within the file.</param>
		protected override EffectFile CreateFile(string fileName, SyntaxTree syntaxTree, CSharpAstResolver resolver)
		{
			var effectFile = new EffectFile(this, syntaxTree, resolver) { Context = Context };
			_effectFiles.Add(effectFile);
			return effectFile;
		}

		/// <summary>
		///     Compiles the given file.
		/// </summary>
		/// <param name="file">The file that should be compiled.</param>
		protected override void Compile(EffectFile file)
		{
			file.Compile();
		}
	}
}