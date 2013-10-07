namespace Pegasus.AssetsCompiler.UserInterface.Markup.TypeConverters
{
	using System;
	using System.Reflection;
	using Platform.Logging;

	/// <summary>
	///   Converts strings to dependency property references.
	/// </summary>
	internal class DependencyPropertyReferenceConverter : TypeConverter
	{
		/// <summary>
		///   Gets the type the string value is converted to.
		/// </summary>
		protected override Type TargetType
		{
			get { return typeof(DependencyPropertyReference); }
		}

		/// <summary>
		///   Gets the runtime type for the given value.
		/// </summary>
		protected override string RuntimeType
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		///   Converts the given string value into an instance of the target type.
		/// </summary>
		/// <param name="xamlFile">The Xaml file the value is specified in.</param>
		/// <param name="value">The value that should be converted.</param>
		protected override object Convert(XamlFile xamlFile, string value)
		{
			var split = value.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
			if (split.Length != 2)
				Log.Die("Expected input '{0}' to be of form 'TypeName'.'PropertyName'.", value);

			var type = xamlFile.GetClrType(split[0]);
			var property = type.GetProperty(split[1], BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

			if (property == null)
				Log.Die("Property '{0}' could not be found.", value);

			return new DependencyPropertyReference(property);
		}

		/// <summary>
		///   Generates the code for the object value.
		/// </summary>
		/// <param name="value">The value the code should be generated for.</param>
		protected override string GenerateInstantiationCode(object value)
		{
			var propertyInfo = (DependencyPropertyReference)value;
			return String.Format("{0}.{1}", propertyInfo.RuntimeDeclaringType, propertyInfo.Name);
		}
	}
}