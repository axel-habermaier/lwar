using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Effects
{
	using System.Collections.Generic;
	using System.Linq;
	using AssetsCompiler.Effects;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;
	using Microsoft.CSharp;
	using Effect = AssetsCompiler.Effects.Effect;

	/// <summary>
	///   Represents a C# class that contains cross-compiled shader code and shader constants.
	/// </summary>
	internal class EffectClass : EffectElement
	{
		/// <summary>
		///   The declaration of the class that represents the effect.
		/// </summary>
		private readonly TypeDeclaration _type;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="type">The declaration of the class that represents the effect.</param>
		public EffectClass(TypeDeclaration type)
		{
			Assert.ArgumentNotNull(type, () => type);
			_type = type;
		}

		/// <summary>
		///   Gets the name of the effect class.
		/// </summary>
		public string Name
		{
			get { return _type.Name; }
		}

		/// <summary>
		///   Gets the full name of the effect class.
		/// </summary>
		public string FullName
		{
			get
			{
				var resolved = (TypeResolveResult)Resolver.Resolve(_type);
				return resolved.Type.FullName;
			}
		}

		/// <summary>
		///   Gets the namespace in which the effect class is declared.
		/// </summary>
		public string Namespace
		{
			get
			{
				var resolved = (TypeResolveResult)Resolver.Resolve(_type);
				return resolved.Type.Namespace;
			}
		}

		/// <summary>
		///   Gets the documentation of the effect.
		/// </summary>
		public IEnumerable<string> Documentation
		{
			get { return _type.GetDocumentation(); }
		}

		/// <summary>
		///   Gets the shaders declared by the effect.
		/// </summary>
		public IEnumerable<ShaderMethod> Shaders
		{
			get { return GetChildElements<ShaderMethod>(); }
		}

		/// <summary>
		///   Gets the constant buffers declared by the effect.
		/// </summary>
		public ConstantBuffer[] ConstantBuffers { get; private set; }

		/// <summary>
		///   Gets the shader constants declared by the effect.
		/// </summary>
		private IEnumerable<ShaderConstant> Constants
		{
			get { return GetChildElements<ShaderConstant>(); }
		}

		/// <summary>
		///   Gets the shader literals declared by the effect.
		/// </summary>
		public IEnumerable<ShaderLiteral> Literals
		{
			get { return GetChildElements<ShaderLiteral>(); }
		}

		/// <summary>
		///   Gets the shader texture objects declared by the effect.
		/// </summary>
		public IEnumerable<ShaderTexture> Textures
		{
			get { return GetChildElements<ShaderTexture>(); }
		}

		/// <summary>
		///   Gets the techniques declared by the effect.
		/// </summary>
		public IEnumerable<EffectTechnique> Techniques
		{
			get { return GetChildElements<EffectTechnique>(); }
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all shader literals
			AddElements(from field in _type.Descendants.OfType<FieldDeclaration>()
						where !field.Attributes.Contain<ConstantAttribute>(Resolver)
						let dataType = field.ResolveType(Resolver).ToDataType()
						where dataType != DataType.Texture2D && dataType != DataType.CubeMap
						where field.ResolveType(Resolver).FullName != typeof(Technique).FullName
						from variable in field.Descendants.OfType<VariableInitializer>()
						select new ShaderLiteral(field, variable));

			// Add all shader textures
			var slot = 0;
			AddElements(from field in _type.Descendants.OfType<FieldDeclaration>()
						let dataType = field.ResolveType(Resolver).ToDataType()
						where dataType == DataType.Texture2D || dataType == DataType.CubeMap
						from variable in field.Descendants.OfType<VariableInitializer>()
						select new ShaderTexture(field, variable, slot++));

			// Add all shader methods
			AddElements(from method in _type.Descendants.OfType<MethodDeclaration>()
						let isVertexShader = method.Attributes.Contain<VertexShaderAttribute>(Resolver)
						let isFragmentShader = method.Attributes.Contain<FragmentShaderAttribute>(Resolver)
						where isVertexShader || isFragmentShader
						select new ShaderMethod(method));

			// Add all shader constants
			AddElements(from field in _type.Descendants.OfType<FieldDeclaration>()
						where field.Attributes.Contain<ConstantAttribute>(Resolver)
						from variable in field.Descendants.OfType<VariableInitializer>()
						select new ShaderConstant(field, variable));

			// Create the default constants
			var view = new ShaderConstant("View", DataType.Matrix);
			var projection = new ShaderConstant("Projection", DataType.Matrix);
			var viewProjection = new ShaderConstant("ViewProjection", DataType.Matrix);
			var viewportSize = new ShaderConstant("ViewportSize", DataType.Vector2);

			// Create the default constant buffers
			var constantBuffers = new[]
			{
				new ConstantBuffer("CameraBuffer", 0, new[] { view, projection, viewProjection }, true),
				new ConstantBuffer("ViewportBuffer", 1, new[] { viewportSize }, true)
			};

			// Create the user defined constant buffers
			var count = constantBuffers.Length;
			ConstantBuffers = Constants.GroupBy(constant => constant.ConstantBufferName)
									   .Select(group => new ConstantBuffer(group.Key, count++, group.ToArray()))
									   .Concat(constantBuffers)
									   .OrderBy(buffer => buffer.Slot)
									   .ToArray();

			// Add the default constants
			AddElement(view);
			AddElement(projection);
			AddElement(viewProjection);
			AddElement(viewportSize);

			// Add all techniques
			AddElements(from field in _type.Descendants.OfType<FieldDeclaration>()
						where field.ResolveType(Resolver).FullName == typeof(Technique).FullName
						from variable in field.Descendants.OfType<VariableInitializer>()
						select new EffectTechnique(field, variable, Shaders.ToArray()));
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check whether the Effect attribute has been applied to the class and whether the class is derived from Effect
			var hasBaseType = _type.IsDerivedFrom<Effect>(Resolver);
			var hasAttribute = _type.Attributes.Contain<EffectAttribute>(Resolver);

			if (hasBaseType && !hasAttribute)
				Warn(_type.NameToken, "Effect is missing attribute '{0}'.", typeof(EffectAttribute).FullName);

			if (!hasBaseType && hasAttribute)
				Warn(_type.NameToken, "Expected base type '{0}'.", typeof(Effect).FullName);

			// Check whether 'public' is the only declared modifier 
			ValidateModifiers(_type, _type.ModifierTokens, new[] { Modifiers.Public });

			// Check whether the effect depends on any type arguments
			foreach (var parameter in _type.TypeParameters)
				Error(parameter, "Unexpected type parameter '{0}'.", parameter.Name);

			// Check whether the effect declares any properties
			foreach (var property in _type.Descendants.OfType<PropertyDeclaration>())
				Error(property, "Unexpected property declaration.");

			// Check whether the effect declares any events
			foreach (var eventDeclaration in _type.Descendants.OfType<EventDeclaration>())
				Error(eventDeclaration, "Unexpected event declaration.");

			// Check whether the effect declares any indexers
			foreach (var indexer in _type.Descendants.OfType<IndexerDeclaration>())
				Error(indexer, "Unexpected indexer declaration.");

			// Check whether the effect declares any operators
			foreach (var operatorDeclaration in _type.Descendants.OfType<OperatorDeclaration>())
				Error(operatorDeclaration, "Unexpected operator declaration.");

			// Check that the effect declares at least one vertex shader
			if (Shaders.All(shader => shader.Type != ShaderType.VertexShader))
				Error(_type, "Expected a declaration of at least one vertex shader.");

			// Check that the effect declares at least one fragment shader
			if (Shaders.All(shader => shader.Type != ShaderType.FragmentShader))
				Error(_type, "Expected a declaration of at least one fragment shader.");

			// Check whether that all local variables and parameters do not hide a shader literal, technique, constant, or texture object
			ValidateVariableNames();

			// Check for assignments to shader constants
			foreach (var assignment in from method in _type.Descendants.OfType<MethodDeclaration>()
									   from assignment in method.Descendants.OfType<AssignmentExpression>()
									   from identifier in assignment.Left.Descendants.OfType<IdentifierExpression>()
									   where Constants.Any(constant => constant.Name == identifier.Identifier)
									   select assignment)
			{
				Error(assignment, "Unexpected assignment to shader constant.");
			}

			// Check whether at least one technique is declared
			if (!Techniques.Any())
				Error(_type, "Expected a declaration of at least one technique.");

			// Check whether the name of any declared local variable is reserved
			ValidateLocalVariableNames();

			// Check whether the names assigned to all declared constant buffers are valid identifiers
			ValidateConstantBufferNames();

			// Check whether any shader texture object or shader constant hides the name of a constant buffer
			foreach (var variable in from field in _type.Descendants.OfType<FieldDeclaration>()
									 from variable in field.Variables
									 where ConstantBuffers.Any(buffer => buffer.Name == variable.Name)
									 select variable)
			{
				Error(variable, "Field '{0}' hides constant buffer of the same name.", variable.Name);
			}

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
			CheckUnsupportedCSharpFeatureUsed<ThisReferenceExpression>("this");
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
			foreach (var unaryOperatorExpression in from method in _type.Descendants.OfType<MethodDeclaration>()
													from expression in method.Descendants.OfType<UnaryOperatorExpression>()
													select expression)
			{
				if (unaryOperatorExpression.Operator == UnaryOperatorType.Dereference)
					Error(unaryOperatorExpression.OperatorToken, "Use of unsupported dereference operator.");

				if (unaryOperatorExpression.Operator == UnaryOperatorType.Await)
					Error(unaryOperatorExpression.OperatorToken, "Use of unsupported await operator.");

				if (unaryOperatorExpression.Operator == UnaryOperatorType.AddressOf)
					Error(unaryOperatorExpression.OperatorToken, "Use of unsupported address-of operator.");
			}

			// Check for unsupported binary operators
			foreach (var binaryOperatorExpression in from method in _type.Descendants.OfType<MethodDeclaration>()
													 from expression in method.Descendants.OfType<BinaryOperatorExpression>()
													 select expression)
			{
				if (binaryOperatorExpression.Operator == BinaryOperatorType.NullCoalescing)
					Error(binaryOperatorExpression.OperatorToken, "Use of unsupported null-coalescing operator.");
			}

			// Check for local variable declarations with unsupported data types
			foreach (var variable in from method in _type.Descendants.OfType<MethodDeclaration>()
									 from declaration in method.Descendants.OfType<VariableDeclarationStatement>()
									 from variable in declaration.Variables
									 let type = Resolver.Resolve(variable).Type
									 where type.ToDataType() == DataType.Unknown
									 select new { Declaration = declaration, Type = type })
			{
				Error(variable.Declaration, "Unsupported data type: '{0}'.", variable.Type.FullName);
			}

			// Check for indexer expressions with out-of-bounds indices
			foreach (var indexArgument in from method in _type.Descendants.OfType<MethodDeclaration>()
										  from indexer in method.Descendants.OfType<IndexerExpression>()
										  from argument in indexer.Arguments
										  select new { Indexer = indexer, Argument = argument })
			{
				var resolved = Resolver.Resolve(indexArgument.Indexer.Target);
				var type = resolved.Type.ToDataType();
				resolved = Resolver.Resolve(indexArgument.Argument);

				if (!resolved.IsCompileTimeConstant)
					continue;

				var value = (int)resolved.ConstantValue;
				var matrix = type == DataType.Matrix && value > 3;
				var vector4 = type == DataType.Vector4 && value > 3;
				var vector3 = type == DataType.Vector3 && value > 2;
				var vector2 = type == DataType.Vector2 && value > 1;
				var literalOutOfBounds = false;

				var identifier = indexArgument.Indexer.Target as IdentifierExpression;
				if (identifier != null)
				{
					var literal = Literals.SingleOrDefault(l => l.Name == identifier.Identifier);
					if (literal != null)
						literalOutOfBounds = value >= ((object[])literal.Value).Length;
				}

				if (value < 0 || matrix || vector4 || vector3 || vector2 || literalOutOfBounds)
					Error(indexArgument.Argument, "Array index is out of bounds.");
			}

			// Check for unsupported method invocations
			foreach (var invocation in from method in _type.Descendants.OfType<MethodDeclaration>()
									   from invocation in method.Descendants.OfType<InvocationExpression>()
									   let intrinsic = invocation.ResolveIntrinsic(Resolver)
									   where intrinsic == Intrinsic.Unknown
									   select invocation)
			{
				Error(invocation, "Invocation of unsupported method.");
			}
		}

		/// <summary>
		///   Checks whether there are any local variables or parameters that hide a shader literal, constant, technique, or
		///   texture object.
		/// </summary>
		private void ValidateVariableNames()
		{
			var localVariables = from methodDeclaration in _type.Descendants.OfType<MethodDeclaration>()
								 from variableDeclaration in methodDeclaration.Descendants.OfType<VariableDeclarationStatement>()
								 from variable in variableDeclaration.Variables
								 select new { Node = (AstNode)variable, variable.Name };

			var parameters = from methodDeclaration in _type.Descendants.OfType<MethodDeclaration>()
							 from parameterDeclaration in methodDeclaration.Descendants.OfType<ParameterDeclaration>()
							 select new { Node = (AstNode)parameterDeclaration.NameToken, parameterDeclaration.Name };

			var classVariables = from fieldDeclaration in _type.Descendants.OfType<FieldDeclaration>()
								 from variable in fieldDeclaration.Variables
								 select variable.Name;

			var methodVariables = localVariables.Concat(parameters);

			foreach (var variable in methodVariables.Where(variable => classVariables.Contains(variable.Name)))
				Error(variable.Node, "Local variable or parameter '{0}' hides field of the same name.", variable.Name);
		}

		/// <summary>
		///   Checks whether the name of any locally declared variable is reserved.
		/// </summary>
		private void ValidateLocalVariableNames()
		{
			foreach (var variable in from method in _type.Descendants.OfType<MethodDeclaration>()
									 from variableDeclaration in method.Descendants.OfType<VariableDeclarationStatement>()
									 from variable in variableDeclaration.Variables
									 select variable)
			{
				ValidateIdentifier(variable.NameToken);
			}
		}

		/// <summary>
		///   Reports any usage of the given node type in the methods' bodies as an unsupported C# feature.
		/// </summary>
		/// <typeparam name="T">The type of the unsupported C# syntax element.</typeparam>
		/// <param name="description">The description of the unsupported C# feature.</param>
		private void CheckUnsupportedCSharpFeatureUsed<T>(string description)
			where T : AstNode
		{
			foreach (var node in from method in _type.Descendants.OfType<MethodDeclaration>()
								 from node in method.Body.Descendants.OfType<T>()
								 select node)
			{
				Error(node, "Unsupported C# feature used: {0}.", description);
			}
		}

		/// <summary>
		///   Checks whether the names assigned to all declared constant buffers are valid identifiers.
		/// </summary>
		private void ValidateConstantBufferNames()
		{
			var provider = new CSharpCodeProvider();
			var validCharacters = Enumerable.Range('A', 'Z' - 'A' + 1)
											.Concat(Enumerable.Range('a', 'z' - 'a' + 1))
											.Concat(Enumerable.Range('0', '9' - '0' + 1))
											.Concat(new[] { (int)'_' })
											.Select(c => (char)c)
											.ToArray();

			var buffers = (from field in _type.Descendants.OfType<FieldDeclaration>()
						   let attribute = field.Attributes.GetAttribute<ConstantAttribute>(Resolver)
						   where attribute != null
						   let argument = attribute.Arguments.FirstOrDefault()
						   where argument != null
						   let resolved = Resolver.Resolve(argument)
						   where resolved.IsCompileTimeConstant
						   let name = (string)resolved.ConstantValue
						   select new { Argument = argument, Name = name }).ToArray();

			foreach (var buffer in buffers.Where(buffer => buffer.Name.StartsWith(Configuration.ReservedVariablePrefix)))
				Error(buffer.Argument, "Identifiers starting with '{0}' are reserved.", Configuration.ReservedVariablePrefix);

			foreach (
				var buffer in
					buffers.Where(buffer => !provider.IsValidIdentifier(buffer.Name) || buffer.Name.Any(c => !validCharacters.Contains(c))))
				Error(buffer.Argument, "Invalid constant buffer name '{0}'.", buffer.Name);
		}
	}
}