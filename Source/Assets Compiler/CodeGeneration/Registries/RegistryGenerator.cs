using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Registries
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using Assets;
	using Framework.Platform.Memory;

	/// <summary>
	///   Generates a C# class from a registry specification interface.
	/// </summary>
	public class RegistryGenerator : IDisposable
	{
		/// <summary>
		///   The registry project.
		/// </summary>
		private RegistryProject _project;

		/// <summary>
		///   Gets the errors that have been raised during the compilation.
		/// </summary>
		public IEnumerable<string> Errors { get; private set; }

		/// <summary>
		///   Gets or sets the source file from which the code is generated.
		/// </summary>
		public string SourceFile { get; set; }

		/// <summary>
		///   Gets or sets the file defining the cvars or commands that should be imported.
		/// </summary>
		public string Import { get; set; }

		/// <summary>
		///   Gets the generated code.
		/// </summary>
		public string GeneratedCode { get; private set; }

		/// <summary>
		///   Gets or sets the namespace in which the generated class should live.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (_project != null)
				_project.CSharpFiles.SafeDisposeAll();

			_project.SafeDispose();
		}

		/// <summary>
		///   Generates the registry C# class.
		/// </summary>
		public void GenerateRegistry()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			_project = new RegistryProject
			{
				CSharpFiles = new[] { new EffectAsset(SourceFile) },
				ImportedRegistry = GetImportedRegistry(),
				Namespace = Namespace
			};
			_project.Compile();

			Errors = _project.Errors.Select(error => error.Message);
			GeneratedCode = _project.GeneratedCode;
		}

		/// <summary>
		///   Gets the imported registry.
		/// </summary>
		private Registry GetImportedRegistry()
		{
			if (String.IsNullOrWhiteSpace(Import))
				return new Registry();

			using (var project = new RegistryProject { CSharpFiles = new[] { new EffectAsset(Import) } })
			{
				RegistryFile[] file;
				project.TryGetValidatedFiles(out file);

				project.CSharpFiles.SafeDisposeAll();
				return file[0].Registry;
			}
		}
	}
}