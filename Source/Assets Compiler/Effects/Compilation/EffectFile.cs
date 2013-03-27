using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.IO;
	using System.Linq;
	using Assets;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.TypeSystem;
	using ICSharpCode.NRefactory.Semantics;
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

			var effects = SyntaxTree.DescendantsAndSelf.OfType<TypeDeclaration>()
									.Where(t => t.ClassType == ClassType.Class)
									.Where(t =>
										{
											var resolved = (TypeResolveResult)context.Resolver.Resolve(t);

											var hasBaseType = resolved.Type.DirectBaseTypes
																	  .Any(b => b.FullName == typeof(Effect).FullName);

											var hasAttribute = t.Attributes
																.SelectMany(s => s.Attributes)
																.Select(a => context.Resolver.Resolve(a))
																.Any(a => a.Type.FullName == typeof(EffectAttribute).FullName);

											if (hasBaseType && !hasAttribute)
												context.Warn(t.NameToken.StartLocation, t.NameToken.EndLocation,
															 "Expected attribute '{0}' to be declared on effect '{1}'.",
															 typeof(EffectAttribute).FullName, resolved.Type.FullName);

											if (!hasBaseType && hasAttribute)
												context.Warn(t.NameToken.StartLocation, t.NameToken.EndLocation,
															 "Expected effect '{0}' to have base type '{1}'.",
															 resolved.Type.FullName, typeof(Effect).FullName);

											return hasBaseType || hasAttribute;
										})
									.Select(t => new EffectClass(t));

			foreach (var effect in effects)
			{
				context.Effect = effect;
				effect.Compile(context);
			}
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
						context.Error(error.Region.Begin, error.Region.End, error.Message);
						break;
					case ErrorType.Warning:
						context.Warn(error.Region.Begin, error.Region.End, error.Message);
						break;
				}
			}
		}
	}
}