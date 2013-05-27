﻿using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Registries
{
	using System.Collections.Generic;
	using System.Linq;
	using Assets;
	using Framework;
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
		///   Gets or sets the base class the generated C# class should be derived from.
		/// </summary>
		public string BaseClass { get; set; }

		/// <summary>
		///   Gets the generated code.
		/// </summary>
		public string GeneratedCode { get; private set; }

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
			Assert.NotNullOrWhitespace(SourceFile);
			Assert.NotNullOrWhitespace(BaseClass);

			_project = new RegistryProject { CSharpFiles = new[] { new CSharpAsset(SourceFile) }, BaseClass = BaseClass };
			_project.Compile();

			Errors = _project.Errors.Select(error => error.Message);
			GeneratedCode = _project.GeneratedCode;
		}
	}
}