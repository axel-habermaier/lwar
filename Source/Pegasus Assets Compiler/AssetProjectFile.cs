namespace Pegasus.AssetsCompiler
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Linq;
	using AssetCompilers;
	using Platform;
	using Platform.Logging;

	/// <summary>
	///     Represents a project file of an asset project.
	/// </summary>
	internal class AssetProjectFile
	{
		/// <summary>
		///     The path to the project file.
		/// </summary>
		private readonly string _projectFile;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="projectFile">The path to the project file that should be parsed.</param>
		public AssetProjectFile(string projectFile)
		{
			Assert.ArgumentNotNullOrWhitespace(projectFile);

			_projectFile = projectFile;
			Root = XElement.Parse(File.ReadAllText(projectFile), LoadOptions.SetLineInfo);

			CompilationContext = new CompilationContext(this);
			LoadReferencedAssemblies();

			ReportInvalidAttributes(Root);
			ReportInvalidElements(Root);
		}

		/// <summary>
		///     The dynamic asset compilers that are required to compile some of the assets of the asset project.
		/// </summary>
		public IAssetCompiler[] DynamicAssetCompilers { get; private set; }

		/// <summary>
		///     Gets the path to the directory containing the assets project file.
		/// </summary>
		public string Directory
		{
			get { return Path.GetDirectoryName(_projectFile); }
		}

		/// <summary>
		///     Gets the root element of the project file.
		/// </summary>
		public XElement Root { get; private set; }

		/// <summary>
		///     Gets the compilation context that provides asset project-specific configuration values to asset compilers.
		/// </summary>
		public CompilationContext CompilationContext { get; private set; }

		/// <summary>
		///     Loads all assemblies referenced by the assets project file into the process.
		/// </summary>
		private void LoadReferencedAssemblies()
		{
			var referencesElement = Root.Element("References");
			if (referencesElement == null)
				return;

			foreach (var assembly in referencesElement.Elements("Assembly"))
			{
				var fileAttribute = assembly.Attribute("File");
				if (fileAttribute == null || String.IsNullOrWhiteSpace(fileAttribute.Value))
					Report(LogType.Fatal, fileAttribute, "'Assembly' element is missing a value for attribute 'File'.");

				fileAttribute.Remove();
				Assembly.LoadFile(Path.Combine(CompilationContext.SourceDirectory, fileAttribute.Value));

				ReportInvalidAttributes(assembly);
				ReportInvalidElements(assembly);

				assembly.Remove();
			}

			ReportInvalidAttributes(referencesElement);
			ReportInvalidElements(referencesElement);

			referencesElement.Remove();
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
			location = String.Format("{0}{1}", _projectFile.Replace("/", "\\"), location);

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