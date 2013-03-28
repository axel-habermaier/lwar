using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using Compilers;
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
		public IEnumerable<Asset> Compile(CompilationContext context)
		{
			if (File.Exists(Asset.HashPath))
			{
				var oldHash = Hash.FromFile(Asset.HashPath);
				var newHash = Hash.Compute(Asset.SourcePath);

				//if (oldHash == newHash)
				//{
				//	CompilationAction.Skip.Describe(Asset);
				//	yield break;
				//}
			}

			CompilationAction.Process.Describe(Asset);
			Asset.WriteHash();
			PrintParserErrorsAndWarnings(context);

			var effectClasses = from type in SyntaxTree.DescendantsAndSelf.OfType<TypeDeclaration>()
								where type.ClassType == ClassType.Class
								let hasBaseType = type.IsDerivedFrom<Effect>(context)
								let hasAttribute = type.HasAttribute<EffectAttribute>(context)
								where hasBaseType || hasAttribute
								select new { Effect = new EffectClass(type), Type = type, HasAttribute = hasAttribute, HasBaseType = hasBaseType };

			foreach (var effectClass in effectClasses)
			{
				var type = effectClass.Type;

				if (effectClass.HasBaseType && !effectClass.HasAttribute)
					context.Warn(type.NameToken,
								 "Expected attribute '{0}' to be declared on effect '{1}'.",
								 typeof(EffectAttribute).FullName, type.GetFullName(context));

				if (!effectClass.HasBaseType && effectClass.HasAttribute)
					context.Warn(type.NameToken,
								 "Expected effect '{0}' to have base type '{1}'.",
								 type.GetFullName(context), typeof(Effect).FullName);

				effectClass.Effect.Compile(context);
				foreach (var shader in effectClass.Effect.Shaders)
					yield return shader.Asset;
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