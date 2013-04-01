using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using Framework;
	using Framework.Platform;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a C# project with effect declarations that have to be cross-compiled.
	/// </summary>
	internal class EffectsProject
	{
		/// <summary>
		///   The C# project that is compiled.
		/// </summary>
		private IProjectContent _project = new CSharpProjectContent();

		/// <summary>
		///   Gets the cross-compiled shader assets.
		/// </summary>
		public IEnumerable<Asset> ShaderAssets { get; private set; }

		/// <summary>
		///   Loads the required assemblies into the project.
		/// </summary>
		private void LoadAssemblies()
		{
			var loader = new CecilLoader();

			var assemblies = new[] { typeof(int).Assembly, typeof(EffectsProject).Assembly };
			foreach (var assembly in assemblies)
			{
				var loadedAssembly = loader.LoadAssemblyFile(assembly.Location);
				_project = _project.AddAssemblyReferences(loadedAssembly);
			}
		}

		/// <summary>
		///   Loads the C# files into the project.
		/// </summary>
		/// <param name="assets">The C# assets that should be compiled.</param>
		private IEnumerable<EffectFile> LoadFiles(CSharpAsset[] assets)
		{
			Assert.ArgumentNotNull(assets, () => assets);

			var parser = new CSharpParser();
			var parsedFiles = assets.Select(asset =>
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
					return new EffectFile(file.SyntaxTree, resolver);
				});
		}

		/// <summary>
		///   Compiles all effects. Returns false to indicate that compilation errors have occurred.
		/// </summary>
		/// <param name="assets">The C# assets that should be compiled.</param>
		public bool Compile(CSharpAsset[] assets)
		{
			Assert.ArgumentNotNull(assets, () => assets);

			var effectFiles = new EffectFile[0];
			try
			{
				LoadAssemblies();
				effectFiles = LoadFiles(assets).ToArray();

				using (var generator = new CSharpCodeGenerator())
				{
					foreach (var file in effectFiles)
					{
						file.InitializeElement();
						file.ValidateElement();

						if (!file.HasErrors)
							file.Compile(generator);
					}
				}

				
				ShaderAssets = effectFiles.SelectMany(file => file.ShaderAssets);
				return effectFiles.All(file => !file.HasErrors);
			}
			catch (Exception e)
			{
				Log.Error("Effect cross-compilation failed: {0}", e.Message);

				effectFiles.SelectMany(file => file.ShaderAssets).ToArray().SafeDisposeAll();
				ShaderAssets = Enumerable.Empty<Asset>();
				return false;
			}
		}

		/// <summary>
		///   Prints all parser errors.
		/// </summary>
		/// <param name="file">The file for which the parser messages should be printed.</param>
		/// <param name="errors">The parser errors that should be printed.</param>
		private static void PrintParserErrors(string file, Error[] errors)
		{
			Assert.ArgumentNotNull(errors, () => errors);
			Assert.ArgumentNotNullOrWhitespace(file, () => file);

			foreach (var error in errors)
				OutputMessage(LogType.Error, file, error.Message, error.Region.Begin, error.Region.End);
		}

		/// <summary>
		///   Outputs a compilation message.
		/// </summary>
		/// <param name="type">The type of the compilation message.</param>
		/// <param name="file">The name of the file for which the message should be raised.</param>
		/// <param name="message">The message that should be output.</param>
		/// <param name="begin">The beginning of the message location in the source file.</param>
		/// <param name="end">The end of the message location in the source file.</param>
		public static void OutputMessage(LogType type, string file, string message, TextLocation begin, TextLocation end)
		{
			Assert.ArgumentInRange(type, () => type);
			Assert.ArgumentNotNullOrWhitespace(file, () => file);
			Assert.ArgumentNotNullOrWhitespace(message, () => message);

			string location;
			if (end.IsEmpty)
				location = String.Format("({0},{1})", begin.Line, begin.Column);
			else
				location = String.Format("({0},{1},{2},{3})", begin.Line, begin.Column, end.Line, end.Column);

			message = message.Replace("{", "{{").Replace("}", "}}");
			var logMessage = String.Format("{0}{1}: {2}: {3}", file.Replace("/", "\\"), location, type, message);

			new LogEntry(type, logMessage).RaiseLogEvent();
		}
	}
}