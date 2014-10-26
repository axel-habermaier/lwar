namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Xml.Linq;
	using Platform.Logging;
	using Utilities;

	/// <summary>
	///     Stores information about a property of a class that can be referenced in a Xaml file.
	/// </summary>
	internal class XamlProperty
	{
		/// <summary>
		///     Initializes a new instances.
		/// </summary>
		/// <param name="file">The file that provides the information about the property.</param>
		/// <param name="element">The Xml element describing the property.</param>
		public XamlProperty(XamlTypeInfoFile file, XElement element)
		{
			Assert.ArgumentNotNull(element);

			var name = element.Attribute("Name");
			var type = element.Attribute("Type");
			var isContentProperty = element.Attribute("IsContentProperty");

			if (name == null || String.IsNullOrWhiteSpace(name.Value))
				file.Report(LogType.Fatal, element, "Invalid or missing property name.");

			if (type == null || String.IsNullOrWhiteSpace(type.Value))
				file.Report(LogType.Fatal, element, "Invalid or missing property type.");

			Element = element;
			Name = name.Value.Trim();
			TypeName = type.Value.Trim();
			IsContentProperty = isContentProperty != null && isContentProperty.Value.Trim().ToLower() == "true";

			name.Remove();
			type.Remove();
			if (isContentProperty != null)
				isContentProperty.Remove();

			file.ReportInvalidAttributes(element);
			file.ReportInvalidElements(element);
		}

		/// <summary>
		///     Gets the element that defines the property.
		/// </summary>
		public XElement Element { get; private set; }

		/// <summary>
		///     Gets or sets the type of the property.
		/// </summary>
		public IXamlType Type { get; set; }

		/// <summary>
		///     Gets the full name of the type of the property.
		/// </summary>
		public string TypeName { get; private set; }

		/// <summary>
		///     Gets the name of the property.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the property is the default content property of the property's class.
		/// </summary>
		public bool IsContentProperty { get; private set; }
	}
}