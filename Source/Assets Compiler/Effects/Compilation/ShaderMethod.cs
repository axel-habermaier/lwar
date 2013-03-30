using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;

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
		///   Gets the input parameters declared by the shader method.
		/// </summary>
		private IEnumerable<ShaderParameter> Inputs
		{
			get { return GetChildElements<ShaderParameter>().Where(parameter => !parameter.IsOutput); }
		}

		/// <summary>
		///   Gets the output parameters declared by the shader method.
		/// </summary>
		private IEnumerable<ShaderParameter> Outputs
		{
			get { return GetChildElements<ShaderParameter>().Where(parameter => parameter.IsOutput); }
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

		///// <summary>
		/////   Gets the cross-compiled shader asset that can subsequently compiled into the binary format.
		///// </summary>
		//public Asset Asset { get; private set; }

		///// <summary>
		/////   Gets the name of the shader.
		///// </summary>
		//public string Name { get; private set; }

		///// <summary>
		/////   Gets the shader inputs.
		///// </summary>
		//public ShaderParameter[] Inputs { get; private set; }

		///// <summary>
		/////   Gets the shader outputs.
		///// </summary>
		//public ShaderParameter[] Outputs { get; private set; }

		///// <summary>
		/////   Gets the local variables of the shader.
		///// </summary>
		//public ShaderVariable[] Variables { get; private set; }

		///// <summary>
		/////   Gets the shader parameters, both inputs and outputs.
		///// </summary>
		//public IEnumerable<ShaderParameter> Parameters
		//{
		//	get { return Inputs.Union(Outputs); }
		//}

		///// <summary>
		/////   Gets the C# shader code.
		///// </summary>
		//public AstNode ShaderCode
		//{
		//	get { return _method.Body; }
		//}

		///// <summary>
		/////   Gets the syntax tree for the shader.
		///// </summary>
		//public IAstNode SyntaxTree { get; private set; }

		///// <summary>
		/////   Returns a string that represents the current object.
		///// </summary>
		//public override string ToString()
		//{
		//	return String.Format("{0} ({1})", Name, Type);
		//}

		///// <summary>
		/////   Compiles the shader method.
		///// </summary>
		///// <param name="context">The context of the compilation.</param>
		///// <param name="effect">The effect the shader belongs to.</param>
		//public void Compile(CompilationContext context, EffectClass effect)
		//{
		//	Name = _method.Name;
		//	GetParameters(context);

		//	if (_method.GetType(context).FullName != typeof(void).FullName)
		//		context.Error(_method, "Shader method '{0}' must have return type 'void'.", Name);

		//	if (_method.TypeParameters.Any() || _method.Modifiers != Modifiers.Public)
		//		context.Error(_method, "Shader '{0}' must be a public, non-static, non-partial, non-abstract, non-sealed, " +
		//							   "non-virtual method without any type arguments.", Name);

		//	switch (Type)
		//	{
		//		case ShaderType.VertexShader:
		//			if (Outputs.All(o => o.Semantics != DataSemantics.Position))
		//				context.Error(_method, "Vertex shader '{0}' must declare an output parameter with the '{1}' semantics.",
		//							  Name, DataSemantics.Position.ToDisplayString());

		//			for (var i = 0; i < Outputs.Length; ++i)
		//			{
		//				if (Outputs[i].Semantics == DataSemantics.Position)
		//				{
		//					var output = Outputs[i];
		//					var lastIndex = Outputs.Length - 1;
		//					Outputs[i] = Outputs[lastIndex];
		//					Outputs[lastIndex] = output;
		//					break;
		//				}
		//			}
		//			break;
		//		case ShaderType.FragmentShader:
		//			if (Outputs.All(o => !o.Semantics.IsColor()))
		//				context.Error(_method, "Fragment shader '{0}' must declare an output parameter with the 'Color' semantics.", Name);
		//			break;
		//		default:
		//			throw new InvalidOperationException("Unsupported shader type.");
		//	}

		//	CheckDistinctSemantics(context, Inputs, "input");
		//	CheckDistinctSemantics(context, Outputs, "output");

		//	GetLocalVariables(context, effect);
		//	SyntaxTree = new AstCreator().CreateAst(context, effect, this);
		//}

		///// <summary>
		/////   Gets the parameters of the shader.
		///// </summary>
		///// <param name="context">The context of the compilation.</param>
		//private void GetParameters(CompilationContext context)
		//{
		//	var parameters = _method.Descendants.OfType<ParameterDeclaration>()
		//							.Select(parameter =>
		//								{
		//									var shaderParameter = new ShaderParameter(parameter);
		//									shaderParameter.Compile(context);

		//									if (shaderParameter.IsOutput && Type == ShaderType.FragmentShader && !shaderParameter.Semantics.IsColor())
		//										context.Error(parameter, "Fragment shader '{0}' cannot assign '{2}' semantics to output parameter '{1}'.",
		//													  Name, shaderParameter.Name, shaderParameter.Semantics.ToDisplayString());

		//									return shaderParameter;
		//								})
		//							.ToArray();

		//	Inputs = parameters.Where(parameter => !parameter.IsOutput).ToArray();
		//	Outputs = parameters.Where(parameter => parameter.IsOutput).ToArray();
		//}

		///// <summary>
		/////   Gets the local variables of the shader.
		///// </summary>
		///// <param name="context">The context of the compilation.</param>
		///// <param name="effect">The effect the shader belongs to.</param>
		//private void GetLocalVariables(CompilationContext context, EffectClass effect)
		//{
		//	Variables = _method.Descendants.OfType<VariableDeclarationStatement>()
		//					   .SelectMany(declaration => declaration.Variables.Select(variable =>
		//						   {
		//							   var shaderVariable = new ShaderVariable(declaration, variable);
		//							   shaderVariable.Compile(context);

		//							   if (Parameters.Any(parameter => parameter.Name == variable.Name))
		//								   context.Error(variable, "Local variable '{0}' hides parameter of the same name.", shaderVariable.Name);

		//							   if (effect.Textures.Any(texture => texture.Name == variable.Name))
		//								   context.Error(variable, "Local variable '{0}' hides shader texture object of the same name.",
		//												 shaderVariable.Name);

		//							   if (effect.Constants.Any(constant => constant.Name == variable.Name))
		//								   context.Error(variable, "Local variable '{0}' hides shader constant of the same name.", shaderVariable.Name);

		//							   if (effect.Literals.Any(literal => literal.Name == variable.Name))
		//								   context.Error(variable, "Local variable '{0}' hides shader literal of the same name.", shaderVariable.Name);

		//							   return shaderVariable;
		//						   }))
		//					   .ToArray();
		//}

		///// <summary>
		/////   Checks whether the given parameters are declared with distinct semantics.
		///// </summary>
		///// <param name="context">The context of the compilation.</param>
		///// <param name="parameters">The parameters that should be checked.</param>
		///// <param name="direction">A description of the parameter direction.</param>
		//private void CheckDistinctSemantics(CompilationContext context, IEnumerable<ShaderParameter> parameters, string direction)
		//{
		//	var groups = parameters.GroupBy(parameter => parameter.Semantics).Where(group => group.Count() > 1);
		//	foreach (var semanticsGroup in groups)
		//	{
		//		context.Error(_method, "Shader '{0}' declares multiple {2} parameters with the '{1}' semantics.",
		//					  Name, semanticsGroup.First().Semantics.ToDisplayString(), direction);
		//	}
		//}

		///// <summary>
		/////   Generates the code for the shader.
		///// </summary>
		///// <param name="context">The context of the compilation.</param>
		///// <param name="effect">The effect the shader belongs to.</param>
		//public void GenerateCode(CompilationContext context, EffectClass effect)
		//{
		//	var assetPath = String.Format("{0}_{1}_{2}", context.File.Asset.RelativePath, effect.FullName, Name);
		//	switch (Type)
		//	{
		//		case ShaderType.VertexShader:
		//			Asset = new VertexShaderAsset(String.Format("{0}.vs", assetPath), Configuration.TempDirectory);
		//			break;
		//		case ShaderType.FragmentShader:
		//			Asset = new FragmentShaderAsset(String.Format("{0}.fs", assetPath), Configuration.TempDirectory);
		//			break;
		//		default:
		//			throw new InvalidOperationException("Unsupported shader type.");
		//	}

		//	var writer = new CodeWriter();
		//	new GlslCrossCompiler().GenerateCode(context, effect, this, writer);

		//	writer.Newline();
		//	writer.AppendLine(Configuration.ShaderSeparator);
		//	writer.Newline();

		//	new HlslCrossCompiler().GenerateCode(context, effect, this, writer);

		//	writer.WriteToFile(Asset.SourcePath);
		//}
	}
}