using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System.Linq;
	using System.Reflection;
	using System.Xml.Linq;
	using CodeGeneration;
	using Mono.CSharp;
	using Platform.Logging;

	/// <summary>
	///   Represents an object instantiation in a Xaml file.
	/// </summary>
	internal class XamlObject : XamlElement
	{
		/// <summary>
		///   The Xaml properties of the Xaml object.
		/// </summary>
		private readonly XamlProperty[] _properties;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml element.</param>
		/// <param name="xamlElement">The Xaml element this object should represent.</param>
		/// <param name="isRoot">Indicates whether the Xaml object is the root object of a Xaml file.</param>
		public XamlObject(XamlFile xamlFile, XElement xamlElement, bool isRoot = false)
			: base(isRoot)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(xamlElement);

			// Get the type of the object
			Type = xamlFile.GetClrType(xamlElement);
			var normalized = Normalize(xamlElement);

			// Initialize all properties
			_properties = normalized.Attributes().Select(attribute => new XamlProperty(xamlFile, Type, attribute))
									.Union(normalized.Elements().Select(element => new XamlProperty(xamlFile, Type, element)))
									.Where(property => property.IsValid)
									.ToArray();

			// Get the name of the object
			if (isRoot)
				Name = "this";
			else
			{
				var nameProperty = _properties.SingleOrDefault(p => p.Name == "Name");
				if (nameProperty == null)
					Name = xamlFile.GenerateUniqueName(Type);
				else
					Name = (string)((XamlValue)nameProperty.Value).Value;
			}
		}

		/// <summary>
		///   Generates the code for the Xaml object.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="assignmentFormat">The target the generated object should be assigned to.</param>
		public override void GenerateCode(CodeWriter writer, string assignmentFormat)
		{
			Assert.ArgumentNotNull(writer);

			writer.Newline();
			if (!IsRoot)
				writer.AppendLine("var {0} = new {1}.{2}();", Name, GetRuntimeNamespace(), Type.Name);

			foreach (var property in _properties)
				property.GenerateCode(writer, Name);

			writer.AppendLine(assignmentFormat, Name);
		}

		/// <summary>
		///   Normalizes the tree by adding a property element for the object's content property.
		/// </summary>
		/// <param name="xamlElement">The element that should be normalized.</param>
		private XElement Normalize(XElement xamlElement)
		{
			var attributes = xamlElement.Attributes();
			var propertyElements = xamlElement.Elements().Where(element => element.Name.LocalName.Contains("."));
			var contentPropertyElements = xamlElement.Elements().Where(element => !element.Name.LocalName.Contains("."));

			if (!contentPropertyElements.Any())
				return xamlElement;

			var contentPropertyName = GetContentPropertyName();
			return new XElement(xamlElement.Name, attributes, propertyElements,
								new XElement(xamlElement.Name + "." + contentPropertyName, contentPropertyElements));
		}

		/// <summary>
		///   Gets the name of the content property.
		/// </summary>
		private string GetContentPropertyName()
		{
			var contentProperty = Type.GetCustomAttributes(typeof(ContentPropertyAttribute), true)
									  .OfType<ContentPropertyAttribute>()
									  .SingleOrDefault();

			if (contentProperty == null)
				Log.Die("Unable to determine the name of the content property of class '{0}'.", Type.FullName);

			return contentProperty.Name;
		}
	}
}