namespace Pegasus.AssetsCompiler.Commands
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using Assets;
	using CommandLine;
	using Registries;
	using Utilities;

	/// <summary>
	///     Generates C# code for cvar and command registries.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public class RegistryCommand : ICommand
	{
		/// <summary>
		///     Gets or sets the action that should be performed.
		/// </summary>
		public CompilationActions Actions { get; set; }

		/// <summary>
		///     Gets the path of the registry specification file.
		/// </summary>
		[Option("registry", Required = true, HelpText = "The path to the registry specification file.")]
		public string RegistryFile { get; set; }

		/// <summary>
		///     Gets the namespace of the generated code.
		/// </summary>
		[Option("namespace", Required = true, HelpText = "The namespace of the generated code.")]
		public string Namespace { get; set; }

		/// <summary>
		///     Gets the path of the imported registry specification file.
		/// </summary>
		[Option("import", Required = false, HelpText = "The path to the imported specification file.")]
		public string Import { get; set; }

		/// <summary>
		///     Executes the command.
		/// </summary>
		public void Execute()
		{
			Configuration.BasePath = Environment.CurrentDirectory;
			Configuration.TempDirectory = Path.Combine(Configuration.BasePath, "obj", "codegen");

			var registry = new RegistryAsset(RegistryFile, Namespace, Import);

			if (Actions.HasFlag(CompilationActions.Compile))
				GenerateCode(registry);

			if (Actions.HasFlag(CompilationActions.Clean))
				CleanCode(registry);
		}

		/// <summary>
		///     Generates the code for the given registry.
		/// </summary>
		/// <param name="registry">The registry the code should be generated for.</param>
		private void GenerateCode(RegistryAsset registry)
		{
			if (!registry.RequiresCompilation && (registry.ImportedRegistry == null || !registry.ImportedRegistry.RequiresCompilation))
				return;

			var watch = new Stopwatch();
			watch.Start();

			var generator = new RegistryGenerator
			{
				SourceFile = registry.AbsoluteSourcePath,
				Import = Import ?? String.Empty,
				Namespace = registry.Namespace
			};

			File.WriteAllText(registry.GeneratedFilePath, generator.GenerateRegistry());

			registry.WriteMetadata();
			if (registry.ImportedRegistry != null)
				registry.ImportedRegistry.WriteMetadata();

			Log.Info("Generated code for registry specification '{0}' ({1:F2}s).", registry.SourcePath, watch.Elapsed.TotalSeconds);
		}

		/// <summary>
		///     Cleans the code for the given registry.
		/// </summary>
		/// <param name="registry">The registry that should be cleaned.</param>
		private void CleanCode(RegistryAsset registry)
		{
			registry.DeleteMetadata();

			if (registry.ImportedRegistry != null)
				registry.ImportedRegistry.DeleteMetadata();

			File.Delete(registry.GeneratedFilePath);

			Log.Info("Cleaned code for registry specification '{0}'.", registry.SourcePath);
		}
	}
}