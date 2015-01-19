namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Collections.Generic;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;
	using ICSharpCode.NRefactory.TypeSystem;
	using Utilities;

	/// <summary>
	///     Represents a field of an effect class that allows access to a texture or cubemap.
	/// </summary>
	internal class ShaderTexture : EffectElement
	{
		/// <summary>
		///     The declaration of the field that represents the texture object.
		/// </summary>
		private readonly FieldDeclaration _field;

		/// <summary>
		///     The variable that represents the texture object.
		/// </summary>
		private readonly VariableInitializer _variable;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the texture object.</param>
		/// <param name="variable">The declaration of the field variable that represents the texture object.</param>
		/// <param name="slot">The slot the texture object should be bound to.</param>
		public ShaderTexture(FieldDeclaration field, VariableInitializer variable, int slot)
		{
			Assert.ArgumentNotNull(field);
			Assert.ArgumentNotNull(variable);
			Assert.ArgumentInRange(slot, 0, 16);

			_field = field;
			_variable = variable;
			Slot = slot;
		}

		/// <summary>
		///     Gets the name of the texture object.
		/// </summary>
		public string Name
		{
			get { return _variable.Name; }
		}

		/// <summary>
		///     Gets the type of the texture object.
		/// </summary>
		public DataType Type
		{
			get { return _field.ResolveType(Resolver).ToDataType(); }
		}

		/// <summary>
		///     Gets the documentation of the effect.
		/// </summary>
		public IEnumerable<string> Documentation
		{
			get { return _field.GetDocumentation(); }
		}

		/// <summary>
		///     Gets the slot the texture object is bound to.
		/// </summary>
		public int Slot { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the shader texture is referenced in the given identifier expression.
		/// </summary>
		/// <param name="identifierExpression">The identifier expression that should be checked.</param>
		public bool IsReferenced(IdentifierExpression identifierExpression)
		{
			var resolvedAccess = Resolver.Resolve(identifierExpression) as MemberResolveResult;
			if (resolvedAccess == null)
				return false;

			var resolvedVariable = Resolver.Resolve(_variable) as MemberResolveResult;
			return resolvedAccess.Member.Equals(resolvedVariable.Member);
		}

		/// <summary>
		///     Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///     initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check whether the name is reserved
			ValidateIdentifier(_variable.NameToken);

			// Check whether the texture object is an array type
			if (_field.ResolveType(Resolver).Kind == TypeKind.Array)
				Error(_field, "Unexpected array declaration.");

			// Check whether the declared modifiers match the expected ones
			ValidateModifiers(_field, _field.ModifierTokens, new[] { Modifiers.Public, Modifiers.Readonly });

			// Check whether the texture object is initialized
			if (!_variable.Initializer.IsNull)
				Error(_variable.Initializer, "Unexpected initialization of shader texture object.");

			// Check whether the texture object should be part of a constant buffer
			if (_field.Attributes.Contain<ConstantAttribute>(Resolver))
				Error(_variable, "Shader texture object '{0}' cannot be part of a constant buffer.", Name);
		}
	}
}