namespace Pegasus.AssetsCompiler
{
	using System;
	using System.IO;
	using System.Xml.Linq;
	using Platform.Logging;

	/// <summary>
	///     Provides asset project-specific configuration values to asset compilers.
	/// </summary>
	public class CompilationContext
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="projectFile">The assets project file the compilation context should be loaded from.</param>
		internal CompilationContext(AssetProjectFile projectFile)
		{
			Assert.ArgumentNotNull(projectFile);

			SourceDirectory = projectFile.Directory;

			var configurationElement = projectFile.Root.Element("Configuration");
			if (configurationElement == null)
				projectFile.Report(LogType.Fatal, projectFile.Root, "Configuration section is missing.");

			ProjectIdentifier = Load(projectFile, configurationElement, "Identifier", "Name");
			TempDirectory = Load(projectFile, configurationElement, "TempDirectory", "Name");
			TypeInfoFilePath = Load(projectFile, configurationElement, "TypeInfo", "File");

			EffectFilePath = Load(projectFile, configurationElement, "Effect");
			UserInterfaceFilePath = Load(projectFile, configurationElement, "UserInterface");
			FontLoaderFilePath = Load(projectFile, configurationElement, "FontLoader");
			AssetIdentifierFilePath = Load(projectFile, configurationElement, "AssetIdentifier");
			RegistryFilePath = Load(projectFile, configurationElement, "Registry");

			TempDirectory = Path.Combine(projectFile.Directory, TempDirectory);
			TypeInfoFilePath = Path.Combine(projectFile.Directory, TypeInfoFilePath);
			EffectFilePath = Path.Combine(projectFile.Directory, EffectFilePath);
			UserInterfaceFilePath = Path.Combine(projectFile.Directory, UserInterfaceFilePath);
			FontLoaderFilePath = Path.Combine(projectFile.Directory, FontLoaderFilePath);
			AssetIdentifierFilePath = Path.Combine(projectFile.Directory, AssetIdentifierFilePath);
			RegistryFilePath = Path.Combine(projectFile.Directory, RegistryFilePath);

			projectFile.ReportInvalidAttributes(configurationElement);
			projectFile.ReportInvalidElements(configurationElement);

			configurationElement.Remove();
		}

		/// <summary>
		///     Gets the absolute path to the source directory of the assets project, i.e., the directory that contains the asset
		///     project file.
		/// </summary>
		public string SourceDirectory { get; private set; }

		/// <summary>
		///     Gets the project identifier that is used to distinguish multiple asset of the same name in different asset projects.
		/// </summary>
		public string ProjectIdentifier { get; private set; }

		/// <summary>
		///     Gets the absolute path to the directory that is used to store temporary files.
		/// </summary>
		public string TempDirectory { get; private set; }

		/// <summary>
		///     Gets the absolute path of the file that contains the generated C# effect code.
		/// </summary>
		public string EffectFilePath { get; private set; }

		/// <summary>
		///     Gets the absolute path of the file that contains the generated C# user interface code.
		/// </summary>
		public string UserInterfaceFilePath { get; private set; }

		/// <summary>
		///     Gets the absolute path of the file that contains the generated C# font loader code.
		/// </summary>
		public string FontLoaderFilePath { get; private set; }

		/// <summary>
		///     Gets the absolute path of the file that contains the generated C# asset identifier code.
		/// </summary>
		public string AssetIdentifierFilePath { get; private set; }

		/// <summary>
		///     Gets the absolute path of the file that contains the generated C# cvar and command registry code.
		/// </summary>
		public string RegistryFilePath { get; private set; }

		/// <summary>
		///     Gets the absolute path of the file that contains the user interface type information required to compile Xaml files.
		/// </summary>
		public string TypeInfoFilePath { get; private set; }

		/// <summary>
		///     Loads a string from a configuration element.
		/// </summary>
		private static string Load(AssetProjectFile projectFile, XElement configurationElement, string elementName, string attributeName)
		{
			var element = configurationElement.Element(elementName);
			if (element == null)
				projectFile.Report(LogType.Fatal, configurationElement, "'{0}' element is missing in configuration section.", elementName);

			var attribute = element.Attribute(attributeName);
			if (attribute == null || String.IsNullOrWhiteSpace(attribute.Value))
				projectFile.Report(LogType.Fatal, element, "'{0}' element is missing a value for attribute '{1}'.", elementName, attributeName);

			attribute.Remove();
			projectFile.ReportInvalidAttributes(element);
			projectFile.ReportInvalidElements(element);

			element.Remove();
			return attribute.Value;
		}

		/// <summary>
		///     Loads the name of a generated file from a configuration element.
		/// </summary>
		private static string Load(AssetProjectFile projectFile, XElement configurationElement, string elementName)
		{
			elementName = "Generated" + elementName + "File";

			var element = configurationElement.Element(elementName);
			if (element == null)
				projectFile.Report(LogType.Fatal, configurationElement, "'{0}' element is missing in configuration section.", elementName);

			var attribute = element.Attribute("Name");
			if (attribute == null || String.IsNullOrWhiteSpace(attribute.Value))
				projectFile.Report(LogType.Fatal, element, "'{0}' element is missing a value for attribute 'Name'.", elementName);

			attribute.Remove();
			projectFile.ReportInvalidAttributes(element);
			projectFile.ReportInvalidElements(element);

			element.Remove();
			return attribute.Value;
		}
	}
}