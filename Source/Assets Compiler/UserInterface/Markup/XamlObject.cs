namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System;
	using System.Linq;
	using System.Xml.Linq;
	using CodeGeneration;
	using Platform.Logging;

	/// <summary>
	///   Represents an object instantiation in a Xaml file.
	/// </summary>
	internal class XamlObject : XamlElement
	{
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
			Properties = normalized.Attributes().Select(attribute => new XamlProperty(xamlFile, Type, attribute))
								   .Union(normalized.Elements().Select(element => new XamlProperty(xamlFile, Type, element)))
								   .Where(property => property.IsValid)
								   .ToArray();

			// Get the name of the object
			if (isRoot)
				Name = "this";
			else
			{
				var nameProperty = Properties.SingleOrDefault(p => p.Name == "Name");
				if (nameProperty == null)
					Name = xamlFile.GenerateUniqueName(Type);
				else
					Name = (string)((XamlValue)nameProperty.Value).Value;
			}
		}

		/// <summary>
		///   Gets the Xaml properties of the Xaml object.
		/// </summary>
		public XamlProperty[] Properties { get; private set; }

		/// <summary>
		///   Generates the code for the Xaml object.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="assignmentFormat">The target the generated object should be assigned to.</param>
		public override void GenerateCode(CodeWriter writer, string assignmentFormat)
		{
			Assert.ArgumentNotNull(writer);

			writer.Newline();

			if (typeof(ICodeGenerator).IsAssignableFrom(Type))
			{
				var generator = (ICodeGenerator)Activator.CreateInstance(Type);
				generator.GenerateCode(this, writer, assignmentFormat);
			}
			else
			{
				if (!IsRoot)
					writer.AppendLine("var {0} = new {1}.{2}();", Name, GetRuntimeNamespace(), Type.Name);

				foreach (var property in Properties)
					property.GenerateCode(writer, Name);

				writer.AppendLine(assignmentFormat, Name);
			}
		}

		/// <summary>
		///   Normalizes the tree by adding a property element for the object's content property.
		/// </summary>
		/// <param name="xamlElement">The element that should be normalized.</param>
		private XElement Normalize(XElement xamlElement)
		{
			xamlElement = NormalizeContentProperty(xamlElement);

			if (typeof(IRequiresNormalization).IsAssignableFrom(Type))
			{
				var normalization = (IRequiresNormalization)Activator.CreateInstance(Type);
				normalization.Normalize(xamlElement);
			}

			return xamlElement;
		}

		/// <summary>
		///   Normalizes the tree by adding a property element for the object's content property.
		/// </summary>
		/// <param name="xamlElement">The element that should be normalized.</param>
		private XElement NormalizeContentProperty(XElement xamlElement)
		{
			var contentProperty = Type.GetCustomAttribute<ContentPropertyAttribute>();
			if (contentProperty == null)
				return xamlElement;

			var attributes = xamlElement.Attributes();
			var propertyElements = xamlElement.Elements().Where(element => element.Name.LocalName.Contains("."));
			var contentPropertyElements = xamlElement.Elements().Where(element => !element.Name.LocalName.Contains("."));

			if ((xamlElement.Elements().Any() || String.IsNullOrWhiteSpace(xamlElement.Value)) && !contentPropertyElements.Any())
				return xamlElement;

			if (!contentPropertyElements.Any())
				contentPropertyElements = new ArraySegment<XElement>(new[]
				{
					new XElement(XamlFile.DefaultNamespace + "TextBlock", new XAttribute("Text", xamlElement.Value))
				});

			return new XElement(xamlElement.Name, attributes, propertyElements,
								new XElement(xamlElement.Name + "." + contentProperty.Name, contentPropertyElements));
		}
	}
}