namespace Pegasus.AssetsCompiler
{
	using System;

	/// <summary>
	///     Represents an asset project.
	/// </summary>
	internal class AssetProject
	{
		/// <summary>
		/// The asset project file describing the asset project.
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

		/// <summary>
		///     Parses the asset projects file.
		/// </summary>
		/// <param name="projectFile">The path to the project file that should be parsed.</param>
		private void ParseAssetProjectFile(string projectFile)
		{
		}
	}
}