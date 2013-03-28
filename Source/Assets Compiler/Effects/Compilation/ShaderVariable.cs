using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a local variable of a shader.
	/// </summary>
	internal class ShaderVariable : ShaderDataObject<VariableDeclarationStatement>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="declaration">The declaration of the field that represents the shader data object.</param>
		/// <param name="variable">The declaration of the field variable that represents the shader data object.</param>
		public ShaderVariable(VariableDeclarationStatement declaration, VariableInitializer variable)
			: base(declaration, variable)
		{
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} : {1}", Name, Type);
		}

		/// <summary>
		///   Compiles the shader variable.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		public void Compile(CompilationContext context)
		{
			Name = Variable.Name;
			context.ValidateIdentifier(Variable.NameToken);

			var type = context.Resolve(Variable).Type;
			Type = type.ToDataType();

			if (type.Kind == TypeKind.Array)
				context.Error(Variable, "Local variable '{0}' cannot be an array.", Name);

			if (Type == DataType.Unknown)
				context.Error(Variable, "Local variable '{0}' is declared with unknown or unsupported data type '{1}'.",
							  Name, type.FullName);
		}

		/// <summary>
		///   Checks whether this variable and the given variable are the same.
		/// </summary>
		/// <param name="variable">The variable that should be checked.</param>
		public bool IsSame(IVariable variable)
		{
			var region = Variable.NameToken.Region;
			return region.Begin == variable.Region.Begin && region.End == variable.Region.End;
		}
	}
}