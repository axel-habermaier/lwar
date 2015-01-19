namespace Pegasus.AssetsCompiler.Commands
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;
	using CommandLine;
	using Compilers;
	using Utilities;

	/// <summary>
	///     Compiles or cleans a Xaml bundle.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public class XamlCommand : ICommand
	{
		/// <summary>
		///     Gets the path of the source file.
		/// </summary>
		[Option("project", Required = true, HelpText = "The path to the Xaml project file.")]
		public string ProjectFile { get; set; }

		/// <summary>
		///     Gets the path of a directory where the generated code should be stored.
		/// </summary>
		[Option("target", Required = true, HelpText = "The target path where the generated code should be stored.")]
		public string TargetPath { get; set; }

		/// <summary>
		///     Gets or sets the action that should be performed on the asset bundle.
		/// </summary>
		public CompilationActions Actions { get; set; }

		/// <summary>
		///     Executes the command.
		/// </summary>
		public void Execute()
		{
			Configuration.Debug = false;
			Configuration.TempDirectory = Path.Combine("obj", "XamlUI");
			Configuration.Platform = PlatformType.Windows;
			Configuration.BasePath = Path.GetDirectoryName(ProjectFile);
			Configuration.XamlCodePath = TargetPath;

			if (!String.IsNullOrWhiteSpace(Configuration.XamlCodePath))
				Directory.CreateDirectory(Configuration.XamlCodePath);

			if (!File.Exists(ProjectFile))
				Log.Die("Unable to load Xaml project file '{0}'.", ProjectFile);

			var xamlNamespace = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");
			var document = XDocument.Load(ProjectFile);
			var root = document.Root;

			Configuration.RootNamespace = GetRootNamespace(root);

			var compiler = new XamlCompiler();
			compiler.LoadTypeInfo(Path.Combine(Path.GetDirectoryName(ProjectFile), "TypeInfo.xml"));

			var assets = root.Descendants(xamlNamespace + "Page")
							 .Select(element => element.Attribute("Include").Value)
							 .Select(asset => asset.Replace("\\", "/"))
							 .Select(asset => new XElement("Xaml", new XAttribute("File", asset)))
							 .ToArray();

			if (Actions.HasFlag(CompilationActions.Clean))
			{
				foreach (var asset in assets)
					compiler.Clean(asset);

				Log.Info("Cleaned Xaml project '{0}'.", Path.GetFileNameWithoutExtension(ProjectFile));
			}

			if (Actions.HasFlag(CompilationActions.Compile))
			{
				var watch = new Stopwatch();
				watch.Start();

				compiler.Compile(assets).Wait();

				var elapsedSeconds = watch.ElapsedMilliseconds / 1000.0;
				Log.Info("Compiled Xaml project '{0}' ({1:F2}s).", Path.GetFileNameWithoutExtension(ProjectFile), elapsedSeconds);
			}
		}

		/// <summary>
		///     Gets the root namespace of the Xaml project.
		/// </summary>
		public string GetRootNamespace(XElement projectFile)
		{
			var xns = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");
			var namespaceNode = projectFile.Descendants(xns + "RootNamespace").FirstOrDefault();
			if (namespaceNode == null)
				Log.Warn("RootNamespace is missing in '{0}'.", projectFile);

			return namespaceNode == null ? "Unspecified" : namespaceNode.Value;
		}
	}
}