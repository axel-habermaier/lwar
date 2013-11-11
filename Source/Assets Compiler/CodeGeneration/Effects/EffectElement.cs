namespace Pegasus.AssetsCompiler.CodeGeneration.Effects
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents an element of an effect declaration.
	/// </summary>
	internal abstract class EffectElement : CodeElement
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="errorReporter">The error reporter that should be used to report validation errors.</param>
		/// <param name="sourceFile">The path to the C# file that contains the element.</param>
		/// <param name="resolver">The resolver that should be used to resolve symbols of the C# file currently being analyzed.</param>
		protected EffectElement(IErrorReporter errorReporter, string sourceFile, CSharpAstResolver resolver)
			: base(errorReporter, sourceFile, resolver)
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected EffectElement()
		{
		}

		/// <summary>
		///   Checks whether the given identifier is reserved.
		/// </summary>
		/// <param name="identifier">The identifier that should be checked.</param>
		protected void ValidateIdentifier(Identifier identifier)
		{
			if (identifier.Name.StartsWith(Configuration.ReservedIdentifierPrefix))
				Error(identifier, "Identifiers starting with '{0}' are reserved.", Configuration.ReservedIdentifierPrefix);
		}

		/// <summary>
		///   Checks whether the given type is valid.
		/// </summary>
		/// <param name="node">The node whose type should be checked.</param>
		/// <param name="resolvedType">The declared type of the node.</param>
		protected void ValidateType(AstNode node, IType resolvedType)
		{
			if (resolvedType.ToDataType() == DataType.Unknown)
				Error(node, "Type '{0}' is not a supported data type.", resolvedType.FullName);
		}

		/// <summary>
		///   Checks whether the declared modifiers match the expected ones.
		/// </summary>
		/// <param name="declaration">The declaration that is affected by the modifiers.</param>
		/// <param name="declaredModifiers">The modifiers that have actually been declared.</param>
		/// <param name="expectedModifiers">The modifiers that should have been declared.</param>
		protected void ValidateModifiers(AstNode declaration, IEnumerable<CSharpModifierToken> declaredModifiers,
										 IEnumerable<Modifiers> expectedModifiers)
		{
			Assert.ArgumentNotNull(declaration);
			Assert.ArgumentNotNull(declaredModifiers);
			Assert.ArgumentNotNull(expectedModifiers);

			ValidateModifiers(declaration, declaredModifiers, new[] { expectedModifiers });
		}

		/// <summary>
		///   Checks whether the declared modifiers match the expected ones.
		/// </summary>
		/// <param name="declaration">The declaration that is affected by the modifiers.</param>
		/// <param name="declaredModifiers">The modifiers that have actually been declared.</param>
		/// <param name="expectedModifiers">The set of modifiers, one of which should have been declared.</param>
		protected void ValidateModifiers(AstNode declaration, IEnumerable<CSharpModifierToken> declaredModifiers,
										 IEnumerable<IEnumerable<Modifiers>> expectedModifiers)
		{
			Assert.ArgumentNotNull(declaration);
			Assert.ArgumentNotNull(declaredModifiers);
			Assert.ArgumentNotNull(expectedModifiers);

			// Check whether any modifiers other than the expected ones are declared 
			foreach (var modifier in declaredModifiers.Where(modifier => !expectedModifiers.Any(m => m.Contains(modifier.Modifier))))
				Error(modifier, "Unexpected modifier '{0}'.", modifier.Modifier.ToString().ToLower());

			// Check that the expected modifiers are present
			if (!expectedModifiers.Any(modifiers => modifiers.All(m => declaredModifiers.Any(declared => declared.Modifier == m))))
			{
				Func<IEnumerable<Modifiers>, string> listModifiers =
					modifiers => String.Join(", ", modifiers.Select(modifier => String.Format("'{0}'", modifier.ToString().ToLower())));

				Error(declaration, "Expected modifiers {0}.", String.Join(" or ", expectedModifiers.Select(listModifiers)));
			}
		}
	}
}