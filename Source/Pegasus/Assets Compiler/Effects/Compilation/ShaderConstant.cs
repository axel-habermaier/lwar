﻿namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;
	using ICSharpCode.NRefactory.TypeSystem;
	using Utilities;

	/// <summary>
	///     Represents a field of an effect class that is part of a constant buffer.
	/// </summary>
	internal class ShaderConstant : EffectElement
	{
		/// <summary>
		///     The declaration of the field that represents the shader constant.
		/// </summary>
		private readonly FieldDeclaration _field;

		/// <summary>
		///     The variable that represents the shader constant.
		/// </summary>
		private readonly VariableInitializer _variable;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the shader constant.</param>
		/// <param name="variable">The declaration of the field variable that represents the shader constant.</param>
		public ShaderConstant(FieldDeclaration field, VariableInitializer variable)
		{
			Assert.ArgumentNotNull(field);
			Assert.ArgumentNotNull(variable);

			_field = field;
			_variable = variable;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the shader constant.</param>
		/// <param name="type">The type of the shader constant.</param>
		public ShaderConstant(string name, DataType type)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.ArgumentInRange(type);

			Name = name;
			Type = type;
			IsSpecial = true;
		}

		/// <summary>
		///     Gets the name of the shader constant.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     Gets the type of the shader constant.
		/// </summary>
		public DataType Type { get; private set; }

		/// <summary>
		///     Gets the documentation of the effect.
		/// </summary>
		public IEnumerable<string> Documentation
		{
			get { return _field.GetDocumentation(); }
		}

		/// <summary>
		///     Gets the name of the constant buffer the constant should be placed in. This property cannot be called if the
		///     constant
		///     is not user-defined.
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
		///     Gets a value indicating whether this constant is a special system-provided constant and not a user-defined one.
		/// </summary>
		public bool IsSpecial { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the shader constant is an array.
		/// </summary>
		public bool IsArray { get; private set; }

		/// <summary>
		///     Gets the array length if the shader constant is an array. Returns 1 if the constant is not an array.
		/// </summary>
		public int ArrayLength
		{
			get
			{
				if (!IsArray)
					return 1;

				var attribute = _field.Attributes.GetAttribute<ArrayLengthAttribute>(Resolver);
				if (attribute == null)
					throw new InvalidOperationException("Unknown array size.");

				var resolved = Resolver.Resolve(attribute.Arguments.Single());
				return (int)resolved.ConstantValue;
			}
		}

		/// <summary>
		///     Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			if (IsSpecial)
				return;

			var type = _field.ResolveType(Resolver);

			Name = _variable.Name;
			Type = type.ToDataType();
			IsArray = type.Kind == TypeKind.Array;
		}

		/// <summary>
		///     Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///     initialization.
		/// </summary>
		protected override void Validate()
		{
			if (IsSpecial)
				return;

			// Check whether the name is reserved
			ValidateIdentifier(_variable.NameToken);

			// Check whether the constant is declared with a known type
			ValidateType(_field, _field.ResolveType(Resolver));

			// Check whether the constant is an array type but has no valid array length specified
			if (_field.ResolveType(Resolver).Kind == TypeKind.Array)
			{
				var attribute = _field.Attributes.GetAttribute<ArrayLengthAttribute>(Resolver);
				if (attribute == null)
					Error(_field, "The '{0}' attribute is required for shader constants of array type.", typeof(ArrayLengthAttribute).FullName);
			}

			// Check whether the declared modifiers match the expected ones
			ValidateModifiers(_field, _field.ModifierTokens, new[] { Modifiers.Public, Modifiers.Readonly });

			// Check whether the constant is initialized
			if (!_variable.Initializer.IsNull)
				Error(_variable.Initializer, "Unexpected initialization of shader constant.");
		}

		/// <summary>
		///     Gets a value indicating whether the shader constant is referenced in the given identifier expression.
		/// </summary>
		/// <param name="identifierExpression">The identifier expression that should be checked.</param>
		public bool IsReferenced(IdentifierExpression identifierExpression)
		{
			if (IsSpecial)
				return true;

			var resolvedAccess = Resolver.Resolve(identifierExpression) as MemberResolveResult;
			if (resolvedAccess == null)
				return false;

			var resolvedVariable = Resolver.Resolve(_variable) as MemberResolveResult;
			return resolvedAccess.Member.Equals(resolvedVariable.Member);
		}
	}
}