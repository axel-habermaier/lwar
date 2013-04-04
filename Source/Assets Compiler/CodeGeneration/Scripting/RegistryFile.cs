using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Scripting
{
	using System.Linq;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;

	/// <summary>
	///   Represents a C# source code file that declares a registry interface.
	/// </summary>
	internal class RegistryFile : RegistryElement
	{
		/// <summary>
		///   The parsed syntax tree of the file.
		/// </summary>
		private readonly AstNode _syntaxTree;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="errorReporter">The error reporter that should be used to report validation errors.</param>
		/// <param name="syntaxTree">The parsed syntax tree of the effect file.</param>
		/// <param name="resolver"> The C# AST resolver that should be used to resolve symbols of the effect file.</param>
		public RegistryFile(IErrorReporter errorReporter, SyntaxTree syntaxTree, CSharpAstResolver resolver)
			: base(errorReporter, syntaxTree.FileName, resolver)
		{
			_syntaxTree = syntaxTree;
		}

		/// <summary>
		///   Gets the registry declared in the file.
		/// </summary>
		public Registry Registry
		{
			get { return GetChildElements<Registry>().Single(); }
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all interface declarations
			AddElements(from declaration in _syntaxTree.Descendants.OfType<TypeDeclaration>()
						where declaration.ClassType == ClassType.Interface
						select new Registry(declaration));
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check that only one interface is declared in the file
			foreach (var declaration in _syntaxTree.Descendants.OfType<TypeDeclaration>()
												   .Where(type => type.ClassType == ClassType.Interface)
												   .Skip(1))
			{
				Error(declaration, "Unexpected interface declaration.");
			}

			// Check that only interfaces are declared in the file
			foreach (var declaration in from type in _syntaxTree.Descendants.OfType<TypeDeclaration>()
										where type.ClassType != ClassType.Interface
										select type)
			{
				Error(declaration, "Unexpected {0} declaration.", declaration.ClassType.ToString().ToLower());
			}
		}
	}
}