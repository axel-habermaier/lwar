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
		///   Gets the constant buffers declared by the effect.
		/// </summary>
		public ConstantBuffer[] ConstantBuffers { get; private set; }

		/// <summary>
		///   Gets the constants declared by the effect.
		/// </summary>
		private IEnumerable<ShaderConstant> Constants
		{
			get { return GetChildElements<ShaderConstant>(); }
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

			// Create the default constants
			var view = new ShaderConstant("View", DataType.Matrix);
			var projection = new ShaderConstant("Projection", DataType.Matrix);
			var viewProjection = new ShaderConstant("ViewProjection", DataType.Matrix);
			var viewportSize = new ShaderConstant("ViewportSize", DataType.Vector2);

			// Create the default constant buffers
			var constantBuffers = new[]
			{
				new ConstantBuffer(0, new[] { view, projection, viewProjection }, true),
				new ConstantBuffer(1, new[] { viewportSize }, true)
			};

			// Create the user defined constant buffers
			var count = constantBuffers.Length;
			ConstantBuffers = Constants.GroupBy(constant => constant.ChangeFrequency)
									   .Select(group => new ConstantBuffer(count++, group.ToArray()))
									   .Union(constantBuffers)
									   .OrderBy(buffer => buffer.Slot)
									   .ToArray();

			// Add the default constants
			AddElement(view);
			AddElement(projection);
			AddElement(viewProjection);
			AddElement(viewportSize);
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

			// Check whether that all local variables and parameters do not hide a shader literal, constant, or texture object
			ValidateVariableNames();
		}

		/// <summary>
		///   Checks whether there are any local variables or parameters that hide a shader literal, constant, or texture object.
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

			var methodVariables = localVariables.Union(parameters);

			foreach (var variable in methodVariables.Where(variable => classVariables.Contains(variable.Name)))
				Error(variable.Node, "Local variable or parameter '{0}' hides field of the same name.", variable.Name);
		}
	}
}