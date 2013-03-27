using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.IO;
	using System.Linq;
	using Assets;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.TypeSystem;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a C# source code file that possibly contains one or more effect declarations.
	/// </summary>
	internal class EffectFile
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="asset">The C# asset file that the effect file should represent.</param>
		public EffectFile(CSharpAsset asset)
		{
			Assert.ArgumentNotNull(asset, () => asset);

			var parser = new CSharpParser();

			SyntaxTree = parser.Parse(File.ReadAllText(asset.SourcePath), asset.SourcePath);
			UnresolvedFile = SyntaxTree.ToTypeSystem();
			Asset = asset;
		}

		/// <summary>
		///   Gets the C# asset file that the effect file represents.
		/// </summary>
		public CSharpAsset Asset { get; private set; }

		/// <summary>
		///   Gets the C# syntax tree of the effect file.
		/// </summary>
		public SyntaxTree SyntaxTree { get; private set; }

		/// <summary>
		///   Gets the unresolved file that represents the effect file in the project.
		/// </summary>
		public CSharpUnresolvedFile UnresolvedFile { get; private set; }

		/// <summary>
		///   Compiles the effect file.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		public void Compile(CompilationContext context)
		{
			PrintParserErrorsAndWarnings(context);
		}

		/// <summary>
		///   Prints all parser errors and warnings.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		private void PrintParserErrorsAndWarnings(CompilationContext context)
		{
			foreach (var error in UnresolvedFile.Errors)
			{
				switch (error.ErrorType)
				{
					default:
						context.Error(error.Message, error.Region.Begin, error.Region.End);
						break;
					case ErrorType.Warning:
						context.Warn(error.Message, error.Region.Begin, error.Region.End);
						break;
				}
			}
		}

		//private static void Compile(ICompilation compilation, CSharpUnresolvedFile file, SyntaxTree syntaxTree)
		//{
		//	var resolver = new CSharpAstResolver(compilation, syntaxTree, file);
		//	var effects = syntaxTree.DescendantsAndSelf.OfType<TypeDeclaration>()
		//							.Where(t => t.ClassType == ClassType.Class);
		//	// && t.BaseTypes.Any(b => b.ToTypeReference().Resolve(resolver.TypeResolveContext).));

		//	if (!effects.Any())
		//		return;

		//	var resolved = resolver.Resolve(effects.First()) as TypeResolveResult;
		//	var baseType = resolved.Type.DirectBaseTypes.First();
		//	var s = baseType.FullName == typeof(Effect).FullName;

		//	return;
		//}
	}
}