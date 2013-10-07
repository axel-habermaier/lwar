namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System;
	using System.Reflection;
	using CodeGeneration;
	using TypeConverters;

	/// <summary>
	///   Represents a Xaml value.
	/// </summary>
	internal class XamlValue : XamlElement
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml value.</param>
		/// <param name="valueType">The type of the value.</param>
		/// <param name="value">The string representation of the value.</param>
		public XamlValue(XamlFile xamlFile, Type valueType, string value)
			: base(false)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(valueType);
			Assert.ArgumentNotNull(value);

			// Check if the value contains type information and extract it
			if (valueType == typeof(object) && value.StartsWith("[typeof("))
			{
				var valueStart = value.IndexOf("]", StringComparison.Ordinal) + 1;
				var propertyStart = value.IndexOf("(", StringComparison.Ordinal) + 1;
				var propertyEnd = value.IndexOf(")", StringComparison.Ordinal);

				var propertyName = value.Substring(propertyStart, propertyEnd - propertyStart);
				var propertyInfo = (DependencyPropertyReference)TypeConverter.Convert(xamlFile, typeof(DependencyPropertyReference), propertyName);

				Type = propertyInfo.PropertyType;
				value = value.Substring(valueStart);
			}
			else
				Type = valueType;

			Name = xamlFile.GenerateUniqueName(Type);
			Value = TypeConverter.Convert(xamlFile, Type, value);
		}

		/// <summary>
		///   Gets the value that was specified in Xaml.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		///   Generates the code for the Xaml object.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="assignmentFormat">The target the generated object should be assigned to.</param>
		public override void GenerateCode(CodeWriter writer, string assignmentFormat)
		{
			Assert.ArgumentNotNull(writer);
			writer.Append(assignmentFormat, TypeConverter.GenerateCode(Value));
		}
	}
}