using System;

namespace Pegasus.AssetsCompiler
{
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;
	using Framework;
	using Framework.Platform.Logging;

	/// <summary>
	///   Represents the assets project that contains the assets that are compiled.
	/// </summary>
	internal class AssetsProject
	{
		/// <summary>
		///   The namespace of the elements contained in the project file.
		/// </summary>
		private static readonly XNamespace Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";

		/// <summary>
		///   The root node of the project file.
		/// </summary>
		private readonly XElement _projectFile;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="projectFile">The path to the project file.</param>
		public AssetsProject(string projectFile)
		{
			Assert.ArgumentNotNullOrWhitespace(projectFile);

			if (!File.Exists(projectFile))
				Log.Die("Unable to load assets project file '{0}'.", projectFile);

			_projectFile = XDocument.Load(projectFile).Root;
		}

		/// <summary>
		///   Gets the root namespace of the project.
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
		///   Gets the asset files contained in the project.
		/// </summary>
		public string[] Assets
		{
			get
			{
				return _projectFile.Descendants(Namespace + "None")
								   .Union(_projectFile.Descendants(Namespace + "Content"))
								   .Union(_projectFile.Descendants(Namespace + "Compile"))
								   .Select(element => element.Attribute("Include").Value)
								   .Select(asset => asset.Replace("\\", "/"))
								   .ToArray();
			}
		}
	}
}