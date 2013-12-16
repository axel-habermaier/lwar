namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml;
	using System.Xml.Linq;
	using Platform;
	using Platform.Logging;

	/// <summary>
	///     Provides type information about types that can be referenced in a Xaml file.
	/// </summary>
	internal class XamlTypeInfoFile
	{
		/// <summary>
		///     The class types defined in the file.
		/// </summary>
		private readonly XamlClass[] _classes;

		/// <summary>
		///     The enumeration types defined in the file.
		/// </summary>
		private readonly XamlEnumeration[] _enumerations;

		/// <summary>
		///     The name of the file that provides the type information.
		/// </summary>
		private readonly string _fileName;

		/// <summary>
		///     The names of the files that the file includes.
		/// </summary>
		private readonly string[] _includedFiles;

		/// <summary>
		///     The root element of the file.
		/// </summary>
		private readonly XElement _root;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="fileName">The path to the file.</param>
		public XamlTypeInfoFile(string fileName)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);

			_fileName = fileName;
			_root = XElement.Parse(File.ReadAllText(fileName), LoadOptions.SetLineInfo);

			if (_root.Name != "Types")
				Report(LogType.Fatal, _root, "Invalid root element; expected 'Types'.");

			var includes = _root.Elements("Include").ToArray();
			var classes = _root.Elements("Class").ToArray();
			var enumerations = _root.Elements("Enumeration").ToArray();

			foreach (var include in includes.Where(i => String.IsNullOrWhiteSpace(i.Value)))
				Report(LogType.Fatal, include, "Invalid or missing include path.");

			_includedFiles = includes.Select(i => i.Value.Trim()).ToArray();
			_classes = classes.Select(c => new XamlClass(this, c)).ToArray();
			_enumerations = enumerations.Select(c => new XamlEnumeration(this, c)).ToArray();

			foreach (var element in includes.Union(classes).Union(enumerations))
				element.Remove();

			ReportInvalidAttributes(_root);
			ReportInvalidElements(_root);
		}

		/// <summary>
		///     Gets the enumeration types defined in the file.
		/// </summary>
		public IEnumerable<XamlEnumeration> Enumerations
		{
			get { return _enumerations; }
		}

		/// <summary>
		///     Gets the class types defined in the file.
		/// </summary>
		public IEnumerable<XamlClass> Classes
		{
			get { return _classes; }
		}

		/// <summary>
		///     Gets all types defined in the file.
		/// </summary>
		public IEnumerable<XamlType> Types
		{
			get { return _classes.Cast<XamlType>().Union(_enumerations); }
		}

		/// <summary>
		///     Gets the names of the files that the file includes.
		/// </summary>
		public IEnumerable<string> IncludedFiles
		{
			get { return _includedFiles; }
		}

		/// <summary>
		///     Reports an error, warning, or informational message about the contents of the file.
		/// </summary>
		/// <param name="type">The type of the message that should be reported.</param>
		/// <param name="lineInfo">The line and column that the report refers to.</param>
		/// <param name="formatMessage">The message that should be reported.</param>
		/// <param name="parameters">The parameters that should be inserted into the reported message.</param>
		[StringFormatMethod("formatMessage")]
		public void Report(LogType type, IXmlLineInfo lineInfo, string formatMessage, params object[] parameters)
		{
			Assert.ArgumentNotNull(lineInfo);
			Assert.ArgumentSatisfies(lineInfo.HasLineInfo(), "No line information has been provided.");
			Assert.ArgumentNotNullOrWhitespace(formatMessage);

			var location = String.Format("({0},{1})", lineInfo.LineNumber, lineInfo.LinePosition);
			location = String.Format("{0}{1}", _fileName.Replace("/", "\\"), location);

			var message = String.Format("{0}: {1}: {2}", location, type, String.Format(formatMessage, parameters));
			new LogEntry(type, message).RaiseLogEvent();
		}

		/// <summary>
		///     Reports all invalid attributes.
		/// </summary>
		/// <param name="element">The element whose invalid attributes should be reported.</param>
		public void ReportInvalidAttributes(XElement element)
		{
			foreach (var attribute in element.Attributes())
				Report(LogType.Warning, attribute, "Invalid attribute '{0}'.", attribute.Name.LocalName);
		}

		/// <summary>
		///     Reports all invalid elements.
		/// </summary>
		/// <param name="element">The element whose invalid elements should be reported.</param>
		public void ReportInvalidElements(XElement element)
		{
			foreach (var e in element.Elements())
				Report(LogType.Warning, e, "Invalid element '{0}'.", e.Name.LocalName);
		}
	}
}