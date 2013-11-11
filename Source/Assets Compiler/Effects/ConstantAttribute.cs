namespace Pegasus.AssetsCompiler.Effects
{
	using System;

	/// <summary>
	///   When applied to a field, indicates that the field is part of a constant buffer. All fields that have the attribute
	///   applied with the same constant buffer name are placed in the same buffer. The order of the fields in the declaring
	///   class defines the order of the fields in the constant buffer. If no constant buffer name is specified, a default name
	///   is chosen.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ConstantAttribute : Attribute
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="constantBuffer">The name of the constant buffer the constant should be placed in.</param>
		public ConstantAttribute(string constantBuffer = "")
		{
		}
	}
}