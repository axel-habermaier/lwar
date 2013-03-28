using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Assets;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a C# project with effect declarations that have to be cross-compiled.
	/// </summary>
	internal class EffectsProject
	{
		/// <summary>
		///   The effect files that the project consists of.
		/// </summary>
		private EffectFile[] _files;

		/// <summary>
		///   The C# project that is compiled.
		/// </summary>
		private IProjectContent _project = new CSharpProjectContent();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="assets">The C# assets that should be added to the project.</param>
		public EffectsProject(CSharpAsset[] assets)
		{
			Assert.ArgumentNotNull(assets, () => assets);

			LoadAssemblies();
			LoadAssets(assets);
		}

		/// <summary>
		///   Gets the cross-compiled shader assets.
		/// </summary>
		public IEnumerable<Asset> ShaderAssets { get; private set; }

		/// <summary>
		///   Loads the required assemblies into the project.
		/// </summary>
		private void LoadAssemblies()
		{
			var loader = new CecilLoader();

			var assemblies = new[] { typeof(int).Assembly, typeof(EffectsProject).Assembly };
			foreach (var assembly in assemblies)
			{
				var loadedAssembly = loader.LoadAssemblyFile(assembly.Location);
				_project = _project.AddAssemblyReferences(loadedAssembly);
			}
		}

		/// <summary>
		///   Parses and loads the C# asset files into the project.
		/// </summary>
		/// <param name="assets">The assets that should be loaded into the project.</param>
		private void LoadAssets(IEnumerable<CSharpAsset> assets)
		{
			_files = assets.Select(a => new EffectFile(a)).ToArray();

			foreach (var file in _files)
				_project = _project.AddOrUpdateFiles(file.UnresolvedFile);
		}

		/// <summary>
		///   Compiles all effects. Returns false to indicate that compilation errors have occurred.
		/// </summary>
		public bool Compile()
		{
			var compilation = _project.CreateCompilation();
			var context = new CompilationContext();

			var effects = _files.SelectMany(file =>
				{
					context.File = file;
					context.Resolver = new CSharpAstResolver(compilation, file.SyntaxTree, file.UnresolvedFile);

					return file.Compile(context);
				}).ToArray();

			ShaderAssets = new Asset[0];
			return !context.HasErrors;
		}
	}
}