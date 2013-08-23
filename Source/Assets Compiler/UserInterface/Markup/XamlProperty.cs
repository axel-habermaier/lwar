using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System.Linq;
	using System.Reflection;
	using System.Xml.Linq;
	using Platform.Logging;

	/// <summary>
	///   Represents a Xaml property setter.
	/// </summary>
	internal class XamlProperty
	{
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

			IsValid = !xamlAttribute.IsNamespaceDeclaration && !xamlFile.ShouldBeIgnored(xamlAttribute);
			if (!IsValid)
				return;

			Name = xamlAttribute.Name.LocalName;
			Initialize(classType);
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
			var isContentProperty = dotIndex == -1;

			if (isContentProperty)
				Name = GetContentPropertyName(classType);
			else
				Name = elementName.Substring(dotIndex + 1);

			Initialize(classType);
		}

		/// <summary>
		///   Gets a value indicating whether the property type is a list type.
		/// </summary>
		public bool IsList { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the property type is a dictionary type.
		/// </summary>
		public bool IsDictionary { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the instance is valid.
		/// </summary>
		public bool IsValid { get; private set; }

		/// <summary>
		///   Gets the name of the property.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the CLR type name of the property.
		/// </summary>
		public string Type { get; private set; }

		/// <summary>
		///   Gets the name of the content property.
		/// </summary>
		/// <param name="classType">The CLR type of the class the content property name should be returned for.</param>
		private static string GetContentPropertyName(Type classType)
		{
			var contentProperty = classType.GetCustomAttributes(typeof(ContentPropertyAttribute))
										   .OfType<ContentPropertyAttribute>()
										   .SingleOrDefault();

			if (contentProperty == null)
				Log.Die("Unable to determine the name of the content property of class '{0}'.", classType.FullName);
			
			return contentProperty.Name;
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
		}
	}
}