using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Assets;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;

	/// <summary>
	///   Represents a C# source code file that possibly contains one or more effect declarations.
	/// </summary>
	internal class EffectFile : CompiledElement
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="syntaxTree">The parsed syntax tree of the effect file.</param>
		/// <param name="resolver"> The C# AST resolver that should be used to resolve symbols of the effect file.</param>
		public EffectFile(SyntaxTree syntaxTree, CSharpAstResolver resolver)
			: base(syntaxTree.FileName, resolver)
		{
			SyntaxTree = syntaxTree;
		}

		/// <summary>
		///   Gets the C# syntax tree of the effect file.
		/// </summary>
		public SyntaxTree SyntaxTree { get; private set; }

		public IEnumerable<Asset> ShaderAssets {get
		{
			f
		}}

		/// <summary>
		///   Compiles the effect file.
		/// </summary>
		public void Compile()
		{
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
	}
}