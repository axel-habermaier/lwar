using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Linq;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a field of an effect class that is part of a constant buffer.
	/// </summary>
	internal class ShaderConstant
	{
		/// <summary>
		///   The declaration of the field that represents the constant.
		/// </summary>
		private readonly FieldDeclaration _field;

		/// <summary>
		///   The declaration of the field variable that represents the constant.
		/// </summary>
		private readonly VariableInitializer _variable;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the constant.</param>
		/// <param name="variable">The declaration of the field variable that represents the constant.</param>
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
		}

		/// <summary>
		///   Gets the name of the shader constant.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the type of the constant.
		/// </summary>
		public DataType Type { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the literal is an array.
		/// </summary>
		public bool IsArray { get; private set; }

		/// <summary>
		///   Gets the change frequency of the constant.
		/// </summary>
		public ChangeFrequency ChangeFrequency { get; private set; }

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return string.Format("{0} : {1} [{2}]", Name, Type, ChangeFrequency);
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
							  "Shader constant '{0}' is declared with unknown or unsupported data type '{1}'.",
							  Name, _field.GetType(context).FullName);

			if (_field.Modifiers != (Modifiers.Public | Modifiers.Readonly))
				context.Error(_variable, "Shader constant '{0}' must be public, non-static, and readonly.", Name);

			if (!_variable.Initializer.IsNull)
				context.Error(_variable.Initializer, "Shader constant '{0}' cannot be initialized.", Name);

			var attribute = _field.GetAttribute<ShaderConstantAttribute>(context);
			var argument = attribute.Arguments.Single();
			ChangeFrequency = argument.GetConstantValue<ChangeFrequency>(context);

			if (ChangeFrequency == ChangeFrequency.Unknown)
				context.Error(_variable, "Change frequency of shader constant '{0}' must be specified.", Name);
		}
	}
}