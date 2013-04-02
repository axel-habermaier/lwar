using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a field of an effect class that is part of a constant buffer.
	/// </summary>
	internal class ShaderConstant : CompiledElement
	{
		/// <summary>
		///   The declaration of the field that represents the shader constant.
		/// </summary>
		private readonly FieldDeclaration _field;

		/// <summary>
		///   The variable that represents the shader constant.
		/// </summary>
		private readonly VariableInitializer _variable;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the shader constant.</param>
		/// <param name="variable">The declaration of the field variable that represents the shader constant.</param>
		public ShaderConstant(FieldDeclaration field, VariableInitializer variable)
		{
			Assert.ArgumentNotNull(field, () => field);
			Assert.ArgumentNotNull(variable, () => variable);

			_field = field;
			_variable = variable;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the shader constant.</param>
		/// <param name="type">The type of the shader constant.</param>
		public ShaderConstant(string name, DataType type)
		{
			Assert.ArgumentNotNullOrWhitespace(name, () => name);
			Assert.ArgumentInRange(type, () => type);

			Name = name;
			Type = type;
			IsSpecial = true;
		}

		/// <summary>
		///   Gets the name of the shader constant.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the type of the shader constant.
		/// </summary>
		public DataType Type { get; private set; }

		/// <summary>
		///   Gets the documentation of the effect.
		/// </summary>
		public IEnumerable<string> Documentation
		{
			get { return _field.GetDocumentation(); }
		}

		/// <summary>
		///   Gets the name of the constant buffer the constant should be placed in. This property cannot be called if the constant
		///   is not user-defined.
		/// </summary>
		public string ConstantBufferName
		{
			get
			{
				var attribute = _field.Attributes.GetAttribute<ConstantAttribute>(Resolver);
				if (!attribute.Arguments.Any())
					return null;

				var argument = attribute.Arguments.Single();
				return (string)argument.GetConstantValue(Resolver);
			}
		}

		/// <summary>
		///   Gets a value indicating whether this constant is a special system-provided constant and not a user-defined one. If
		///   true is returned, the change frequency of the constant has no meaning.
		/// </summary>
		public bool IsSpecial { get; private set; }

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			if (IsSpecial)
				return;

			Name = _variable.Name;
			Type = _field.ResolveType(Resolver).ToDataType();
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			if (IsSpecial)
				return;

			// Check whether the name is reserved
			ValidateIdentifier(_variable.NameToken);

			// Check whether the constant is declared with a known type
			ValidateType(_field, _field.ResolveType(Resolver));

			// Check whether the constant is an array type
			if (_field.ResolveType(Resolver).Kind == TypeKind.Array)
				Error(_field, "Unexpected array declaration.");

			// Check whether the declared modifiers match the expected ones
			ValidateModifiers(_field, _field.ModifierTokens, new[] { Modifiers.Public, Modifiers.Readonly });

			// Check whether the constant is initialized
			if (!_variable.Initializer.IsNull)
				Error(_variable.Initializer, "Unexpected initialization of shader constant.");
		}
	}
}