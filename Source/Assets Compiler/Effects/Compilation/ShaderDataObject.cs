using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a data object that can be read by a shader.
	/// </summary>
	internal abstract class ShaderDataObject
	{
		/// <summary>
		///   The declaration of the field that represents the shader data object.
		/// </summary>
		protected readonly FieldDeclaration Field;

		/// <summary>
		///   The declaration of the field variable that represents the shader data object.
		/// </summary>
		protected readonly VariableInitializer Variable;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the shader data object.</param>
		/// <param name="variable">The declaration of the field variable that represents the shader data object.</param>
		protected ShaderDataObject(FieldDeclaration field, VariableInitializer variable)
		{
			Assert.ArgumentNotNull(field, () => field);
			Assert.ArgumentNotNull(variable, () => variable);

			Field = field;
			Variable = variable;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected ShaderDataObject()
		{
		}

		/// <summary>
		///   Gets the name of the shader constant.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///   Gets the type of the texture.
		/// </summary>
		public DataType Type { get; protected set; }

		/// <summary>
		///   Gets a value indicating whether the literal is an array.
		/// </summary>
		public bool IsArray { get; protected set; }
	}
}