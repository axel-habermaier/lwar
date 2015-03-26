namespace Pegasus.AssetsCompiler.CSharp
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.TypeSystem;
	using Utilities;

	/// <summary>
	///     Represents a NRefactory C# project.
	/// </summary>
	/// <typeparam name="TFileElement">The type of the file elements that are compiled by the project.</typeparam>
	internal abstract class CSharpProject<TFileElement> : IErrorReporter
		where TFileElement : CodeElement
	{
		/// <summary>
		///     The C# project that is compiled.
		/// </summary>
		private IProjectContent _project = new CSharpProjectContent();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		protected CSharpProject()
		{
			Assemblies = Enumerable.Empty<Assembly>();
			CSharpFiles = Enumerable.Empty<CSharpFile>();
		}

		/// <summary>
		///     Gets or sets the assemblies that are loaded into the project. By default, only mscorlib and the assets compiler
		///     assembly are loaded.
		/// </summary>
		public IEnumerable<Assembly> Assemblies { get; set; }

		/// <summary>
		///     Gets or sets the path to the C# files that are loaded into the project.
		/// </summary>
		public IEnumerable<CSharpFile> CSharpFiles { get; set; }

		/// <summary>
		///     Outputs a compilation message.
		/// </summary>
		/// <param name="type">The type of the compilation message.</param>
		/// <param name="file">The name of the file for which the message should be raised.</param>
		/// <param name="message">The message that should be output.</param>
		/// <param name="begin">The beginning of the message location in the source file.</param>
		/// <param name="end">The end of the message location in the source file.</param>
		public void Report(LogType type, string file, string message, TextLocation begin, TextLocation end)
		{
			file = file.ToLocationString(begin, end);

			var logMessage = String.Format("{0}: {1}: {2}", file, type, message);
			new LogEntry(type, logMessage).RaiseLogEvent();
		}

		/// <summary>
		///     Loads the required assemblies into the project.
		/// </summary>
		private void LoadAssemblies()
		{
			var loader = new CecilLoader();

			var defaultAssemblies = new[] { typeof(int).Assembly, typeof(CSharpProject<>).Assembly };
			foreach (var assembly in defaultAssemblies.Union(Assemblies))
			{
				var loadedAssembly = loader.LoadAssemblyFile(assembly.Location);
				_project = _project.AddAssemblyReferences(loadedAssembly);
			}
		}

		/// <summary>
		///     Loads the C# files into the project.
		/// </summary>
		private IEnumerable<TFileElement> LoadFiles()
		{
			var parser = new CSharpParser();
			var parsedFiles = CSharpFiles.Select(file =>
			{
				var syntaxTree = parser.Parse(File.ReadAllText(file.SourcePath), file.SourcePath);
				syntaxTree.FileName = file.SourcePath;
				PrintParserErrors(file.SourcePath, parser.Errors.ToArray());

				var unresolvedFile = syntaxTree.ToTypeSystem();
				_project = _project.AddOrUpdateFiles(unresolvedFile);

				return new { FileName = file.SourcePath, SyntaxTree = syntaxTree, UnresolvedFile = unresolvedFile };
			}).ToArray();

			var compilation = _project.CreateCompilation();
			return parsedFiles.Select(file =>
			{
				var resolver = new CSharpAstResolver(compilation, file.SyntaxTree, file.UnresolvedFile);
				return CreateFile(file.FileName, file.SyntaxTree, resolver);
			});
		}

		/// <summary>
		///     Creates a code element representing the a file.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <param name="syntaxTree">The syntax tree of the file.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information within the file.</param>
		protected abstract TFileElement CreateFile(string fileName, SyntaxTree syntaxTree, CSharpAstResolver resolver);

		/// <summary>
		///     Compiles the project. Returns false to indicate that compilation errors have occurred.
		/// </summary>
		public bool Compile()
		{
			var files = GetValidatedFiles();

			foreach (var file in files.Where(file => !file.HasErrors))
				Compile(file);

			return files.All(file => !file.HasErrors);
		}

		/// <summary>
		///     Gets all validated file elements.
		/// </summary>
		public TFileElement[] GetValidatedFiles()
		{
			LoadAssemblies();
			var files = LoadFiles().ToArray();

			foreach (var file in files)
			{
				file.InitializeElement();
				file.ValidateElement();
			}

			return files;
		}

		/// <summary>
		///     Compiles the given file.
		/// </summary>
		/// <param name="file">The file that should be compiled.</param>
		protected abstract void Compile(TFileElement file);

		/// <summary>
		///     Prints all parser errors.
		/// </summary>
		/// <param name="file">The file for which the parser messages should be printed.</param>
		/// <param name="errors">The parser errors that should be printed.</param>
		private void PrintParserErrors(string file, Error[] errors)
		{
			Assert.ArgumentNotNull(errors);
			Assert.ArgumentNotNullOrWhitespace(file);

			foreach (var error in errors)
				Report(LogType.Error, file, error.Message, error.Region.Begin, error.Region.End);
		}
	}
}