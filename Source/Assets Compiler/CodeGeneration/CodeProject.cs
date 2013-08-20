using System;

namespace Pegasus.AssetsCompiler.CodeGeneration
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Assets;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.TypeSystem;
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///   Represents a NRefactory C# project.
	/// </summary>
	/// <typeparam name="TAsset">The type of the assets that are compiled by the project.</typeparam>
	/// <typeparam name="TFileElement">The type of the file elements that are compiled by the project.</typeparam>
	internal abstract class CodeProject<TAsset, TFileElement> : DisposableObject, IErrorReporter
		where TAsset : Asset
		where TFileElement : CodeElement
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
			CSharpFiles = Enumerable.Empty<TAsset>();
		}

		/// <summary>
		///   Gets or sets the assemblies that are loaded into the project. By default, only mscorlib and the assets compiler
		///   assembly are loaded.
		/// </summary>
		public IEnumerable<Assembly> Assemblies { get; set; }

		/// <summary>
		///   Gets or sets the path to the C# files that are loaded into the project.
		/// </summary>
		public IEnumerable<TAsset> CSharpFiles { get; set; }

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

			var defaultAssemblies = new[] { typeof(int).Assembly, typeof(CodeProject<,>).Assembly };
			foreach (var assembly in defaultAssemblies.Union(Assemblies))
			{
				var loadedAssembly = loader.LoadAssemblyFile(assembly.Location);
				_project = _project.AddAssemblyReferences(loadedAssembly);
			}
		}

		/// <summary>
		///   Loads the C# files into the project.
		/// </summary>
		private IEnumerable<TFileElement> LoadFiles()
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
				return CreateFile(file.FileName, file.SyntaxTree, resolver);
			});
		}

		/// <summary>
		///   Creates a code element representing the a file.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <param name="syntaxTree">The syntax tree of the file.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information within the file.</param>
		protected abstract TFileElement CreateFile(string fileName, SyntaxTree syntaxTree, CSharpAstResolver resolver);

		/// <summary>
		///   Compiles the project. Returns false to indicate that compilation errors have occurred.
		/// </summary>
		public bool Compile()
		{
			TFileElement[] files;
			TryGetValidatedFiles(out files);

			foreach (var file in files.Where(file => !file.HasErrors))
				Compile(file);

			return files.All(file => !file.HasErrors);
		}

		/// <summary>
		///   Gets all validated file elements.
		/// </summary>
		/// <param name="files">Returns the validated file elements.</param>
		public void TryGetValidatedFiles(out TFileElement[] files)
		{
			LoadAssemblies();
			files = LoadFiles().ToArray();

			foreach (var file in files)
			{
				file.InitializeElement();
				file.ValidateElement();
			}
		}

		/// <summary>
		///   Compiles the given file.
		/// </summary>
		/// <param name="file">The file that should be compiled.</param>
		protected abstract void Compile(TFileElement file);

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