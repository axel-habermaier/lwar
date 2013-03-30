using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a C# method that is cross-compiled to GLSL or HLSL.
	/// </summary>
	internal class ShaderMethod : CompiledElement
	{
		/// <summary>
		///   The declaration of the method that represents the shader.
		/// </summary>
		private readonly MethodDeclaration _method;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="method">The declaration of the method that represents the shader.</param>
		public ShaderMethod(MethodDeclaration method)
		{
			Assert.ArgumentNotNull(method, () => method);
			_method = method;
		}

		/// <summary>
		///   Gets the name of the shader.
		/// </summary>
		public string Name
		{
			get { return _method.Name; }
		}

		/// <summary>
		///   Gets the type of the shader.
		/// </summary>
		public ShaderType Type
		{
			get
			{
				if (_method.Attributes.Contain<VertexShaderAttribute>(Resolver))
					return ShaderType.VertexShader;

				if (_method.Attributes.Contain<FragmentShaderAttribute>(Resolver))
					return ShaderType.FragmentShader;

				throw new InvalidOperationException("Unsupported shader type.");
			}
		}

		/// <summary>
		///   Gets the syntax tree of the shader method's body.
		/// </summary>
		public BlockStatement MethodBody
		{
			get { return _method.Body; }
		}

		/// <summary>
		///   Gets the input parameters declared by the shader method.
		/// </summary>
		public IEnumerable<ShaderParameter> Inputs
		{
			get { return GetChildElements<ShaderParameter>().Where(parameter => !parameter.IsOutput); }
		}

		/// <summary>
		///   Gets the output parameters declared by the shader method.
		/// </summary>
		public IEnumerable<ShaderParameter> Outputs
		{
			get { return GetChildElements<ShaderParameter>().Where(parameter => parameter.IsOutput); }
		}

		/// <summary>
		///   Gets all parameters declared by the shader method.
		/// </summary>
		public IEnumerable<ShaderParameter> Parameters
		{
			get { return GetChildElements<ShaderParameter>(); }
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all shader parameters
			AddElements(from parameter in _method.Descendants.OfType<ParameterDeclaration>()
						select new ShaderParameter(parameter));
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check whether both the vertex and fragment shader attributes are declared on the method
			var isVertexShader = _method.Attributes.Contain<VertexShaderAttribute>(Resolver);
			var isFragmentShader = _method.Attributes.Contain<FragmentShaderAttribute>(Resolver);

			if (isVertexShader && isFragmentShader)
				Error(_method.Attributes.First(), "Unexpected declaration of both '{0}' and '{1}'.",
					  typeof(VertexShaderAttribute).FullName, typeof(FragmentShaderAttribute).FullName);

			// Check whether the method returns void
			if (_method.ResolveType(Resolver).FullName != typeof(void).FullName)
				Error(_method.ReturnType, "Expected return type 'void'.");

			// Check whether 'public' is the only declared modifier 
			ValidateModifiers(_method, _method.ModifierTokens, new[] { Modifiers.Public });

			// Check whether the shader depends on any type arguments
			foreach (var parameter in _method.TypeParameters)
				Error(parameter, "Unexpected type parameter '{0}'.", parameter.Name);

			// Check whether the vertex shader declares an output parameter with the Position semantics
			if (Type == ShaderType.VertexShader && Outputs.All(output => output.Semantics != DataSemantics.Position))
				Error(_method, "Expected an output parameter with the '{0}' semantics.", DataSemantics.Position.ToDisplayString());

			// Check whether the fragment shader declares an output parameter with the Color semantics
			if (Type == ShaderType.FragmentShader && Outputs.All(output => !output.Semantics.IsColor()))
				Error(_method, "Expected an output parameter with the 'Color' semantics.");

			// Check whether the fragment shader declares any output parameters that do not have the color semantics
			if (Type == ShaderType.FragmentShader)
			{
				foreach (var output in from parameter in _method.Descendants.OfType<ParameterDeclaration>()
									   where parameter.ParameterModifier == ParameterModifier.Out
									   from attribute in parameter.GetSemantics(Resolver)
									   let semantics = attribute.ToSemanticsAttribute(Resolver)
									   where !semantics.Semantics.IsColor()
									   select new { Attribute = attribute, semantics.Semantics })
				{
					Error(output.Attribute, "Unexpected '{0}' semantics.", output.Semantics.ToDisplayString());
				}
			}

			// Check whether the all inputs and outputs have distinct semantics
			ValidateSemantics(Inputs, "input");
			ValidateSemantics(Outputs, "output");

			// Check whether the name of any declared local variable is reserved
			ValidateLocalVariableNames();

			// Check that no unsupported C# features are used by the method body
			CheckUnsupportedCSharpFeatureUsed<PreProcessorDirective>("preprocessor directive");
			CheckUnsupportedCSharpFeatureUsed<CheckedStatement>("checked");
			CheckUnsupportedCSharpFeatureUsed<FixedStatement>("fixed");
			CheckUnsupportedCSharpFeatureUsed<GotoCaseStatement>("goto");
			CheckUnsupportedCSharpFeatureUsed<ForeachStatement>("foreach");
			CheckUnsupportedCSharpFeatureUsed<GotoDefaultStatement>("goto");
			CheckUnsupportedCSharpFeatureUsed<GotoStatement>("goto");
			CheckUnsupportedCSharpFeatureUsed<LabelStatement>("label");
			CheckUnsupportedCSharpFeatureUsed<LockStatement>("lock");
			CheckUnsupportedCSharpFeatureUsed<ThrowStatement>("throw");
			CheckUnsupportedCSharpFeatureUsed<TryCatchStatement>("try-catch");
			CheckUnsupportedCSharpFeatureUsed<CatchClause>("catch");
			CheckUnsupportedCSharpFeatureUsed<UncheckedStatement>("unchecked");
			CheckUnsupportedCSharpFeatureUsed<UnsafeStatement>("unsafe");
			CheckUnsupportedCSharpFeatureUsed<UsingStatement>("using");
			CheckUnsupportedCSharpFeatureUsed<YieldBreakStatement>("yield break");
			CheckUnsupportedCSharpFeatureUsed<YieldReturnStatement>("yield return");
			CheckUnsupportedCSharpFeatureUsed<AnonymousMethodExpression>("anonymous method");
			CheckUnsupportedCSharpFeatureUsed<LambdaExpression>("lambda function");
			CheckUnsupportedCSharpFeatureUsed<BaseReferenceExpression>("base");
			CheckUnsupportedCSharpFeatureUsed<CheckedExpression>("checked");
			CheckUnsupportedCSharpFeatureUsed<NullReferenceExpression>("null");
			CheckUnsupportedCSharpFeatureUsed<AnonymousTypeCreateExpression>("anonymous type");
			CheckUnsupportedCSharpFeatureUsed<ArrayCreateExpression>("dynamic array initialization");
			CheckUnsupportedCSharpFeatureUsed<PointerReferenceExpression>("pointer");
			CheckUnsupportedCSharpFeatureUsed<SizeOfExpression>("sizeof");
			CheckUnsupportedCSharpFeatureUsed<StackAllocExpression>("stackalloc");
			CheckUnsupportedCSharpFeatureUsed<TypeOfExpression>("typeof");
			CheckUnsupportedCSharpFeatureUsed<UncheckedExpression>("unchecked");
			CheckUnsupportedCSharpFeatureUsed<QueryExpression>("query");
			CheckUnsupportedCSharpFeatureUsed<QueryContinuationClause>("query");
			CheckUnsupportedCSharpFeatureUsed<QueryFromClause>("from");
			CheckUnsupportedCSharpFeatureUsed<QueryLetClause>("let");
			CheckUnsupportedCSharpFeatureUsed<QueryWhereClause>("where");
			CheckUnsupportedCSharpFeatureUsed<QueryJoinClause>("join");
			CheckUnsupportedCSharpFeatureUsed<QueryOrderClause>("orderby");
			CheckUnsupportedCSharpFeatureUsed<QueryOrdering>("ordering");
			CheckUnsupportedCSharpFeatureUsed<QuerySelectClause>("select");
			CheckUnsupportedCSharpFeatureUsed<QueryGroupClause>("groupby");
			CheckUnsupportedCSharpFeatureUsed<AsExpression>("as");
			CheckUnsupportedCSharpFeatureUsed<IsExpression>("is");
			CheckUnsupportedCSharpFeatureUsed<DefaultValueExpression>("default");
			CheckUnsupportedCSharpFeatureUsed<UndocumentedExpression>("undocumented expression");
			CheckUnsupportedCSharpFeatureUsed<ArrayInitializerExpression>("dynamic array initialization");
			CheckUnsupportedCSharpFeatureUsed<NamedArgumentExpression>("named arguments");
			CheckUnsupportedCSharpFeatureUsed<SwitchStatement>("switch statement");
			CheckUnsupportedCSharpFeatureUsed<SwitchSection>("switch statement");
			CheckUnsupportedCSharpFeatureUsed<CaseLabel>("case label");

			// Check for unsupported unary operators
			foreach (var unaryOperatorExpression in _method.Descendants.OfType<UnaryOperatorExpression>())
			{
				if (unaryOperatorExpression.Operator == UnaryOperatorType.Dereference)
					Error(unaryOperatorExpression.OperatorToken, "Use of unsupported dereference operator.");

				if (unaryOperatorExpression.Operator == UnaryOperatorType.Await)
					Error(unaryOperatorExpression.OperatorToken, "Use of unsupported await operator.");

				if (unaryOperatorExpression.Operator == UnaryOperatorType.AddressOf)
					Error(unaryOperatorExpression.OperatorToken, "Use of unsupported address-of operator.");
			}

			// Check for unsupported binary operators
			foreach (var binaryOperatorExpression in _method.Descendants.OfType<BinaryOperatorExpression>())
			{
				if (binaryOperatorExpression.Operator == BinaryOperatorType.NullCoalescing)
					Error(binaryOperatorExpression.OperatorToken, "Use of unsupported null-coalescing operator.");
			}

			// Check for local variable declarations with unsupported data types
			foreach (var variable in from declaration in _method.Descendants.OfType<VariableDeclarationStatement>()
									 from variable in declaration.Variables
									 let type = Resolver.Resolve(variable).Type
									 where type.ToDataType() == DataType.Unknown
									 select new { Declaration = declaration, Type = type })
			{
				Error(variable.Declaration, "Unsupported data type: '{0}'.", variable.Type.FullName);
			}
		}

		/// <summary>
		///   Checks whether the given parameters are declared with distinct semantics.
		/// </summary>
		/// <param name="parameters">The parameters that should be checked.</param>
		/// <param name="direction">A description of the parameter direction.</param>
		private void ValidateSemantics(IEnumerable<ShaderParameter> parameters, string direction)
		{
			var groups = from parameter in parameters
						 group parameter by parameter.Semantics
						 into semantics
						 where semantics.Count() > 1
						 select semantics;

			foreach (var group in groups)
			{
				var semantics = group.First().Semantics;
				Error(_method, "Semantics '{0}' is applied to more than one {1} parameter.", semantics.ToDisplayString(), direction);
			}
		}

		/// <summary>
		///   Check whether the name of any locally declared variable is reserved.
		/// </summary>
		private void ValidateLocalVariableNames()
		{
			var variables = from variableDeclaration in _method.Descendants.OfType<VariableDeclarationStatement>()
							from variable in variableDeclaration.Variables
							where variable.Name.StartsWith(Configuration.ReservedVariablePrefix)
							select variable;

			foreach (var variable in variables)
				ValidateIdentifier(variable.NameToken);
		}

		/// <summary>
		///   Reports any usage of the given node type in the shader method's body as an unsupported C# feature.
		/// </summary>
		/// <typeparam name="T">The type of the unsupported C# syntax element.</typeparam>
		/// <param name="description">The description of the unsupported C# feature.</param>
		private void CheckUnsupportedCSharpFeatureUsed<T>(string description)
			where T : AstNode
		{
			foreach (var node in _method.Body.Descendants.OfType<T>())
				Error(node, "Unsupported C# feature used: {0}.", description);
		}
	}
}