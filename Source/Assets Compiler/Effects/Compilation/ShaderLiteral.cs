using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a field of an effect class that acts as compile-time constant literal shader value.
	/// </summary>
	internal class ShaderLiteral
	{
		/// <summary>
		///   The declaration of the field that represents the literal.
		/// </summary>
		private readonly FieldDeclaration _field;

		/// <summary>
		///   The declaration of the field variable that represents the literal.
		/// </summary>
		private readonly VariableInitializer _variable;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the literal.</param>
		/// <param name="variable">The declaration of the field variable that represents the literal.</param>
		public ShaderLiteral(FieldDeclaration field, VariableInitializer variable)
		{
			Assert.ArgumentNotNull(field, () => field);
			Assert.ArgumentNotNull(variable, () => variable);

			_field = field;
			_variable = variable;
		}

		/// <summary>
		///   Gets the name of the shader literal.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the value of the literal.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		///   Gets the type of the literal.
		/// </summary>
		public DataType Type { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the literal is an array.
		/// </summary>
		public bool IsArray { get; private set; }

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} : {1} = {2}", Name, Type, Value);
		}

		/// <summary>
		///   Compiles the shader constant.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		public void Compile(CompilationContext context)
		{
			Name = _variable.Name;
			Type = _field.GetDataType(context);
			IsArray = _field.GetType(context).Kind == TypeKind.Array;

			if (Type == DataType.Unknown)
				context.Error(_variable,
							  "Shader literal '{0}' is declared with unknown or unsupported data type '{1}'.",
							  Name, _field.GetType(context).FullName);

			if (_field.Modifiers != (Modifiers.Private | Modifiers.Static | Modifiers.Readonly) &&
				_field.Modifiers != (Modifiers.Private | Modifiers.Static | Modifiers.Const))
				context.Error(_variable, "Shader literal '{0}' must be private, static, and either readonly or constant.", Name);

			if (_variable.Initializer.IsNull)
				context.Error(_variable.Initializer, "Shader literal '{0}' must be initialized.", Name);

			if (IsArray)
				Value = _variable.Initializer.GetConstantValues(context);
			else
				Value = _variable.Initializer.GetConstantValue<object>(context);

			if (Value == null)
				context.Error(_variable.Initializer, "Shader literal '{0}' must be initialized with a compile-time constant value.", Name);
		}
	}
}