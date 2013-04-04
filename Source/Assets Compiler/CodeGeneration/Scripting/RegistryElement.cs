using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Scripting
{
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
	}
}