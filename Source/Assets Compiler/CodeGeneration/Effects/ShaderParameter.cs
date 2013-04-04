using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Effects
{
	using System.Linq;
	using AssetsCompiler.Effects;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a parameter of a shader.
	/// </summary>
	internal class ShaderParameter : EffectElement
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
				var semantics = _parameter.GetSemanticsAttributes(Resolver).FirstOrDefault();
				if (semantics != null)
					return semantics.Semantics;

				// If no semantics attribute has been specified, just return some meaningless default semantics; 
				// this error will be caught during the validation of the parameter.
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

			// Check whether the parameter is declared with a valid type
			var types = new[] { DataType.Float, DataType.Vector2, DataType.Vector3, DataType.Vector4 };
			if (!types.Contains(Type))
				Error(_parameter.Type, "Unexpected data type.");

			// Check whether the parameter has type Vector4 if it has the color semantics
			if (Semantics.IsColor() && Type != DataType.Vector4)
				Error(_parameter, "Parameters with the 'Color' semantics must be of type '{0}'.", typeof(Vector4).FullName);

			// Check whether the parameter is an array type
			if (_parameter.ResolveType(Resolver).Kind == TypeKind.Array)
				Error(_parameter.Type, "Unexpected array declaration.");

			// Check whether the parameter is declared with modifier 'out' or no modifier at all
			if (_parameter.ParameterModifier != ParameterModifier.Out && _parameter.ParameterModifier != ParameterModifier.None)
				Error(_parameter, "Unexpected modifier '{0}'.", _parameter.ParameterModifier.ToString().ToLower());

			// Check whether the parameter is declared with any semantics or with multiple semantics
			var semanticsCount = _parameter.GetSemantics(Resolver).Count();
			if (semanticsCount > 1)
				Error(_parameter, "Unexpected declaration of multiple semantics attributes.");
			if (semanticsCount == 0)
				Error(_parameter, "Expected declaration of a semantics attribute.");

			// Check whether the semantic index is out of range
			var invalidSemantics = from attribute in _parameter.GetSemantics(Resolver)
								   let semantics = attribute.ToSemanticsAttribute(Resolver)
								   where semantics.Index < 0 || semantics.Index > SemanticsAttribute.MaximumIndex
								   select attribute;

			foreach (var attribute in invalidSemantics)
				Error(attribute.Arguments.First(), "Semantic index is out of range.");
		}
	}
}