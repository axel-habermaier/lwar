using System;

namespace Pegasus.AssetsCompiler.Xaml
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;
	using Platform.Logging;

	/// <summary>
	///     Stores type information about a class that can be referenced in a Xaml file.
	/// </summary>
	internal class XamlClass : XamlType
	{
		/// <summary>
		///     The properties defined by the class.
		/// </summary>
		private readonly XamlProperty[] _properties;

		/// <summary>
		///     Initializes a new instances.
		/// </summary>
		/// <param name="file">The file that provides the information about the class.</param>
		/// <param name="element">The Xml element describing the class.</param>
		public XamlClass(XamlTypeInfoFile file, XElement element)
			: base(file, element)
		{
			Assert.ArgumentNotNull(element);

			var name = element.Attribute("Name");
			var parent = element.Attribute("Inherits");
			var isList = element.Attribute("IsList");
			var isDictionary = element.Attribute("IsDictionary");
			var properties = element.Elements("Property").ToArray();

			if (name == null || String.IsNullOrWhiteSpace(name.Value))
				file.Report(LogType.Fatal, element, "Invalid or missing class name.");

			FullName = name.Value.Trim();
			ParentName = parent != null ? parent.Value.Trim() : null;
			IsList = isList != null && isList.Value.Trim().ToLower() == "true";
			IsDictionary = isDictionary != null && isDictionary.Value.Trim().ToLower() == "true";
			_properties = properties.Select(p => new XamlProperty(file, p)).ToArray();

			name.Remove();

			if (parent != null)
				parent.Remove();

			if (isList != null)
				isList.Remove();

			if (isDictionary != null)
				isDictionary.Remove();

			foreach (var property in properties)
				property.Remove();

			file.ReportInvalidAttributes(element);
			file.ReportInvalidElements(element);
		}

		/// <summary>
		///     Gets the properties defined by the class.
		/// </summary>
		public IEnumerable<XamlProperty> Properties
		{
			get { return _properties; }
		}

		/// <summary>
		///     Gets a value indicating whether the type is a list type.
		/// </summary>
		public bool IsList { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the type is a dictionary type.
		/// </summary>
		public bool IsDictionary { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the class defines a content property.
		/// </summary>
		public bool HasContentProperty
		{
			get { return ContentProperty != null; }
		}

		/// <summary>
		///     Gets a value indicating whether the class is derived from a parent class other than System.Object.
		/// </summary>
		public bool HasParent
		{
			get { return !String.IsNullOrWhiteSpace(ParentName); }
		}

		/// <summary>
		///     Gets the full name of the parent class of the class, or null if the parent is System.Object.
		/// </summary>
		public string ParentName { get; private set; }

		/// <summary>
		///     Gets or sets the parent class of the class, or null if the parent is System.Object.
		/// </summary>
		public XamlClass Parent { get; set; }

		/// <summary>
		///     Gets the content property of the class, or null if it doesn't have one.
		/// </summary>
		public XamlProperty ContentProperty
		{
			get
			{
				Assert.That(_properties.Count(p => p.IsContentProperty) <= 1, "Class '{0}' defines more than one content property.", Name);
				return _properties.Single(p => p.IsContentProperty);
			}
		}
	}
}