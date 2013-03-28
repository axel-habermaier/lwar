using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a field of an effect class that allows access to a texture or cubemap.
	/// </summary>
	internal class ShaderTexture
	{
		/// <summary>
		///   The declaration of the field that represents the texture.
		/// </summary>
		private readonly FieldDeclaration _field;

		/// <summary>
		///   The declaration of the field variable that represents the texture.
		/// </summary>
		private readonly VariableInitializer _variable;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the texture.</param>
		/// <param name="variable">The declaration of the field variable that represents the texture.</param>
		/// <param name="slot">The slot the texture object should be bound to.</param>
		public ShaderTexture(FieldDeclaration field, VariableInitializer variable, int slot)
		{
			Assert.ArgumentNotNull(field, () => field);
			Assert.ArgumentNotNull(variable, () => variable);
			Assert.ArgumentInRange(slot, () => slot, 0, 16);

			_field = field;
			_variable = variable;
			Slot = slot;
		}

		/// <summary>
		///   Gets the name of the shader constant.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets or sets the slot the constant is bound to.
		/// </summary>
		public int Slot { get; set; }

		/// <summary>
		///   Gets the type of the texture.
		/// </summary>
		public DataType Type { get; private set; }

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} : {1} (Slot {2})", Name, Type, Slot);
		}

		/// <summary>
		///   Compiles the shader constant.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		public void Compile(CompilationContext context)
		{
			Name = _variable.Name;
			Type = _field.GetDataType(context);

			if (_field.GetType(context).Kind == TypeKind.Array)
				context.Error(_variable, "Shader texture object '{0}' cannot be an array type.", Name);

			if (_field.Modifiers != (Modifiers.Public | Modifiers.Readonly))
				context.Error(_variable, "Shader texture object '{0}' must be public, non-static, and readonly.", Name);

			if (!_variable.Initializer.IsNull)
				context.Error(_variable.Initializer, "Shader texture object '{0}' cannot be initialized.", Name);

			if (_field.HasAttribute<ShaderConstantAttribute>(context))
				context.Error(_variable, "Shader texture object '{0}' cannot be part of a constant buffer.", Name);
		}
	}
}