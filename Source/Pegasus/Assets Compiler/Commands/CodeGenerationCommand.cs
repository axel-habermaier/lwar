namespace Pegasus.AssetsCompiler.Commands
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;
	using Assets;
	using CommandLine;
	using Utilities;

	/// <summary>
	///     Generates boilerplate C# code for a given C# project.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public class CodeGenerationCommand : ICommand
	{
		/// <summary>
		///     The namespace of the elements contained in the project file.
		/// </summary>
		private static readonly XNamespace Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";

		/// <summary>
		///     Gets the path of the source file.
		/// </summary>
		[Option("project", Required = true, HelpText = "The path to the C# project file.")]
		public string ProjectFile { get; set; }

		/// <summary>
		///     Executes the command.
		/// </summary>
		public void Execute()
		{
			Configuration.BasePath = Path.GetDirectoryName(ProjectFile);
			Configuration.TempDirectory = Path.Combine("obj", "codegen");

			if (!File.Exists(ProjectFile))
				Log.Die("Unable to load assets project file '{0}'.", ProjectFile);

			var project = XDocument.Load(ProjectFile).Root;
			var csharpFiles = project.Descendants(Namespace + "Compile")
									 .Select(element => element.Attribute("Include").Value)
									 .Select(asset => new CSharpAsset(asset.Replace("\\", "/")))
									 .Where(asset => asset.RequiresCompilation);

			foreach (var file in csharpFiles)
			{
				Log.Info("compiling; {0}", file);
				file.WriteMetadata();
			}
		}
	}
}