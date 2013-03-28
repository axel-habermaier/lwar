using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a field of an effect class that acts as compile-time constant literal shader value.
	/// </summary>
	internal class ShaderLiteral : ShaderDataObject
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the literal.</param>
		/// <param name="variable">The declaration of the field variable that represents the literal.</param>
		public ShaderLiteral(FieldDeclaration field, VariableInitializer variable)
			: base(field, variable)
		{
		}

		/// <summary>
		///   Gets the value of the literal.
		/// </summary>
		public object Value { get; private set; }

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
			Name = Variable.Name;
			Type = Field.GetDataType(context);
			IsArray = Field.GetType(context).Kind == TypeKind.Array;

			if (Type == DataType.Unknown)
				context.Error(Variable,
							  "Shader literal '{0}' is declared with unknown or unsupported data type '{1}'.",
							  Name, Field.GetType(context).FullName);

			if (Field.Modifiers != (Modifiers.Private | Modifiers.Static | Modifiers.Readonly) &&
				Field.Modifiers != (Modifiers.Private | Modifiers.Static | Modifiers.Const))
				context.Error(Variable, "Shader literal '{0}' must be private, static, and either readonly or constant.", Name);

			if (Variable.Initializer.IsNull)
				context.Error(Variable.Initializer, "Shader literal '{0}' must be initialized.", Name);

			if (IsArray)
				Value = Variable.Initializer.GetConstantValues(context);
			else
				Value = Variable.Initializer.GetConstantValue<object>(context);

			if (Value == null)
				context.Error(Variable.Initializer, "Shader literal '{0}' must be initialized with a compile-time constant value.", Name);
		}
	}
}