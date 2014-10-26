namespace Pegasus.AssetsCompiler.Registries
{
	using System;
	using System.Linq;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using Utilities;
	using Attribute = ICSharpCode.NRefactory.CSharp.Attribute;

	/// <summary>
	///     Represents an element of an effect declaration.
	/// </summary>
	internal abstract class RegistryElement : CodeElement
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="errorReporter">The error reporter that should be used to report validation errors.</param>
		/// <param name="sourceFile">The path to the C# file that contains the element.</param>
		/// <param name="resolver">The resolver that should be used to resolve symbols of the C# file currently being analyzed.</param>
		protected RegistryElement(IErrorReporter errorReporter, string sourceFile, CSharpAstResolver resolver)
			: base(errorReporter, sourceFile, resolver)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		protected RegistryElement()
		{
		}

		/// <summary>
		///     Gets the attribute with the given type, where the type can optionally end with 'Attribute'.
		/// </summary>
		/// <param name="attributeSections">The attributes that should be searched.</param>
		/// <param name="type">The type of the attribute that should be returned.</param>
		protected Attribute GetAttribute(AstNodeCollection<AttributeSection> attributeSections, string type)
		{
			Assert.ArgumentNotNull(attributeSections);
			Assert.ArgumentNotNullOrWhitespace(type);

			var attributes = from attribute in attributeSections.SelectMany(section => section.Attributes)
							 where AttributeHasType(attribute, type)
							 select attribute;

			return attributes.FirstOrDefault();
		}

		/// <summary>
		///     Checks whether the attribute is of the given type, where the type can optionally end with 'Attribute'.
		/// </summary>
		/// <param name="attribute">The attribute that should be checked.</param>
		/// <param name="type">The type of the attribute that should be checked.</param>
		protected bool AttributeHasType(Attribute attribute, string type)
		{
			return attribute.Type.ToString() == type || attribute.Type.ToString() == type + "Attribute";
		}
	}
}