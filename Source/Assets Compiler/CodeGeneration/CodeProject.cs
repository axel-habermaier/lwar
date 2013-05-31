using System;

namespace Pegasus.AssetsCompiler.CodeGeneration
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Assets;
	using Framework;
	using Framework.Platform.Logging;
	using Framework.Platform.Memory;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a NRefactory C# project.
	/// </summary>
	/// <typeparam name="T">The type of the file elements that are compiled by the project.</typeparam>
	internal abstract class CodeProject<T> : DisposableObject, IErrorReporter
		where T : CodeElement
	{
		/// <summary>
		///   The C# project that is compiled.
		/// </summary>
		private IProjectContent _project = new CSharpProjectContent();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected CodeProject()
		{
			Assemblies = Enumerable.Empty<Assembly>();
			CSharpFiles = Enumerable.Empty<CSharpAsset>();
		}

		/// <summary>
		///   Gets or sets the assemblies that are loaded into the project. By default, only mscorlib and the assets compiler
		///   assembly are loaded.
		/// </summary>
		public IEnumerable<Assembly> Assemblies { get; set; }

		/// <summary>
		///   Gets or sets the path to the C# files that are loaded into the project.
		/// </summary>
		public IEnumerable<CSharpAsset> CSharpFiles { get; set; }

		/// <summary>
		///   Outputs a compilation message.
		/// </summary>
		/// <param name="type">The type of the compilation message.</param>
		/// <param name="file">The name of the file for which the message should be raised.</param>
		/// <param name="message">The message that should be output.</param>
		/// <param name="begin">The beginning of the message location in the source file.</param>
		/// <param name="end">The end of the message location in the source file.</param>
		public abstract void Report(LogType type, string file, string message, TextLocation begin, TextLocation end);

		/// <summary>
		///   Loads the required assemblies into the project.
		/// </summary>
		private void LoadAssemblies()
		{
			var loader = new CecilLoader();

			var defaultAssemblies = new[] { typeof(int).Assembly, typeof(CodeProject<>).Assembly };
			foreach (var assembly in defaultAssemblies.Union(Assemblies))
			{
				var loadedAssembly = loader.LoadAssemblyFile(assembly.Location);
				_project = _project.AddAssemblyReferences(loadedAssembly);
			}
		}

		/// <summary>
		///   Loads the C# files into the project.
		/// </summary>
		private IEnumerable<T> LoadFiles()
		{
			var parser = new CSharpParser();
			var parsedFiles = CSharpFiles.Select(asset =>
				{
					var syntaxTree = parser.Parse(File.ReadAllText(asset.SourcePath), asset.SourcePath);
					syntaxTree.FileName = asset.RelativePath;
					PrintParserErrors(asset.RelativePath, parser.Errors.ToArray());

					var unresolvedFile = syntaxTree.ToTypeSystem();
					_project = _project.AddOrUpdateFiles(unresolvedFile);

					return new { FileName = asset.RelativePath, SyntaxTree = syntaxTree, UnresolvedFile = unresolvedFile };
				}).ToArray();

			var compilation = _project.CreateCompilation();
			return parsedFiles.Select(file =>
				{
					var resolver = new CSharpAstResolver(compilation, file.SyntaxTree, file.UnresolvedFile);
					return CreateFile(file.SyntaxTree, resolver);
				});
		}

		/// <summary>
		///   Creates a code element representing the a file.
		/// </summary>
		/// <param name="syntaxTree">The syntax tree of the file.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information within the file.</param>
		protected abstract T CreateFile(SyntaxTree syntaxTree, CSharpAstResolver resolver);

		/// <summary>
		///   Compiles the project. Returns false to indicate that compilation errors have occurred.
		/// </summary>
		public bool Compile()
		{
			LoadAssemblies();
			var effectFiles = LoadFiles().ToArray();

			foreach (var file in effectFiles)
			{
				file.InitializeElement();
				file.ValidateElement();

				if (!file.HasErrors)
					Compile(file);
			}

			return effectFiles.All(file => !file.HasErrors);
		}

		/// <summary>
		///   Compiles the given file.
		/// </summary>
		/// <param name="file">The file that should be compiled.</param>
		protected abstract void Compile(T file);

		/// <summary>
		///   Prints all parser errors.
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

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
		}
	}
}