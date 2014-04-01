namespace Pegasus.AssetsCompiler
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;
	using Platform.Logging;

	/// <summary>
	///     Represents the assets project that contains the assets that are compiled.
	/// </summary>
	internal class AssetsProject
	{
		/// <summary>
		///     The namespace of the elements contained in the project file.
		/// </summary>
		private static readonly XNamespace Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";

		/// <summary>
		///     The project XML file.
		/// </summary>
		private readonly XDocument _document;

		/// <summary>
		///     The root node of the project file.
		/// </summary>
		private readonly XElement _projectFile;

		/// <summary>
		///     The file name of the project.
		/// </summary>
		private readonly string _projectFileName;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="projectFile">The path to the project file.</param>
		public AssetsProject(string projectFile)
		{
			Assert.ArgumentNotNullOrWhitespace(projectFile);

			if (!File.Exists(projectFile))
				Log.Die("Unable to load assets project file '{0}'.", projectFile);

			_projectFileName = projectFile;
			_document = XDocument.Load(projectFile);
			_projectFile = _document.Root;

			Name = Path.GetFileNameWithoutExtension(projectFile);
			SourceDirectory = Path.GetFullPath(Path.GetDirectoryName(projectFile));
			TargetDirectory = Environment.CurrentDirectory;
			TempDirectory = Path.Combine(SourceDirectory, "obj");

#if DEBUG
			TempDirectory = Path.Combine(TempDirectory, "Debug");
#else
			TempDirectory = Path.Combine(TempDirectory, "Release");
#endif
		}

		/// <summary>
		///     Gets the root namespace of the project.
		/// </summary>
		public string RootNamespace
		{
			get
			{
				var namespaceNode = _projectFile.Descendants(Namespace + "RootNamespace").FirstOrDefault();
				if (namespaceNode == null)
					Log.Warn("RootNamespace is missing in '{0}'.", Configuration.AssetsProject);

				return namespaceNode == null ? "Unspecified" : namespaceNode.Value;
			}
		}

		/// <summary>
		///     Gets the asset files contained in the project.
		/// </summary>
		public string[] Assets
		{
			get
			{
				return _projectFile.Descendants(Namespace + "None")
								   .Union(_projectFile.Descendants(Namespace + "Content"))
								   .Union(_projectFile.Descendants(Namespace + "Compile"))
								   .Union(_projectFile.Descendants(Namespace + "Page"))
								   .Select(element => element.Attribute("Include").Value)
								   .Select(asset => asset.Replace("\\", "/"))
								   .ToArray();
			}
		}

		/// <summary>
		///     Gets the path to the source assets.
		/// </summary>
		public string SourceDirectory { get; private set; }

		/// <summary>
		///     Gets the path where the temporary asset files should be stored.
		/// </summary>
		public string TempDirectory { get; private set; }

		/// <summary>
		///     Gets the path where the compiled assets should be stored.
		/// </summary>
		public string TargetDirectory { get; private set; }

		/// <summary>
		///     Gets the name of the compiled assets project assembly.
		/// </summary>
		public string AssemblyName
		{
			get
			{
				var assemblyName = _projectFile.Descendants(Namespace + "AssemblyName").FirstOrDefault();
				if (assemblyName == null)
					Log.Die("Unable to determine the assembly name of the assets project.");

				return assemblyName.Value;
			}
		}

		/// <summary>
		/// Gets the name of the assets project.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     Gets the path to the compiled assets assembly.
		/// </summary>
		public string CompiledAssemblyPath
		{
			get
			{
				var outputType = _projectFile.Descendants(Namespace + "OutputType").FirstOrDefault();
				if (outputType == null)
					Log.Die("Unable to determine the output type of the assets project.");

				var extension = String.Empty;
				switch (outputType.Value.ToLower())
				{
					case "winexe":
						extension = "exe";
						break;
					case "library":
						extension = "dll";
						break;
					default:
						Log.Die("Assets project has unsupported output type: '{0}'.", outputType.Value);
						break;
				}

				return Path.Combine(Environment.CurrentDirectory, AssemblyName + "." + extension);
			}
		}

		/// <summary>
		///     Adds the file to the assets project as a child of the parent file.
		/// </summary>
		/// <param name="file">The file that should be added.</param>
		/// <param name="parentFile">The parent file of the file that should be added.</param>
		public void AddFile(string file, string parentFile)
		{
			Assert.ArgumentNotNullOrWhitespace(file);
			Assert.ArgumentNotNullOrWhitespace(parentFile);
			Assert.ArgumentSatisfies(file.EndsWith(".cs"), "Only C# files can be added.");
			Assert.ArgumentSatisfies(parentFile.EndsWith(".cs"), "The parent file must be a C# file.");

			var fileElement = FindFile(file);
			var parentFileElement = FindFile(parentFile);

			if (parentFileElement == null)
				Log.Die("File '{0}' is not referenced by the assets project.", parentFile);

			if (fileElement != null)
				return;

			var newFile = new XElement(Namespace + "Compile",
									   new XAttribute("Include", file.Replace("/", "\\")),
									   new XElement(Namespace + "DependentUpon", Path.GetFileName(parentFile)));

			parentFileElement.AddAfterSelf(newFile);
			_document.Save(_projectFileName);
		}

		/// <summary>
		///     Finds the XML element that corresponds to the file.
		/// </summary>
		/// <param name="file">The file that should be searched for.</param>
		private XElement FindFile(string file)
		{
			var files = _projectFile.Descendants(Namespace + "None")
									.Union(_projectFile.Descendants(Namespace + "Content"))
									.Union(_projectFile.Descendants(Namespace + "Compile"))
									.Where(element => element.Attribute("Include").Value.Replace("\\", "/") == file)
									.ToArray();

			if (files.Length > 2)
				Log.Die("Found multiple references to file '{0}' in assets project.", file);

			return files.Length > 0 ? files[0] : null;
		}
	}
}