﻿using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Effects
{
	using System.Linq;
	using AssetsCompiler.Effects;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.TypeSystem;
	using ICSharpCode.NRefactory.TypeSystem.Implementation;

	/// <summary>
	///   Represents a field of an effect class that acts as compile-time constant literal shader value.
	/// </summary>
	internal class ShaderLiteral : EffectElement
	{
		/// <summary>
		///   The declaration of the field that represents the literal.
		/// </summary>
		private readonly FieldDeclaration _field;

		/// <summary>
		///   The variable that represents the literal.
		/// </summary>
		private readonly VariableInitializer _variable;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the literal.</param>
		/// <param name="variable">The declaration of the field variable that represents the literal.</param>
		public ShaderLiteral(FieldDeclaration field, VariableInitializer variable)
		{
			Assert.ArgumentNotNull(field);
			Assert.ArgumentNotNull(variable);

			_field = field;
			_variable = variable;
		}

		/// <summary>
		///   Gets the name of the literal.
		/// </summary>
		public string Name
		{
			get { return _variable.Name; }
		}

		/// <summary>
		///   Gets the type of the literal.
		/// </summary>
		public DataType Type
		{
			get { return _field.ResolveType(Resolver).ToDataType(); }
		}

		/// <summary>
		///   Gets the value of the literal.
		/// </summary>
		public Expression Value
		{
			get { return _variable.Initializer; }
		}

		/// <summary>
		///   Checks whether the literal has a constant value.
		/// </summary>
		private bool HasConstantValue
		{
			get
			{
				if (IsConstructed)
					return true;

				if (IsArray)
					return _variable.Initializer.GetConstantValues(Resolver) != null;

				return _variable.Initializer.GetConstantValue(Resolver) != null;
			}
		}

		/// <summary>
		///   Gets a value indicating whether the literal's value is a call to a constructor of Vector2, Vector3, Vector4, or
		///   Matrix.
		/// </summary>
		public bool IsConstructed
		{
			get
			{
				var resolved = Resolver.Resolve(_variable.Initializer) as CSharpInvocationResolveResult;
				if (resolved == null)
					return false;

				var method = resolved.Member as DefaultResolvedMethod;
				if (method == null || !method.IsConstructor)
					return false;

				if (resolved.Type.FullName != typeof(Vector2).FullName &&
					resolved.Type.FullName != typeof(Vector3).FullName &&
					resolved.Type.FullName != typeof(Vector4).FullName &&
					resolved.Type.FullName != typeof(Matrix).FullName)
					return false;

				return resolved.Arguments.All(a => a.IsCompileTimeConstant);
			}
		}

		/// <summary>
		///   Gets a value indicating whether the literal is an array.
		/// </summary>
		public bool IsArray
		{
			get { return _field.ResolveType(Resolver).Kind == TypeKind.Array; }
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check whether the name is reserved
			ValidateIdentifier(_variable.NameToken);

			// Check whether the literal is declared with a known type
			ValidateType(_field, _field.ResolveType(Resolver));

			// Check whether the declared modifiers match the expected ones
			var constModifiers = new[] { Modifiers.Private, Modifiers.Const };
			var readonlyModifiers = new[] { Modifiers.Private, Modifiers.Static, Modifiers.Readonly };
			ValidateModifiers(_field, _field.ModifierTokens, new[] { constModifiers, readonlyModifiers });

			// Check whether the literal is initialized
			if (_variable.Initializer.IsNull)
				Error(_variable, "Shader literal '{0}' must be initialized.", Name);

			// Check whether the literal is initialized with a compile-time constant
			if (!_variable.Initializer.IsNull && !HasConstantValue)
				Error(_variable.Initializer, "Shader literal '{0}' must be initialized with a non-null compile-time constant value.",
					  Name);
		}
	}
}