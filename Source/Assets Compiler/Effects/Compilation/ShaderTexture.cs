using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a field of an effect class that allows access to a texture or cubemap.
	/// </summary>
	internal class ShaderTexture : ShaderDataObject<FieldDeclaration>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="declaration">The declaration of the field that represents the texture.</param>
		/// <param name="variable">The declaration of the field variable that represents the texture.</param>
		/// <param name="slot">The slot the texture object should be bound to.</param>
		public ShaderTexture(FieldDeclaration declaration, VariableInitializer variable, int slot)
			: base(declaration, variable)
		{
			Assert.ArgumentInRange(slot, () => slot, 0, 16);
			Slot = slot;
		}

		/// <summary>
		///   Gets or sets the slot the constant is bound to.
		/// </summary>
		public int Slot { get; set; }

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
			Name = Variable.Name;
			context.ValidateIdentifier(Variable.NameToken);
			Type = Declaration.GetDataType(context);

			if (Declaration.GetType(context).Kind == TypeKind.Array)
				context.Error(Variable, "Shader texture object '{0}' cannot be an array type.", Name);

			if (Declaration.Modifiers != (Modifiers.Public | Modifiers.Readonly))
				context.Error(Variable, "Shader texture object '{0}' must be public, non-static, and readonly.", Name);

			if (!Variable.Initializer.IsNull)
				context.Error(Variable.Initializer, "Shader texture object '{0}' cannot be initialized.", Name);

			if (Declaration.HasAttribute<ShaderConstantAttribute>(context))
				context.Error(Variable, "Shader texture object '{0}' cannot be part of a constant buffer.", Name);
		}
	}
}