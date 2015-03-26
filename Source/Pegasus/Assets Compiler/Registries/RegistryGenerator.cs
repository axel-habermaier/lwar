namespace Pegasus.AssetsCompiler.Registries
{
	using System;
	using CSharp;

	/// <summary>
	///     Generates a C# class from a registry specification interface.
	/// </summary>
	public class RegistryGenerator
	{
		/// <summary>
		///     The registry project.
		/// </summary>
		private RegistryProject _project;

		/// <summary>
		///     Gets or sets the source file from which the code is generated.
		/// </summary>
		public string SourceFile { get; set; }

		/// <summary>
		///     Gets or sets the file defining the cvars or commands that should be imported.
		/// </summary>
		public string Import { get; set; }

		/// <summary>
		///     Gets or sets the namespace in which the generated class should live.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		///     Generates the registry C# class.
		/// </summary>
		public string GenerateRegistry()
		{
			_project = new RegistryProject
			{
				CSharpFiles = new[] { new CSharpFile("", SourceFile) },
				ImportedRegistry = GetImportedRegistry(),
				Namespace = Namespace
			};
			_project.Compile();

			return _project.GeneratedCode;
		}

		/// <summary>
		///     Gets the imported registry.
		/// </summary>
		private Registry GetImportedRegistry()
		{
			if (String.IsNullOrWhiteSpace(Import))
				return new Registry();

			var project = new RegistryProject { CSharpFiles = new[] { new CSharpFile("", Import) } };
			var files = project.GetValidatedFiles();

			return files[0].Registry;
		}
	}
}