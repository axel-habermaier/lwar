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
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents an element that takes part in the effect compilation.
	/// </summary>
	internal abstract class CompiledElement
	{
		/// <summary>
		///   The list of child elements that are compiled when the current element is compiled.
		/// </summary>
		private readonly List<CompiledElement> _childElements = new List<CompiledElement>();

		/// <summary>
		///   The path to the C# effect file that contains the compiled element.
		/// </summary>
		private string _file;

		/// <summary>
		///   Indicates whether any errors occurred during the compilation of the element.
		/// </summary>
		private bool _hasErrors;

		/// <summary>
		///   The current state of the compiled element.
		/// </summary>
		private State _state = State.Uninitialized;

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
			get { return _hasErrors || _childElements.Any(child => child.HasErrors); }
		}

		/// <summary>
		///   Gets the C# AST resolver that should be used to resolve symbols of the effect file currently being compiled.
		/// </summary>
		protected CSharpAstResolver Resolver { get; private set; }

		/// <summary>
		///   Initializes the compiled element and all of its children.
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
		///   Validates the compiled element and all of its children. This method has no effect if any errors occurred during
		///   initialization.
		/// </summary>
		public void ValidateElement()
		{
			Assert.That(_state != State.Initialized, "This method must be called when the element is initialized.");

			if (HasErrors)
				return;

			Validate();
			foreach (var child in _childElements)
				child.ValidateElement();

			_state = State.Validated;
		}

		/// <summary>
		///   Compiles the element and all of its children. This method has no effect if any errors occurred during initialization
		///   or validation.
		/// </summary>
		public void CompileElement()
		{
			Assert.That(_state != State.Validated, "This method must be called when the element is validated.");

			if (HasErrors)
				return;

			Compile();
			foreach (var child in _childElements)
				child.CompileElement();

			_state = State.Compiled;
		}

		/// <summary>
		///   Adds the given elements to the current element.
		/// </summary>
		/// <param name="elements">The elements that should be added.</param>
		protected void AddElements(IEnumerable<CompiledElement> elements)
		{
			Assert.ArgumentNotNull(elements, () => elements);

			foreach (var element in elements)
				AddElement(element);
		}

		/// <summary>
		///   Adds the given element to the current element.
		/// </summary>
		/// <param name="element">The element that should be added.</param>
		protected void AddElement(CompiledElement element)
		{
			Assert.ArgumentNotNull(element, () => element);
			Assert.That(_state == State.Uninitialized, "No child elements can be added once the element has already been initialized.");

			element._file = _file;
			element.Resolver = Resolver;
			_childElements.Add(element);
		}

		/// <summary>
		///   Returns all child elements of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the elements that should be returned.</typeparam>
		protected IEnumerable<T> GetChildElements<T>()
			where T : CompiledElement
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
		///   Invoked when the element should compile itself. This method is invoked only if no errors occurred during
		///   initialization and validation.
		/// </summary>
		protected virtual void Compile()
		{
		}

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

		/// <summary>
		///   Checks whether the given identifier is reserved.
		/// </summary>
		/// <param name="identifier">The identifier that should be checked.</param>
		protected void ValidateIdentifier(Identifier identifier)
		{
			if (identifier.Name.StartsWith(Configuration.ReservedVariablePrefix))
				Error(identifier, "Identifiers starting with '{0}' are reserved.", Configuration.ReservedVariablePrefix);
		}

		/// <summary>
		///   Checks whether the given type is valid.
		/// </summary>
		/// <param name="node">The node whose type should be checked.</param>
		/// <param name="resolvedType">The declared type of the node.</param>
		protected void ValidateType(AstNode node, IType resolvedType)
		{
			if (resolvedType.ToDataType() == DataType.Unknown)
				Error(node, "Type '{0}' is not a supported data type.", resolvedType.FullName);
		}

		/// <summary>
		///   Checks whether the declared modifiers match the expected ones.
		/// </summary>
		/// <param name="declaration">The declaration that is affected by the modifiers.</param>
		/// <param name="declaredModifiers">The modifiers that have actually been declared.</param>
		/// <param name="expectedModifiers">The modifiers that should have been declared.</param>
		protected void ValidateModifiers(AstNode declaration, IEnumerable<CSharpModifierToken> declaredModifiers,
										 IEnumerable<Modifiers> expectedModifiers)
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			Assert.ArgumentNotNull(declaredModifiers, () => declaredModifiers);
			Assert.ArgumentNotNull(expectedModifiers, () => expectedModifiers);

			ValidateModifiers(declaration, declaredModifiers, new[] { expectedModifiers });
		}

		/// <summary>
		///   Checks whether the declared modifiers match the expected ones.
		/// </summary>
		/// <param name="declaration">The declaration that is affected by the modifiers.</param>
		/// <param name="declaredModifiers">The modifiers that have actually been declared.</param>
		/// <param name="expectedModifiers">The set of modifiers, one of which should have been declared.</param>
		protected void ValidateModifiers(AstNode declaration, IEnumerable<CSharpModifierToken> declaredModifiers,
										 IEnumerable<IEnumerable<Modifiers>> expectedModifiers)
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			Assert.ArgumentNotNull(declaredModifiers, () => declaredModifiers);
			Assert.ArgumentNotNull(expectedModifiers, () => expectedModifiers);

			// Check whether any modifiers other than the exepcted ones are declared 
			foreach (var modifier in declaredModifiers.Where(modifier => !expectedModifiers.Any(m => m.Contains(modifier.Modifier))))
				Error(modifier, "Unexpected modifier '{0}'.", modifier.Modifier.ToString().ToLower());

			// Check that the expected modifiers are present
			if (!expectedModifiers.Any(modifiers => modifiers.All(m => declaredModifiers.Any(declared => declared.Modifier == m))))
			{
				Func<IEnumerable<Modifiers>, string> listModifiers =
					modifiers => String.Join(", ", modifiers.Select(modifier => String.Format("'{0}'", modifier.ToString().ToLower())));

				Error(declaration, "Expected modifiers {0}.", String.Join(" or ", expectedModifiers.Select(listModifiers)));
			}
		}

		/// <summary>
		///   Describes the state of a compiled element.
		/// </summary>
		private enum State
		{
			Uninitialized,
			Initialized,
			Validated,
			Compiled
		}
	}
}