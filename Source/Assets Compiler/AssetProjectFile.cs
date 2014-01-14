namespace Pegasus.AssetsCompiler
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Xml.Linq;
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
		}

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
	}
}