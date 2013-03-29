using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using Framework.Platform;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.Semantics;

	/// <summary>
	///   Represents an element that takes part in the effect compilation.
	/// </summary>
	internal abstract class CompiledElement
	{
		/// <summary>
		///   The list of child elements that are compiled when the current element is compiled.
		/// </summary>
		private readonly List<CompiledElement> _children = new List<CompiledElement>();

		/// <summary>
		///   The path to the C# effect file that contains the compiled element.
		/// </summary>
		private readonly string _file;

		/// <summary>
		///   Indicates whether any errors occurred during the compilation of the element.
		/// </summary>
		private bool _hasErrors;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="file"> The path to the C# effect file that contains the compiled element.</param>
		/// <param name="resolver">
		///   The C# AST resolver that should be used to resolve symbols of the effect file currently being
		///   compiled.
		/// </param>
		protected CompiledElement(string file, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNullOrWhitespace(file, () => file);
			Assert.ArgumentNotNull(resolver, () => resolver);

			_file = file;
			Resolver = resolver;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected CompiledElement()
		{
		}

		/// <summary>
		///   Gets a value indicating whether there have been any errors during the compilation of the element or any of its
		///   children.
		/// </summary>
		public bool HasErrors
		{
			get { return _hasErrors || _children.Any(child => child.HasErrors); }
		}

		/// <summary>
		///   Gets the C# AST resolver that should be used to resolve symbols of the effect file currently being compiled.
		/// </summary>
		protected CSharpAstResolver Resolver { get; private set; }

		/// <summary>
		///   Resolves the semantics of the given node.
		/// </summary>
		/// <typeparam name="T">The type of the returned result.</typeparam>
		/// <param name="node">The node that should be resolved.</param>
		protected T Resolve<T>(AstNode node)
			where T : ResolveResult
		{
			return Resolver.Resolve(node) as T;
		}

		/// <summary>
		///   Resolves the semantics of the given node.
		/// </summary>
		/// <param name="node">The node that should be resolved.</param>
		protected ResolveResult Resolve(AstNode node)
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
		protected void Error(AstNode node, string message, params object[] arguments)
		{
			Assert.ArgumentNotNull(node, () => node);
			Assert.ArgumentNotNullOrWhitespace(message, () => message);

			var formattedMessage = String.Format(message, arguments);
			EffectsProject.OutputMessage(LogType.Error, _file, formattedMessage, node.StartLocation, node.EndLocation);

			_hasErrors = true;
		}

		/// <summary>
		///   Logs a compilation warning.
		/// </summary>
		/// <param name="message">The message of the warning.</param>
		/// <param name="node">The node for which the warning should be raised.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		protected void Warn(AstNode node, string message, params object[] arguments)
		{
			Assert.ArgumentNotNull(node, () => node);
			Assert.ArgumentNotNullOrWhitespace(message, () => message);

			var formattedMessage = String.Format(message, arguments);
			EffectsProject.OutputMessage(LogType.Warning, _file, formattedMessage, node.StartLocation, node.EndLocation);
		}
	}
}