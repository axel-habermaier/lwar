using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a C# class that contains cross-compiled shader code and shader constants.
	/// </summary>
	internal class EffectClass : CompiledElement
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
		///   The name of the effect class.
		/// </summary>
		private string Name
		{
			get { return _type.Name; }
		}

		/// <summary>
		///   Gets the shaders declared by the effect.
		/// </summary>
		private IEnumerable<ShaderMethod> Shaders
		{
			get { return GetChildElements<ShaderMethod>(); }
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all shader literals
			AddElements(from field in _type.Descendants.OfType<FieldDeclaration>()
						where !field.Attributes.Contain<ShaderConstantAttribute>(Resolver)
						let dataType = field.ResolveType(Resolver).ToDataType()
						where dataType != DataType.Texture2D && dataType != DataType.CubeMap
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
						where field.Attributes.Contain<ShaderConstantAttribute>(Resolver)
						from variable in field.Descendants.OfType<VariableInitializer>()
						select new ShaderConstant(field, variable));

			// Add the default constants
			AddElement(new ShaderConstant("View", DataType.Matrix));
			AddElement(new ShaderConstant("Projection", DataType.Matrix));
			AddElement(new ShaderConstant("ViewProjection", DataType.Matrix));
			AddElement(new ShaderConstant("ViewportSize", DataType.Vector2));
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
				Error(property.NameToken, "Unexpected property '{0}'.", property.Name);

			// Check that the effect declares at least one vertex shader
			if (Shaders.All(shader => shader.Type != ShaderType.VertexShader))
				Error(_type, "Expected a declaration of at least one vertex shaders.");

			// Check that the effect declares at least one fragment shader
			if (Shaders.All(shader => shader.Type != ShaderType.FragmentShader))
				Error(_type, "Expected a declaration of at least one fragment shaders.");

			// Check whether that all local variables and parameters do not hide a shader literal, constant, or texture object
			CheckForVariableHiding();
		}

		/// <summary>
		///   Invoked when the element should compile itself. This method is invoked only if no errors occurred during
		///   initialization and validation.
		/// </summary>
		protected override void Compile()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Checks whether there are any local variables or parameters that hide a shader literal, constant, or texture object.
		/// </summary>
		private void CheckForVariableHiding()
		{
			var localVariables = from methodDeclaration in _type.Descendants.OfType<MethodDeclaration>()
								 from variableDeclaration in methodDeclaration.Descendants.OfType<VariableDeclarationStatement>()
								 from variable in variableDeclaration.Variables
								 select new { Node = (AstNode)variable, variable.Name };

			var parameters = from methodDeclaration in _type.Descendants.OfType<MethodDeclaration>()
							 from parameterDeclaration in methodDeclaration.Descendants.OfType<ParameterDeclaration>()
							 select new { Node = (AstNode)parameterDeclaration, parameterDeclaration.Name };

			var classVariables = from fieldDeclaration in _type.Descendants.OfType<FieldDeclaration>()
								 from variable in fieldDeclaration.Variables
								 select variable.Name;

			var methodVariables = localVariables.Union(parameters);

			foreach (var variable in methodVariables.Where(variable => classVariables.Contains(variable.Name)))
				Error(variable.Node, "Local variable or parameter '{0}' hides field of the same name.", variable.Name);
		}

		///// <summary>
		/////   Gets the shaders defined by the effect.
		///// </summary>
		//public ShaderMethod[] Shaders { get; private set; }

		///// <summary>
		/////   Gets the shader constants accessed by the effect.
		///// </summary>
		//public ShaderConstant[] Constants { get; private set; }

		///// <summary>
		/////   Gets the compile-time constant literals accessed by the effect.
		///// </summary>
		//public ShaderLiteral[] Literals { get; private set; }

		///// <summary>
		/////   Gets the texture objects accessed by the effect.
		///// </summary>
		//public ShaderTexture[] Textures { get; private set; }

		///// <summary>
		/////   Gets the constant buffers that are accessed by the effect.
		///// </summary>
		//public ConstantBuffer[] ConstantBuffers { get; private set; }

		///// <summary>
		/////   Gets the shader assets that have been generated during the compilation process.
		///// </summary>
		//public IEnumerable<Asset> ShaderAssets
		//{
		//	get { return _effects.SelectMany(effect => effect.ShaderAssets); }
		//}

		///// <summary>
		/////   Gets the name of the effect.
		///// </summary>
		//public string Name { get; private set; }

		///// <summary>
		/////   Gets the full name of the effect.
		///// </summary>
		//public string FullName { get; private set; }

		///// <summary>
		/////   Compiles the effect.
		///// </summary>
		//public void Compile()
		//{
		//	Name = _type.GetFullName(context);
		//	FullName = _type.GetFullName(context);

		//	GetShaderLiterals(context);
		//	GetShaderConstants(context);
		//	GetShaderTextures(context);
		//	GetShaderMethods(context);

		//	if (effectClass.HasBaseType && !effectClass.HasAttribute)
		//		context.Warn(type.NameToken,
		//					 "Expected attribute '{0}' to be declared on effect '{1}'.",
		//					 typeof(EffectAttribute).FullName, type.GetFullName(context));

		//	if (!effectClass.HasBaseType && effectClass.HasAttribute)
		//		context.Warn(type.NameToken,
		//					 "Expected effect '{0}' to have base type '{1}'.",
		//					 type.GetFullName(context), typeof(Effect).FullName);

		//	if (_type.TypeParameters.Any() || _type.Modifiers != Modifiers.Public)
		//		context.Error(_type,
		//					  "Effect '{0}' must be a public, non-static, non-partial, non-abstract, non-sealed class without any type arguments.",
		//					  Name);

		//	foreach (var property in _type.Descendants.OfType<PropertyDeclaration>())
		//		context.Error(property.NameToken, "Unexpected property '{0}' declared by effect '{1}'.", property.Name, Name);

		//	if (Shaders.All(shader => shader.Type != ShaderType.VertexShader))
		//		context.Error(_type, "Effect '{0}' must declare at least one vertex shader.", Name);

		//	if (Shaders.All(shader => shader.Type != ShaderType.FragmentShader))
		//		context.Error(_type, "Effect '{0}' must declare at least one fragment shader.", Name);

		//	GenerateCode(context);
		//}

		//public void GenerateAssets()
		//{

		//}

		///// <summary>
		/////   Gets the shader literals from the effect.
		///// </summary>
		///// <param name="context">The context of the compilation.</param>
		//private void GetShaderLiterals(CompilationContext context)
		//{
		//	var literals = from field in _type.Descendants.OfType<FieldDeclaration>()
		//				   where !field.HasAttribute<ShaderConstantAttribute>(context)
		//				   let dataType = field.GetDataType(context)
		//				   where dataType != DataType.Texture2D && dataType != DataType.CubeMap
		//				   from variable in field.Descendants.OfType<VariableInitializer>()
		//				   select new ShaderLiteral(field, variable);

		//	Literals = literals.ToArray();

		//	foreach (var literal in Literals)
		//		literal.Compile(context);
		//}

		///// <summary>
		/////   Gets the shader constants from the effect.
		///// </summary>
		///// <param name="context">The context of the compilation.</param>
		//private void GetShaderConstants(CompilationContext context)
		//{
		//	var constants = from field in _type.Descendants.OfType<FieldDeclaration>()
		//					let attribute = field.GetAttribute<ShaderConstantAttribute>(context)
		//					where attribute != null
		//					from variable in field.Descendants.OfType<VariableInitializer>()
		//					select new ShaderConstant(field, variable);

		//	Constants = constants.ToArray();

		//	foreach (var constant in Constants)
		//		constant.Compile(context);

		//	var view = new ShaderConstant("View", DataType.Matrix);
		//	var projection = new ShaderConstant("Projection", DataType.Matrix);
		//	var viewProjection = new ShaderConstant("ViewProjection", DataType.Matrix);
		//	var viewportSize = new ShaderConstant("ViewportSize", DataType.Vector2);

		//	var constantBuffers = new[]
		//	{
		//		new ConstantBuffer(0, new[] { view, projection, viewProjection }, true),
		//		new ConstantBuffer(1, new[] { viewportSize }, true)
		//	};

		//	var count = constantBuffers.Length;
		//	ConstantBuffers = Constants.GroupBy(constant => constant.ChangeFrequency)
		//							   .Select(group => new ConstantBuffer(count++, group.ToArray()))
		//							   .Union(constantBuffers)
		//							   .OrderBy(buffer => buffer.Slot)
		//							   .ToArray();

		//	var defaultConstants = new[] { view, projection, viewProjection, viewportSize };
		//	Constants = Constants.Union(defaultConstants).ToArray();
		//}

		///// <summary>
		/////   Gets the shader textures from the effect.
		///// </summary>
		///// <param name="context">The context of the compilation.</param>
		//private void GetShaderTextures(CompilationContext context)
		//{
		//	var slot = 0;
		//	var textures = from field in _type.Descendants.OfType<FieldDeclaration>()
		//				   let dataType = field.GetDataType(context)
		//				   where dataType == DataType.Texture2D || dataType == DataType.CubeMap
		//				   from variable in field.Descendants.OfType<VariableInitializer>()
		//				   select new ShaderTexture(field, variable, slot++);

		//	Textures = textures.ToArray();

		//	foreach (var texture in Textures)
		//		texture.Compile(context);
		//}

		///// <summary>
		/////   Gets the shader methods from the effect.
		///// </summary>
		///// <param name="context">The context of the compilation.</param>
		//private void GetShaderMethods(CompilationContext context)
		//{
		//	var methods = from method in _type.Descendants.OfType<MethodDeclaration>()
		//				  let isVertexShader = method.HasAttribute<VertexShaderAttribute>(context)
		//				  let isFragmentShader = method.HasAttribute<FragmentShaderAttribute>(context)
		//				  let shaderType = isVertexShader ? ShaderType.VertexShader : ShaderType.FragmentShader
		//				  where isVertexShader || isFragmentShader
		//				  select new { Declaration = method, Type = shaderType, HasUnknownType = isVertexShader && isFragmentShader };

		//	Shaders = methods.Select(method =>
		//		{
		//			if (method.HasUnknownType)
		//				context.Error(method.Declaration, "Shader method '{0}' cannot be both a vertex shader and a fragment shader.",
		//							  method.Declaration.Name);

		//			var shaderMethod = new ShaderMethod(method.Declaration, method.Type);
		//			shaderMethod.Compile(context,this);
		//			return shaderMethod;
		//		}).ToArray();
		//}

		///// <summary>
		/////   Generates the code for the effect.
		///// </summary>
		///// <param name="context">The context of the compilation.</param>
		//private void GenerateCode(CompilationContext context)
		//{
		//	foreach (var shader in Shaders)
		//		shader.GenerateCode(context, this);
		//}
	}
}