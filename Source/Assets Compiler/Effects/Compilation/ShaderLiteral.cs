using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a field of an effect class that acts as compile-time constant literal shader value.
	/// </summary>
	internal class ShaderLiteral
	{
		/// <summary>
		///   The declaration of the field that represents the literal.
		/// </summary>
		private FieldDeclaration _field;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the literal.</param>
		public ShaderLiteral(FieldDeclaration field)
		{
			Assert.ArgumentNotNull(field, () => field);
			_field = field;
		}

		///// <summary>
		/////   Initializes a new instance.
		///// </summary>
		///// <param name="field">The field of the effect class that represents the shader constant.</param>
		//public ShaderConstant(FieldInfo field)
		//{
		//	Assert.ArgumentNotNull(field, () => field);

		//	Name = field.Name;
		//	FullName = String.Format("{0}:{1}", field.DeclaringType.FullName, field.Name);
		//	Type = field.FieldType.GetTypeInfo();

		//	var attribute = field.GetCustomAttribute<ShaderConstantAttribute>();
		//	if (attribute != null)
		//	{
		//		ChangeFrequency = attribute.ChangeFrequency;
		//		IsConstantBufferMember = true;
		//	}
		//	else
		//		ChangeFrequency = ChangeFrequency.PerDrawCall;

		//	if (ChangeFrequency == ChangeFrequency.Unknown)
		//		Log.Error("'{0}' is not a valid value for the change frequency of shader constant '{1}'.", ChangeFrequency, FullName);

		//	if (field.IsStatic || field.IsLiteral)
		//		Value = field.GetValue(null);

		//	if (Type.IsArray && Value == null)
		//		Log.Error("A value must be assigned to shader constant '{0}'.", FullName);

		//	if (!field.IsLiteral && !field.IsInitOnly)
		//		Log.Error("Shader constant '{0}' must be constant or read-only.", FullName);
		//}

		/// <summary>
		///   Gets the name of the shader constant.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the value of the constant.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return string.Format("Name: {0}, Type: {1}, Value: {2}", Name, Value);
		}
	}
}