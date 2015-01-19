namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;
	using Utilities;

	/// <summary>
	///     Stores type information about an enumeration that can be referenced in a Xaml file.
	/// </summary>
	internal class XamlEnumeration : XamlType
	{
		/// <summary>
		///     The literals defined by the enumeration.
		/// </summary>
		private readonly string[] _literals;

		/// <summary>
		///     Initializes a new instances.
		/// </summary>
		/// <param name="file">The file that provides the information about the enumeration.</param>
		/// <param name="element">The Xml element describing the enumeration.</param>
		public XamlEnumeration(XamlTypeInfoFile file, XElement element)
			: base(file, element)
		{
			Assert.ArgumentNotNull(element);

			var name = element.Attribute("Name");
			var literals = element.Elements("Literal").ToArray();

			if (name == null || String.IsNullOrWhiteSpace(name.Value))
				file.Report(LogType.Fatal, element, "Invalid or missing enumeration name.");

			if (literals.Length == 0)
				file.Report(LogType.Fatal, element, "Enumeration does not define any literals.");

			foreach (var literal in literals.Where(l => String.IsNullOrWhiteSpace(l.Value)))
				file.Report(LogType.Fatal, literal, "Invalid literal.");

			FullName = name.Value.Trim();
			_literals = literals.Select(l => l.Value.Trim()).ToArray();

			name.Remove();
			foreach (var literal in literals)
				literal.Remove();

			file.ReportInvalidAttributes(element);
			file.ReportInvalidElements(element);
		}

		/// <summary>
		///     Gets the literals defined by the enumeration.
		/// </summary>
		public IEnumerable<string> Literals
		{
			get { return _literals; }
		}
	}
}