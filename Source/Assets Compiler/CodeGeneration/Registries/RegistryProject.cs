using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Registries
{
	using System.Collections.Generic;
	using Framework;
	using Framework.Platform.Logging;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;

	/// <summary>
	///   Represents a C# project with registry declaration interfaces for which a C# registry class is generated.
	/// </summary>
	internal class RegistryProject : CodeProject<RegistryFile>
	{
		/// <summary>
		///   The errors that have been raised during the compilation.
		/// </summary>
		private readonly List<LogEntry> _errors = new List<LogEntry>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public RegistryProject()
		{
			GeneratedCode = String.Empty;
		}

		/// <summary>
		///   Gets or sets the namespace in which the generated class should live.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		///   Gets the errors that have been raised during the compilation of the project.
		/// </summary>
		public IEnumerable<LogEntry> Errors
		{
			get { return _errors; }
		}

		/// <summary>
		///   Gets the generated code.
		/// </summary>
		public string GeneratedCode { get; private set; }

		/// <summary>
		///   Gets or sets the imported registry.
		/// </summary>
		public Registry ImportedRegistry { get; set; }

		/// <summary>
		///   Outputs a compilation message.
		/// </summary>
		/// <param name="type">The type of the compilation message.</param>
		/// <param name="file">The name of the file for which the message should be raised.</param>
		/// <param name="message">The message that should be output.</param>
		/// <param name="begin">The beginning of the message location in the source file.</param>
		/// <param name="end">The end of the message location in the source file.</param>
		public override void Report(LogType type, string file, string message, TextLocation begin, TextLocation end)
		{
			Assert.ArgumentSatisfies(type == LogType.Error, "Unsupported log type.");

			file = file.ToLocationString(begin, end);
			file = file.Replace("/", "\\");

			message = String.Format("{0}: error: {1}", file, message);
			_errors.Add(new LogEntry(LogCategory.Assets, LogType.Error, message));
		}

		/// <summary>
		///   Creates a code element representing the a file.
		/// </summary>
		/// <param name="syntaxTree">The syntax tree of the file.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information within the file.</param>
		protected override RegistryFile CreateFile(SyntaxTree syntaxTree, CSharpAstResolver resolver)
		{
			return new RegistryFile(this, syntaxTree, resolver);
		}

		/// <summary>
		///   Compiles the given file.
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