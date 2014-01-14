namespace Pegasus.AssetsCompiler
{
	using System;
	using System.Linq;
	using AssetCompilers;

	/// <summary>
	///     Represents an asset project.
	/// </summary>
	internal class AssetProject
	{
		/// <summary>
		///     The asset compilers that are used to the compile the project's assets.
		/// </summary>
		private IAssetCompiler[] _assetCompilers;

		/// <summary>
		///     The asset project file describing the asset project.
		/// </summary>
		private AssetProjectFile _projectFile;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="projectFile">The path to the project file of the asset project.</param>
		/// <param name="mode">The mode of the asset compiler.</param>
		public AssetProject(string projectFile, Mode mode)
		{
			Assert.ArgumentNotNull(projectFile);
			Assert.ArgumentInRange(mode);

			_projectFile = new AssetProjectFile(projectFile);

			_assetCompilers = AppDomain.CurrentDomain.GetAssemblies()
									   .SelectMany(a => a.GetTypes())
									   .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IAssetCompiler)))
									   .Select(Activator.CreateInstance)
									   .Cast<IAssetCompiler>()
									   .ToArray();
		}

		/// <summary>
		///     Compiles all assets in the assets project. Assets that have not changed since the last compilation are skipped.
		/// </summary>
		public void Compile()
		{
		}

		/// <summary>
		///     Removes all compiled assets and temporary files.
		/// </summary>
		public void Clean()
		{
		}
	}
}