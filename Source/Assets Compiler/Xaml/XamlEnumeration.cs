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
	///     Provides type information about all types that can be referenced in a Xaml file.
	/// </summary>
	internal class XamlTypeInfoProvider
	{
		/// <summary>
		///     The files that provide the type information.
		/// </summary>
		private readonly List<XamlTypeInfoFile> _files = new List<XamlTypeInfoFile>();

		/// <summary>
		///     The primitive Xaml types.
		/// </summary>
		private readonly XamlPrimitiveType[] _primitiveTypes =
		{
			new XamlPrimitiveType("object"),
			new XamlPrimitiveType("bool"),
			new XamlPrimitiveType("char"),
			new XamlPrimitiveType("byte"),
			new XamlPrimitiveType("sbyte"),
			new XamlPrimitiveType("short"),
			new XamlPrimitiveType("ushort"),
			new XamlPrimitiveType("int"),
			new XamlPrimitiveType("uint"),
			new XamlPrimitiveType("long"),
			new XamlPrimitiveType("ulong"),
			new XamlPrimitiveType("single"),
			new XamlPrimitiveType("double"),
			new XamlPrimitiveType("string")
		};

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="typeInfoFile">The path to the type info file that stores the type information.</param>
		public XamlTypeInfoProvider(string typeInfoFile)
		{
			Assert.ArgumentNotNullOrWhitespace(typeInfoFile);

			LoadRecursive(typeInfoFile);
			EnsureDistinct();
			ResolveTypes();
		}

		/// <summary>
		///     Gets all defined types.
		/// </summary>
		public IEnumerable<IXamlType> AllTypes
		{
			get { return _files.SelectMany(f => f.Types).Cast<IXamlType>().Union(_primitiveTypes); }
		}

		/// <summary>
		///     Recursively loads the given file and all of its includes.
		/// </summary>
		/// <param name="fileName">The file that should be loaded.</param>
		private void LoadRecursive(string fileName)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);

			var file = new XamlTypeInfoFile(fileName);
			_files.Add(file);

			foreach (var includedFile in file.IncludedFiles)
				LoadRecursive(Path.Combine(Configuration.SourceDirectory, includedFile));
		}

		/// <summary>
		///     Ensures that all loaded type names are distinct.
		/// </summary>
		private void EnsureDistinct()
		{
			var groups = _files.SelectMany(f => f.Types)
							   .GroupBy(t => t.FullName)
							   .Where(g => g.Count() > 1);

			foreach (var group in groups)
				group.First().File.Report(LogType.Fatal, group.First().Element,
										  "Type information for type '{0}' has been specified more than once.", group.Key);
		}

		/// <summary>
		///     Resolves the base types of all classes as well as all property types.
		/// </summary>
		private void ResolveTypes()
		{
			foreach (var file in _files)
			{
				foreach (var xamlClass in file.Classes)
				{
					if (xamlClass.HasParent)
					{
						IXamlType parent;
						if (!TryFind(xamlClass.ParentName, out parent))
							xamlClass.File.Report(LogType.Fatal, xamlClass.Element, "Unknown parent type.");
						else
						{
							var parentClass = parent as XamlClass;
							if (parentClass == null)
								xamlClass.File.Report(LogType.Fatal, xamlClass.Element, "Invalid parent type.");

							xamlClass.Parent = parentClass;
						}
					}

					foreach (var property in xamlClass.Properties)
					{
						IXamlType type;
						if (!TryFind(property.TypeName, out type))
							xamlClass.File.Report(LogType.Fatal, property.Element, "Unknown property type.");
						else
							property.Type = type;
					}
				}
			}
		}

		/// <summary>
		///     Tries to find the type information for the type with the given full name.
		/// </summary>
		/// <param name="fullName">The full name of thetype.</param>
		/// <param name="type">Returns the type if one has been found.</param>
		public bool TryFind(string fullName, out IXamlType type)
		{
			type = AllTypes.SingleOrDefault(t => t.FullName == fullName);
			return type != null;
		}

		/// <summary>
		///     Tries to find the type information for the type with the given name.
		/// </summary>
		/// <param name="namespaceName">The namespace the type is defined in.</param>
		/// <param name="typeName">The name of thetype.</param>
		/// <param name="type">Returns the type if one has been found.</param>
		public bool TryFind(string namespaceName, string typeName, out IXamlType type)
		{
			type = AllTypes.SingleOrDefault(t => t.Namespace == namespaceName && t.Name == typeName);
			return type != null;
		}
	}

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

	/// <summary>
	///     Represents a type that can be referenced in a Xaml file.
	/// </summary>
	internal interface IXamlType
	{
		/// <summary>
		///     Gets the namespace the type belongs to.
		/// </summary>
		string Namespace { get; }

		/// <summary>
		///     Gets the name of the type.
		/// </summary>
		string Name { get; }

		/// <summary>
		///     Gets the full name of the type.
		/// </summary>
		string FullName { get; }
	}

	/// <summary>
	///     Represents a built-in primitive Xaml type such as int, string, or double.
	/// </summary>
	internal class XamlPrimitiveType : IXamlType
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Object" /> class.
		/// </summary>
		public XamlPrimitiveType(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Name = name;
		}

		/// <summary>
		///     Gets the namespace the type belongs to.
		/// </summary>
		public string Namespace
		{
			get { return ""; }
		}

		/// <summary>
		///     Gets the name of the type.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     Gets the full name of the type.
		/// </summary>
		public string FullName
		{
			get { return Name; }
		}
	}

	/// <summary>
	///     Stores information about a type that can be referenced in a Xaml file.
	/// </summary>
	internal abstract class XamlType : IXamlType
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="file">The file the xaml type was defined in.</param>
		/// <param name="element">The element that defined the type.</param>
		protected XamlType(XamlTypeInfoFile file, XElement element)
		{
			Assert.ArgumentNotNull(file);
			Assert.ArgumentNotNull(element);

			File = file;
			Element = element;
		}

		/// <summary>
		///     Gets the file the xaml type was defined in.
		/// </summary>
		public XamlTypeInfoFile File { get; private set; }

		/// <summary>
		///     Gets the element that defined the type.
		/// </summary>
		public XElement Element { get; private set; }

		/// <summary>
		///     Gets the namespace the type belongs to.
		/// </summary>
		public string Namespace { get; protected set; }

		/// <summary>
		///     Gets the name of the type.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///     Gets the full name of the type.
		/// </summary>
		public string FullName
		{
			get { return String.Format("{0}.{1}", Namespace, Name); }
			protected set
			{
				Assert.ArgumentNotNullOrWhitespace(value);
				var split = value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

				Name = split[split.Length - 1];
				Namespace = String.Join(".", split.Take(split.Length - 1));
			}
		}
	}

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