namespace Pegasus.AssetsCompiler.Registries
{
	using System;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;

	/// <summary>
	///     Represents a C# project with registry declaration interfaces for which a C# registry class is generated.
	/// </summary>
	internal class RegistryProject : CSharpProject<RegistryFile>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public RegistryProject()
		{
			GeneratedCode = String.Empty;
		}

		/// <summary>
		///     Gets or sets the namespace in which the generated class should live.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		///     Gets the generated code.
		/// </summary>
		public string GeneratedCode { get; private set; }

		/// <summary>
		///     Gets or sets the imported registry.
		/// </summary>
		public Registry ImportedRegistry { get; set; }

		/// <summary>
		///     Creates a code element representing the a file.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <param name="syntaxTree">The syntax tree of the file.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information within the file.</param>
		protected override RegistryFile CreateFile(string fileName, SyntaxTree syntaxTree, CSharpAstResolver resolver)
		{
			return new RegistryFile(this, syntaxTree, resolver);
		}

		/// <summary>
		///     Compiles the given file.
		/// </summary>
		/// <param name="file">The file that should be compiled.</param>
		protected override void Compile(RegistryFile file)
		{
			var writer = new CodeWriter();

			var generator = new CSharpCodeGenerator(writer, file.Registry, ImportedRegistry);
			generator.GenerateCode(Namespace);

			GeneratedCode = writer.ToString();
		}
	}
}