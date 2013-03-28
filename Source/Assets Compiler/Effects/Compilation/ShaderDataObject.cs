using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a data object that can be read by a shader.
	/// </summary>
	internal abstract class ShaderDataObject<T>
		where T : AstNode
	{
		/// <summary>
		///   The declaration of the field that represents the shader data object.
		/// </summary>
		protected readonly T Declaration;

		/// <summary>
		///   The declaration of the field variable that represents the shader data object.
		/// </summary>
		protected readonly VariableInitializer Variable;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="declaration">The declaration of the field that represents the shader data object.</param>
		/// <param name="variable">The declaration of the field variable that represents the shader data object.</param>
		protected ShaderDataObject(T declaration, VariableInitializer variable)
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			Assert.ArgumentNotNull(variable, () => variable);

			Declaration = declaration;
			Variable = variable;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="declaration">The declaration of the field that represents the shader data object.</param>
		protected ShaderDataObject(T declaration)
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			Declaration = declaration;
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
	}
}