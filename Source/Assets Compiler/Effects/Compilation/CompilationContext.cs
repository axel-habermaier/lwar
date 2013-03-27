using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using Framework;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.Semantics;

	/// <summary>
	///   Represents the context of an effect compilation.
	/// </summary>
	internal class CompilationContext
	{
		/// <summary>
		///   Gets a value indicating whether there have been any errors during compilation.
		/// </summary>
		public bool HasErrors { get; private set; }

		/// <summary>
		///   Gets or sets the effect class that is currently being compiled.
		/// </summary>
		public EffectClass Effect { get; set; }

		/// <summary>
		///   Gets or sets the C# effect file that is currently being compiled.
		/// </summary>
		public EffectFile File { get; set; }

		/// <summary>
		///   Gets or sets the C# AST resolver that should be used to resolve symbols of the effect file currently being compiled.
		/// </summary>
		public CSharpAstResolver Resolver { get; set; }

		/// <summary>
		///   Resolves the semantics of the given node.
		/// </summary>
		/// <typeparam name="T">The type of the returned result.</typeparam>
		/// <param name="node">The node that should be resolved.</param>
		public T Resolve<T>(AstNode node)
			where T : ResolveResult
		{
			return Resolver.Resolve(node) as T;
		}

		/// <summary>
		///   Resolves the semantics of the given node.
		/// </summary>
		/// <typeparam name="T">The type of the returned result.</typeparam>
		/// <param name="node">The node that should be resolved.</param>
		public ResolveResult Resolve(AstNode node)
		{
			return Resolver.Resolve(node);
		}

		/// <summary>
		///   Logs a compilation error.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="node">The node for which the error should be reported.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public void Error(AstNode node, string message, params object[] arguments)
		{
			Error(node.StartLocation, node.EndLocation, message, arguments);
		}

		/// <summary>
		///   Logs a compilation warning.
		/// </summary>
		/// <param name="message">The message of the warning.</param>
		/// <param name="node">The node for which the warning should be raised.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public void Warn(AstNode node, string message, params object[] arguments)
		{
			Warn(node.StartLocation, node.EndLocation, message, arguments);
		}

		/// <summary>
		///   Logs a compilation error.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="begin">The beginning of the error location in the source file.</param>
		/// <param name="end">The end of the error location in the source file.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public void Error(TextLocation begin, TextLocation end, string message, params object[] arguments)
		{
			HasErrors = true;
			Output(s => Log.Error(s), "error", String.Format(message, arguments), begin, end);
		}

		/// <summary>
		///   Logs a compilation warning.
		/// </summary>
		/// <param name="message">The message of the warning.</param>
		/// <param name="begin">The beginning of the warning location in the source file.</param>
		/// <param name="end">The end of the warning location in the source file.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public void Warn(TextLocation begin, TextLocation end, string message, params object[] arguments)
		{
			Output(s => Log.Warn(s), "warning", String.Format(message, arguments), begin, end);
		}

		/// <summary>
		///   Outputs a compilation message.
		/// </summary>
		/// <param name="type">The type of the compilation message.</param>
		/// <param name="message">The message that should be output.</param>
		/// <param name="begin">The beginning of the message location in the source file.</param>
		/// <param name="end">The end of the message location in the source file.</param>
		private void Output(Action<string> log, string type, string message, TextLocation begin, TextLocation end)
		{
			Assert.ArgumentNotNull(log, () => log);
			Assert.ArgumentNotNullOrWhitespace(type, () => type);
			Assert.ArgumentNotNullOrWhitespace(message, () => message);

			string location;
			if (end.IsEmpty)
				location = String.Format("({0},{1})", begin.Line, begin.Column);
			else
				location = String.Format("({0},{1},{2},{3})", begin.Line, begin.Column, end.Line, end.Column);

			var file = File.Asset.RelativePath.Replace("/", "\\");
			message = message.Replace("{", "{{").Replace("}", "}}");
			var logMessage = String.Format("{0}{1}: {2}: {3}", file, location, type, message);

			log(logMessage);
		}
	}
}