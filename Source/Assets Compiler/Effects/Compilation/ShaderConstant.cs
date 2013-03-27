using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Reflection;
	using Framework;

	/// <summary>
	///   Represents a field of an effect class that acts as a shader constant.
	/// </summary>
	internal class ShaderConstant
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="field">The field of the effect class that represents the shader constant.</param>
		public ShaderConstant(FieldInfo field)
		{
			Assert.ArgumentNotNull(field, () => field);

			Name = field.Name;
			FullName = String.Format("{0}:{1}", field.DeclaringType.FullName, field.Name);
			Type = field.FieldType.GetTypeInfo();

			var attribute = field.GetCustomAttribute<ShaderConstantAttribute>();
			if (attribute != null)
			{
				ChangeFrequency = attribute.ChangeFrequency;
				IsConstantBufferMember = true;
			}
			else
				ChangeFrequency = ChangeFrequency.PerDrawCall;

			if (ChangeFrequency == ChangeFrequency.Unknown)
				Log.Error("'{0}' is not a valid value for the change frequency of shader constant '{1}'.", ChangeFrequency, FullName);

			if (field.IsStatic || field.IsLiteral)
				Value = field.GetValue(null);

			if (Type.IsArray && Value == null)
				Log.Error("A value must be assigned to shader constant '{0}'.", FullName);

			if (!field.IsLiteral && !field.IsInitOnly)
				Log.Error("Shader constant '{0}' must be constant or read-only.", FullName);
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the constant.</param>
		/// <param name="type">The type of the constant.</param>
		public ShaderConstant(string name, Type type)
		{
			Assert.ArgumentNotNullOrWhitespace(name, () => name);
			Assert.ArgumentNotNull(type, () => type);

			Name = name;
			FullName = name;
			Type = type.GetTypeInfo();
		}

		/// <summary>
		///   Gets the name of the shader constant.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the full name of the shader constant.
		/// </summary>
		public string FullName { get; private set; }

		/// <summary>
		///   Gets the value of the constant or null if no value is defined.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		///   Gets or sets the slot the constant is bound to.
		/// </summary>
		public int Slot { get; set; }

		/// <summary>
		///   Gets the type of the constant.
		/// </summary>
		public TypeInfo Type { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the constant is a 2D texture.
		/// </summary>
		public bool IsTexture2D
		{
			get { return Type == typeof(Texture2D); }
		}

		/// <summary>
		///   Gets a value indicating whether the constant is a cubemap
		/// </summary>
		public bool IsCubeMap
		{
			get { return Type == typeof(CubeMap); }
		}

		/// <summary>
		///   Gets the change frequency of the constant.
		/// </summary>
		public ChangeFrequency ChangeFrequency { get; private set; }

		/// <summary>
		///   Gets a value indicating that the constant should be a member of a constant buffer.
		/// </summary>
		public bool IsConstantBufferMember { get; private set; }

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return string.Format("Name: {0}, Type: {1}, Value: {2}", Name, Type, Value);
		}
	}
}