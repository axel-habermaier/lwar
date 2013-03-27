using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Reflection;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a field of an effect class that allows access to a texture or cubemap.
	/// </summary>
	internal class ShaderTexture
	{
		/// <summary>
		///   The declaration of the field that represents the texture object.
		/// </summary>
		private FieldDeclaration _field;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the texture object.</param>
		public ShaderTexture(FieldDeclaration field)
		{
			Assert.ArgumentNotNull(field, () => field);
			_field = field;
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
		///   Gets a value indicating whether the constant is a 2D texture.
		/// </summary>
		public bool IsTexture2D { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the constant is a cubemap
		/// </summary>
		public bool IsCubeMap { get; private set; }

		

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return string.Format("Name: {0}, Type: {1}, Value: {2}", Name);
		}
	}
}