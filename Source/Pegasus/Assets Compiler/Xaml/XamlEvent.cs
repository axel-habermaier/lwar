namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Xml.Linq;
	using Platform.Logging;
	using Utilities;

	/// <summary>
	///     Stores information about a routed event of a class that can be referenced in a Xaml file.
	/// </summary>
	internal class XamlEvent
	{
		/// <summary>
		///     Initializes a new instances.
		/// </summary>
		/// <param name="file">The file that provides the information about the event.</param>
		/// <param name="element">The Xml element describing the event.</param>
		public XamlEvent(XamlTypeInfoFile file, XElement element)
		{
			Assert.ArgumentNotNull(element);

			var name = element.Attribute("Name");
			var type = element.Attribute("Type");

			if (name == null || String.IsNullOrWhiteSpace(name.Value))
				file.Report(LogType.Fatal, element, "Invalid or missing event name.");

			if (type == null || String.IsNullOrWhiteSpace(type.Value))
				file.Report(LogType.Fatal, element, "Invalid or missing event handler type.");

			Element = element;
			Name = name.Value.Trim();
			TypeName = type.Value.Trim();

			name.Remove();
			type.Remove();

			file.ReportInvalidAttributes(element);
			file.ReportInvalidElements(element);
		}

		/// <summary>
		///     Gets the element that defines the event.
		/// </summary>
		public XElement Element { get; private set; }

		/// <summary>
		///     Gets or sets the type of the event handler.
		/// </summary>
		public IXamlType Type { get; set; }

		/// <summary>
		///     Gets the full name of the type of the event handler.
		/// </summary>
		public string TypeName { get; private set; }

		/// <summary>
		///     Gets the name of the event.
		/// </summary>
		public string Name { get; private set; }
	}
}