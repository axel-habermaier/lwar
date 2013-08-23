using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System.Xml.Linq;
	using CodeGeneration;
	using TypeConverters;

	/// <summary>
	///   Represents a Xaml value that is either a simple value, a list of values, or a Xaml object.
	/// </summary>
	internal abstract class XamlValue
	{
		/// <summary>
		///   Gets the actual value represented by this Xaml value.
		/// </summary>
		public abstract object Value { get; }

		/// <summary>
		///   Generates the code for the Xaml value.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="objectName">The name of the object on which the property should be set.</param>
		/// <param name="propertyName">The name of the property on which the value should be set.</param>
		public abstract void GenerateCode(CodeWriter writer, string objectName, string propertyName);

		/// <summary>
		///   Creates a Xaml value for the given Xaml attribute and value type.
		/// </summary>
		/// <param name="xamlAttribute">The Xaml attribute the Xaml value should be created for.</param>
		/// <param name="valueType">The type of the value that should be created.</param>
		public static XamlValue Create(XAttribute xamlAttribute, Type valueType)
		{
			Assert.ArgumentNotNull(xamlAttribute);
			Assert.ArgumentNotNull(valueType);

			return new ObjectValue(valueType, xamlAttribute.Value);
		}

		/// <summary>
		///   Creates a Xaml value for the given Xaml element and value type.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml value.</param>
		/// <param name="xamlElement">The Xaml element the Xaml value should be created for.</param>
		/// <param name="valueType">The type of the value that should be created.</param>
		public static XamlValue Create(XamlFile xamlFile, XElement xamlElement, Type valueType)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(xamlElement);
			Assert.ArgumentNotNull(valueType);

			if (!xamlElement.HasElements)
				return new ObjectValue(valueType, xamlElement.Value);

			return new XamlObjectValue(xamlFile, xamlElement);
		}

		/// <summary>
		///   Creates a Xaml value for the given Xaml element and value type, if the element represents a content property.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml value.</param>
		/// <param name="xamlElement">The Xaml element the Xaml value should be created for.</param>
		public static XamlValue Create(XamlFile xamlFile, XElement xamlElement)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(xamlElement);

			return new XamlObjectValue(xamlFile, xamlElement);
		}

		/// <summary>
		///   Represents an object value.
		/// </summary>
		private class ObjectValue : XamlValue
		{
			/// <summary>
			///   The value represented by this instance.
			/// </summary>
			private readonly object _value;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="valueType">The type of the value.</param>
			/// <param name="value">The string representation of the value.</param>
			public ObjectValue(Type valueType, string value)
			{
				Assert.ArgumentNotNull(valueType);
				Assert.ArgumentNotNull(value);

				_value = TypeConverter.Convert(valueType, value);
			}

			/// <summary>
			///   Gets the actual value represented by this Xaml value.
			/// </summary>
			public override object Value
			{
				get { return _value; }
			}

			/// <summary>
			///   Generates the code for the Xaml value.
			/// </summary>
			/// <param name="writer">The code writer that should be used to write the generated code.</param>
			/// <param name="objectName">The name of the object on which the property should be set.</param>
			/// <param name="propertyName">The name of the property on which the value should be set.</param>
			public override void GenerateCode(CodeWriter writer, string objectName, string propertyName)
			{
				Assert.ArgumentNotNull(writer);
				Assert.ArgumentNotNullOrWhitespace(objectName);
				Assert.ArgumentNotNullOrWhitespace(propertyName);

				writer.Append("{0}.{1} = ", objectName, propertyName);
				TypeConverter.GenerateCode(writer, _value);
				writer.AppendLine(";");
			}
		}

		/// <summary>
		///   Represents a Xaml object value.
		/// </summary>
		private class XamlObjectValue : XamlValue
		{
			/// <summary>
			///   The value represented by this instance.
			/// </summary>
			private readonly XamlObject _value;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="xamlFile">The Xaml file that defines the Xaml value.</param>
			/// <param name="xamlElement">The Xaml that represents the Xaml object.</param>
			public XamlObjectValue(XamlFile xamlFile, XElement xamlElement)
			{
				Assert.ArgumentNotNull(xamlElement);
				_value = new XamlObject(xamlFile, xamlElement);
			}

			/// <summary>
			///   Gets the actual value represented by this Xaml value.
			/// </summary>
			public override object Value
			{
				get { return _value; }
			}

			/// <summary>
			///   Generates the code for the Xaml value.
			/// </summary>
			/// <param name="writer">The code writer that should be used to write the generated code.</param>
			/// <param name="objectName">The name of the object on which the property should be set.</param>
			/// <param name="propertyName">The name of the property on which the value should be set.</param>
			public override void GenerateCode(CodeWriter writer, string objectName, string propertyName)
			{
				Assert.ArgumentNotNull(writer);
				Assert.ArgumentNotNullOrWhitespace(objectName);
				Assert.ArgumentNotNullOrWhitespace(propertyName);

				_value.GenerateCode(writer);
				writer.AppendLine("{0}.{1} = {2};", objectName, propertyName, _value.Name);
			}
		}
	}
}