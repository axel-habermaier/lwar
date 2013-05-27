using System;

namespace Pegasus.AssetsCompiler.CodeGeneration
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Logging;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;

	/// <summary>
	///   Represents a code element.
	/// </summary>
	internal abstract class CodeElement
	{
		/// <summary>
		///   The list of child elements that this element consists of.
		/// </summary>
		private readonly List<CodeElement> _childElements = new List<CodeElement>();

		/// <summary>
		///   The error reporter that is used to report validation errors.
		/// </summary>
		private IErrorReporter _errorReporter;

		/// <summary>
		///   Indicates whether any errors occurred during the validation of the element.
		/// </summary>
		private bool _hasErrors;

		/// <summary>
		///   The path to the C# file that contains the element.
		/// </summary>
		private string _sourceFile;

		/// <summary>
		///   The current state of the element.
		/// </summary>
		private State _state = State.Uninitialized;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="errorReporter">The error reporter that should be used to report validation errors.</param>
		/// <param name="sourceFile">The path to the C# file that contains the element.</param>
		/// <param name="resolver">The resolver that should be used to resolve symbols of the C# file currently being analyzed.</param>
		protected CodeElement(IErrorReporter errorReporter, string sourceFile, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(errorReporter);
			Assert.ArgumentNotNullOrWhitespace(sourceFile);
			Assert.ArgumentNotNull(resolver);

			_errorReporter = errorReporter;
			_sourceFile = sourceFile;
			Resolver = resolver;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected CodeElement()
		{
		}

		/// <summary>
		///   Gets the C# AST resolver that should be used to resolve symbols of the C# file currently being analyzed.
		/// </summary>
		protected CSharpAstResolver Resolver { get; private set; }

		/// <summary>
		///   Gets a value indicating whether there have been any errors during the validation of the element or any of its
		///   children.
		/// </summary>
		public bool HasErrors
		{
			get { return _hasErrors || _childElements.Any(child => child.HasErrors); }
		}

		/// <summary>
		///   Initializes the element and all of its children.
		/// </summary>
		public void InitializeElement()
		{
			Assert.That(_state == State.Uninitialized, "This method must be called while the element is still uinitialized.");

			Initialize();
			foreach (var child in _childElements)
				child.InitializeElement();

			_state = State.Initialized;
		}

		/// <summary>
		///   Validates the element and all of its children. This method has no effect if any errors occurred during
		///   initialization.
		/// </summary>
		public void ValidateElement()
		{
			Assert.That(_state == State.Initialized, "This method must be called when the element is initialized.");

			if (HasErrors)
				return;

			foreach (var child in _childElements)
				child.ValidateElement();

			Validate();
			_state = State.Validated;
		}

		/// <summary>
		///   Adds the given elements to the current element.
		/// </summary>
		/// <param name="elements">The elements that should be added.</param>
		protected void AddElements(IEnumerable<CodeElement> elements)
		{
			Assert.ArgumentNotNull(elements);

			foreach (var element in elements)
				AddElement(element);
		}

		/// <summary>
		///   Adds the given element to the current element.
		/// </summary>
		/// <param name="element">The element that should be added.</param>
		protected void AddElement(CodeElement element)
		{
			Assert.ArgumentNotNull(element);
			Assert.That(_state == State.Uninitialized, "No child elements can be added once the element has already been initialized.");

			element._sourceFile = _sourceFile;
			element.Resolver = Resolver;
			element._errorReporter = _errorReporter;

			_childElements.Add(element);
		}

		/// <summary>
		///   Returns all child elements of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the elements that should be returned.</typeparam>
		protected IEnumerable<T> GetChildElements<T>()
			where T : CodeElement
		{
			return _childElements.OfType<T>();
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected virtual void Initialize()
		{
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected virtual void Validate()
		{
		}

		/// <summary>
		///   Logs an error.
		/// </summary>
		/// <param name="node">The node for which the error should be reported.</param>
		/// <param name="message">The error message.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		protected void Error(AstNode node, string message, params object[] arguments)
		{
			Assert.ArgumentNotNull(node);
			Error(node.StartLocation, node.EndLocation, message, arguments);
		}

		/// <summary>
		///   Logs a warning.
		/// </summary>
		/// <param name="node">The node for which the warning should be raised.</param>
		/// <param name="message">The message of the warning.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		protected void Warn(AstNode node, string message, params object[] arguments)
		{
			Assert.ArgumentNotNull(node);
			Warn(node.StartLocation, node.EndLocation, message, arguments);
		}

		/// <summary>
		///   Logs an error.
		/// </summary>
		/// <param name="begin">The beginning of the message location in the source file.</param>
		/// <param name="end">The end of the message location in the source file.</param>
		/// <param name="message">The error message.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		protected void Error(TextLocation begin, TextLocation end, string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message);

			_errorReporter.Report(LogType.Error, _sourceFile, String.Format(message, arguments), begin, end);
			_hasErrors = true;
		}

		/// <summary>
		///   Logs a warning.
		/// </summary>
		/// <param name="begin">The beginning of the message location in the source file.</param>
		/// <param name="end">The end of the message location in the source file.</param>
		/// <param name="message">The message of the warning.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		protected void Warn(TextLocation begin, TextLocation end, string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message);
			_errorReporter.Report(LogType.Warning, _sourceFile, String.Format(message, arguments), begin, end);
		}

		/// <summary>
		///   Describes the state of a compiled element.
		/// </summary>
		private enum State
		{
			Uninitialized,
			Initialized,
			Validated
		}
	}
}