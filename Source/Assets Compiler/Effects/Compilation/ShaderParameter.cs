using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Linq;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.TypeSystem;
	using Semantics;

	/// <summary>
	///   Represents a parameter of a shader.
	/// </summary>
	internal class ShaderParameter : CompiledElement
	{
		/// <summary>
		///   The declaration of the method parameter that represents the shader parameter.
		/// </summary>
		private readonly ParameterDeclaration _parameter;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parameter">The declaration of the method parameter that represents the shader parameter.</param>
		public ShaderParameter(ParameterDeclaration parameter)
		{
			Assert.ArgumentNotNull(parameter, () => parameter);
			_parameter = parameter;
		}

		/// <summary>
		///   Gets the name of the parameter
		/// </summary>
		public string Name
		{
			get { return _parameter.Name; }
		}

		/// <summary>
		///   Gets the type of the parameter.
		/// </summary>
		public DataType Type
		{
			get { return _parameter.ResolveType(Resolver).ToDataType(); }
		}

		/// <summary>
		///   Gets the semantics of the shader parameter.
		/// </summary>
		public DataSemantics Semantics
		{
			get
			{
				var position = _parameter.Attributes.Contain<PositionAttribute>(Resolver);
				var normal = _parameter.Attributes.Contain<NormalAttribute>(Resolver);
				var texCoords = _parameter.Attributes.GetAttribute<TexCoordsAttribute>(Resolver);
				var color = _parameter.Attributes.GetAttribute<ColorAttribute>(Resolver);

				if (position)
					return DataSemantics.Position;

				if (normal)
					return DataSemantics.Normal;

				if (texCoords != null)
				{
					var index = GetSemanticIndex(texCoords);
					return DataSemantics.TexCoords0 + index;
				}

				if (color != null)
				{
					var index = GetSemanticIndex(color);
					return DataSemantics.Color0 + index;
				}

				// Just return some meaningless default semantics; this error will be catched during the validation of the parameter.
				return DataSemantics.TexCoords0;
			}
		}

		/// <summary>
		///   Gets a value indicating whether the parameter is a shader output.
		/// </summary>
		public bool IsOutput
		{
			get { return _parameter.ParameterModifier == ParameterModifier.Out; }
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check whether the name is reserved
			ValidateIdentifier(_parameter.NameToken);

			// Check whether the parameter is declared with a known type
			ValidateType(_parameter, _parameter.ResolveType(Resolver));

			// Check whether the parameter is an array type
			if (_parameter.ResolveType(Resolver).Kind == TypeKind.Array)
				Error(_parameter, "Unexpected array declaration.");

			// Check whether the parameter is declared with modifier 'out' or no modifier at all
			if (_parameter.ParameterModifier != ParameterModifier.Out && _parameter.ParameterModifier != ParameterModifier.None)
				Error(_parameter, "Unexpected modifier '{0}'.", _parameter.ParameterModifier.ToString().ToLower());

			// Check whether the parameter is declared with any semantics or with multiple semantics
			var position = _parameter.Attributes.Contain<PositionAttribute>(Resolver);
			var normal = _parameter.Attributes.Contain<NormalAttribute>(Resolver);
			var texCoords = _parameter.Attributes.Contain<TexCoordsAttribute>(Resolver);
			var color = _parameter.Attributes.Contain<ColorAttribute>(Resolver);

			var semanticsCount = new[] { position, normal, color, texCoords }.Count(attribute => attribute);
			if (semanticsCount > 1)
				Error(_parameter, "Unexpected declaration of multiple semantics attributes.");
			if (semanticsCount == 0)
				Error(_parameter, "Expected declaration of a semantics attribute.");

			// Check whether the semantic index is out of range
			var attributes = from attributeSection in _parameter.Attributes
							 from attribute in attributeSection.Attributes
							 let index = GetSemanticIndex(attribute)
							 where index < 0 || index > 3
							 select attribute;

			foreach (var attribute in attributes)
				Error(attribute, "Semantic index is out of range or could not be determined.");
		}

		/// <summary>
		///   Gets the semantic index of the given attribute.
		/// </summary>
		/// <param name="attribute">The attribute whose semantic index specification should be returned.</param>
		private int GetSemanticIndex(Attribute attribute)
		{
			if (!attribute.HasArgumentList)
				return 0;

			var resolved = Resolver.Resolve(attribute.Arguments.Single());
			if (resolved.ConstantValue == null)
				return -1;

			return (int)resolved.ConstantValue;
		}
	}
}