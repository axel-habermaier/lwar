using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Effects
{
	using Assets;
	using Framework.Platform.Logging;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;

	/// <summary>
	///   Represents a C# project with effect declarations that have to be cross-compiled.
	/// </summary>
	internal class EffectsProject : CodeProject<EffectAsset, EffectFile>
	{
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
			file = file.ToLocationString(begin, end);

			var logMessage = String.Format("{0}: {1}: {2}", file, type, message);
			new LogEntry(type, logMessage).RaiseLogEvent();
		}

		/// <summary>
		///   Creates a code element representing the a file.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <param name="syntaxTree">The syntax tree of the file.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information within the file.</param>
		protected override EffectFile CreateFile(string fileName, SyntaxTree syntaxTree, CSharpAstResolver resolver)
		{
			return new EffectFile(fileName, this, syntaxTree, resolver);
		}

		/// <summary>
		///   Compiles the given file.
		/// </summary>
		/// <param name="file">The file that should be compiled.</param>
		protected override void Compile(EffectFile file)
		{
			file.Compile();
		}
	}
}