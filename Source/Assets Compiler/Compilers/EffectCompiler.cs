using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using Assets;
	using Framework;
	using Framework.Platform;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Refactoring;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.CSharp.TypeSystem;
	using ICSharpCode.NRefactory.Semantics;
	using ICSharpCode.NRefactory.TypeSystem;
	using ShaderCompilation;

	/// <summary>
	///   Cross-compiles shaders written in C# to HLSL and GLSL.
	/// </summary>
	internal class EffectCompiler : IAssetCompiler
	{
		/// <summary>
		///   Compiles all assets of the compiler's asset source type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		public bool Compile(IEnumerable<Asset> assets)
		{
			var cecilLoader = new CecilLoader();
			var files = Parse(assets).Select(s => new { SyntaxTree = s, File = s.ToTypeSystem() }).ToArray();
			IProjectContent project = new CSharpProjectContent();

			var assemblies = new[] { typeof(int).Assembly, typeof(EffectCompiler).Assembly };
			foreach (var assembly in assemblies)
			{
				var loadedAssembly = cecilLoader.LoadAssemblyFile(assembly.Location);
				project = project.AddAssemblyReferences(loadedAssembly);
			}

			foreach (var file in files)
				project = project.AddOrUpdateFiles(file.File);

			var compilation = project.CreateCompilation();
			foreach (var file in files)
				Compile(compilation, file.File, file.SyntaxTree);

			return true;
		}

		/// <summary>
		///   Parses all C# files.
		/// </summary>
		/// <param name="assets">The assets that should be parsed.</param>
		private static IEnumerable<SyntaxTree> Parse(IEnumerable<Asset> assets)
		{
			var parser = new CSharpParser();
			foreach (var file in assets.OfType<CSharpAsset>())
			{
				var syntaxTree = parser.Parse(File.ReadAllText(file.SourcePath), file.SourcePath);

				foreach (var error in parser.ErrorsAndWarnings)
				{
					switch (error.ErrorType)
					{
						default:
							Error(error.Message, file.RelativePath, error.Region.Begin, error.Region.End);
							break;
						case ErrorType.Warning:
							Warn(error.Message, file.RelativePath, error.Region.Begin, error.Region.End);
							break;
					}
				}

				yield return syntaxTree;
			}
		}

		private static void Compile(ICompilation compilation, CSharpUnresolvedFile file, SyntaxTree syntaxTree)
		{
			var resolver = new CSharpAstResolver(compilation, syntaxTree, file);
			var effects = syntaxTree.DescendantsAndSelf.OfType<TypeDeclaration>()
									.Where(t => t.ClassType == ClassType.Class);// && t.BaseTypes.Any(b => b.ToTypeReference().Resolve(resolver.TypeResolveContext).));

			if (!effects.Any())
				return;

			var resolved = resolver.Resolve(effects.First()) as TypeResolveResult;
			var baseType = resolved.Type.DirectBaseTypes.First();
			var s =baseType.FullName == typeof(Effect).FullName;

			return;
		}

		/// <summary>
		///   Logs a compilation error.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="file">The file that produced the error during compilation.</param>
		/// <param name="begin">The beginning of the error location in the source file.</param>
		/// <param name="end">The end of the error location in the source file.</param>
		public static void Error(string message, string file, TextLocation begin, TextLocation end)
		{
			Output(LogType.Error, message, file, begin, end);
		}

		/// <summary>
		///   Logs a compilation warning.
		/// </summary>
		/// <param name="message">The message of the warning.</param>
		/// <param name="file">The file that produced the warning during compilation.</param>
		/// <param name="begin">The beginning of the warning location in the source file.</param>
		/// <param name="end">The end of the warning location in the source file.</param>
		public static void Warn(string message, string file, TextLocation begin, TextLocation end)
		{
			Output(LogType.Warning, message, file, begin, end);
		}

		/// <summary>
		///   Outputs a compilation message.
		/// </summary>
		/// <param name="type">The type of the compilation message.</param>
		/// <param name="message">The message that should be output.</param>
		/// <param name="file">The file that produced the message during compilation.</param>
		/// <param name="begin">The beginning of the message location in the source file.</param>
		/// <param name="end">The end of the message location in the source file.</param>
		private static void Output(LogType type, string message, string file, TextLocation begin, TextLocation end)
		{
			Assert.ArgumentNotNullOrWhitespace(message, () => message);
			Assert.ArgumentNotNullOrWhitespace(file, () => file);

			string typeString;
			Action<string> log;

			switch (type)
			{
				case LogType.Error:
					typeString = "error";
					log = s => Log.Error(s);
					break;
				case LogType.Warning:
					typeString = "warning";
					log = s => Log.Warn(s);
					break;
				default:
					throw new InvalidOperationException("Unsupported log type.");
			}

			string location;
			if (end.IsEmpty)
				location = String.Format("({0},{1})", begin.Line, begin.Column);
			else
				location = String.Format("({0},{1},{2},{3})", begin.Line, begin.Column, end.Line, end.Column);

			message = message.Replace("{", "{{").Replace("}", "}}");
			var logMessage = String.Format("{0}{1}: {2}: {3}", file, location, typeString, message);

			log(logMessage);
		}
	}
}