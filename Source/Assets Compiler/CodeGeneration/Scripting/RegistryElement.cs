using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Scripting
{
	using System.Linq;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;

	/// <summary>
	///   Represents an element of an effect declaration.
	/// </summary>
	internal abstract class RegistryElement : CodeElement
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="errorReporter">The error reporter that should be used to report validation errors.</param>
		/// <param name="sourceFile">The path to the C# file that contains the element.</param>
		/// <param name="resolver">The resolver that should be used to resolve symbols of the C# file currently being analyzed.</param>
		protected RegistryElement(IErrorReporter errorReporter, string sourceFile, CSharpAstResolver resolver)
			: base(errorReporter, sourceFile, resolver)
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected RegistryElement()
		{
		}

		/// <summary>
		///   Gets the attribute with the given name, where the name can optionally end with 'Attribute'.
		/// </summary>
		/// <param name="attributeSections">The attributes that should be searched.</param>
		/// <param name="name">The name of the attribute that should be returned.</param>
		protected Attribute GetAttribute(AstNodeCollection<AttributeSection> attributeSections, string name)
		{
			Assert.ArgumentNotNull(attributeSections, () => attributeSections);
			Assert.ArgumentNotNullOrWhitespace(name, () => name);

			var attributes = from attribute in attributeSections.SelectMany(section => section.Attributes)
							 where attribute.Type.ToString() == name || attribute.Type.ToString() == name + "Attribute"
							 select attribute;

			return attributes.FirstOrDefault();
		}
	}
}