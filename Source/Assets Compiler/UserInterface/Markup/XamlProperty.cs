namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System;
	using System.Collections;
	using System.Linq;
	using System.Reflection;
	using System.Xml.Linq;
	using CodeGeneration;
	using Platform.Logging;

	/// <summary>
	///   Represents a Xaml property setter.
	/// </summary>
	internal class XamlProperty
	{
		/// <summary>
		///   Indicates whether the property should be ignored at runtime.
		/// </summary>
		private bool _ignoreAtRuntime;

		/// <summary>
		///   Initializes a new instance from a Xaml property attribute.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml attribute.</param>
		/// <param name="classType">The CLR type of the class that defines the Xaml property.</param>
		/// <param name="xamlAttribute">The Xaml attribute this property should represent.</param>
		public XamlProperty(XamlFile xamlFile, Type classType, XAttribute xamlAttribute)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(xamlAttribute);

			IsValid = !xamlAttribute.IsNamespaceDeclaration && !xamlFile.ShouldBeIgnored(xamlAttribute) && !IgnoreAttribute(xamlAttribute);
			if (!IsValid)
				return;

			Name = xamlAttribute.Name.LocalName;
			Initialize(classType);

			Value = new XamlValue(xamlFile, Type, xamlAttribute.Value);
		}

		/// <summary>
		///   Initializes a new instance from a Xaml property element.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml element.</param>
		/// <param name="classType">The CLR type of the class that defines the Xaml property.</param>
		/// <param name="xamlElement">The Xaml element this property should represent.</param>
		public XamlProperty(XamlFile xamlFile, Type classType, XElement xamlElement)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(xamlElement);

			var elementName = xamlElement.Name.LocalName;
			var dotIndex = elementName.IndexOf('.');
			Name = elementName.Substring(dotIndex + 1);

			Initialize(classType);

			if (IsDictionary)
			{
				var firstElement = xamlElement.Elements().FirstOrDefault();

				XElement normalized;
				if (firstElement == null)
					normalized = new XElement(Type.Name);
				else if (xamlFile.GetClrType(firstElement) != Type)
					normalized = new XElement(Type.Name, xamlElement.Elements());
				else
					normalized = firstElement;

				Value = new XamlDictionary(xamlFile, normalized);
			}
			else if (IsList)
			{
				var firstElement = xamlElement.Elements().FirstOrDefault();

				XElement normalized;
				if (firstElement == null)
					normalized = new XElement(Type.Name);
				else if (xamlFile.GetClrType(firstElement) != Type)
					normalized = new XElement(Type.Name, xamlElement.Elements());
				else
					normalized = firstElement;

				Value = new XamlList(xamlFile, normalized);
			}
			else if (xamlElement.HasElements)
				Value = new XamlObject(xamlFile, xamlElement.Elements().First());
			else
				Value = new XamlValue(xamlFile, Type, xamlElement.Value);

			IsValid = true;
		}

		/// <summary>
		///   Gets the property's value.
		/// </summary>
		public XamlElement Value { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the instance is valid.
		/// </summary>
		public bool IsValid { get; private set; }

		/// <summary>
		///   Gets the name of the property.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the CLR type of the property.
		/// </summary>
		public Type Type { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the property's type is a list type.
		/// </summary>
		public bool IsList { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the property's type is a dictionary type.
		/// </summary>
		public bool IsDictionary { get; private set; }

		/// <summary>
		///   Checks whether the given attribute should be ignored.
		/// </summary>
		/// <param name="xamlAttribute">The attribute that should be checked.</param>
		private static bool IgnoreAttribute(XAttribute xamlAttribute)
		{
			return xamlAttribute.Name == XamlFile.MarkupNamespace + "Key";
		}

		/// <summary>
		///   Initializes the instance.
		/// </summary>
		/// <param name="classType">The CLR type of the class that defines the Xaml property.</param>
		private void Initialize(Type classType)
		{
			var propertyInfo = classType.GetProperty(Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			if (propertyInfo == null)
				Log.Die("Property '{0}.{1}' does not exist.", classType.FullName, Name);

			Type = propertyInfo.PropertyType;
			IsDictionary = typeof(IDictionary).IsAssignableFrom(Type);
			IsList = typeof(IList).IsAssignableFrom(Type) && !IsDictionary;

			_ignoreAtRuntime = propertyInfo.GetCustomAttributes(typeof(IgnoreAtRuntimeAttribute), true)
										   .OfType<IgnoreAtRuntimeAttribute>()
										   .SingleOrDefault() != null;
		}

		/// <summary>
		///   Generates the code for the Xaml property.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="objectName">The name of the object.</param>
		public void GenerateCode(CodeWriter writer, string objectName)
		{
			Assert.ArgumentNotNull(writer);
			Assert.ArgumentNotNullOrWhitespace(objectName);

			if (_ignoreAtRuntime)
				return;

			if (IsDictionary || IsList)
				Value.GenerateCode(writer, String.Format("{0}.{1}.{{0}};", objectName, Name));
			else
				Value.GenerateCode(writer, String.Format("{0}.{1} = {{0}};", objectName, Name));

			writer.Newline();
		}
	}
}