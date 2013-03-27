using System;

namespace Pegasus.AssetsCompiler.Effects
{
	/// <summary>
	///   When applied to a field, indicates that the field is part of a constant buffer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ShaderConstantAttribute : Attribute
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="changeFrequency">
		///   Indicates how often the shader constant changes, allow the shader compiler to optimize
		///   the constant buffer layout.
		/// </param>
		public ShaderConstantAttribute(ChangeFrequency changeFrequency)
		{
			ChangeFrequency = changeFrequency;
		}

		/// <summary>
		///   Indicates how often the shader constant changes, allow the shader compiler to optimize the constant buffer layout.
		/// </summary>
		public ChangeFrequency ChangeFrequency { get; private set; }
	}
}