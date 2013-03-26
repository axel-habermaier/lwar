using System;

namespace Pegasus.AssetsCompiler
{
	using System.IO;
	using System.Reflection;

	/// <summary>
	///   Provides access to the asset compiler configuration.
	/// </summary>
	internal static class Configuration
	{
		/// <summary>
		///   The path to the source assets.
		/// </summary>
		public static readonly string SourceDirectory = Path.Combine(Environment.CurrentDirectory, "../../Assets");

		/// <summary>
		///   The path to the assets list.
		/// </summary>
		private static readonly string AssetListPath = Path.Combine(Environment.CurrentDirectory, "Assets.dll");

		/// <summary>
		///   The asset list assembly.
		/// </summary>
		private static Assembly _assetListAssembly;

		/// <summary>
		///   The path to the assets project.
		/// </summary>
		public static readonly string AssetsProject = Path.Combine(SourceDirectory, "Assets.csproj");

		/// <summary>
		///   The path where the temporary asset files should be stored.
		/// </summary>
		public static readonly string TempDirectory = Path.Combine(Environment.CurrentDirectory, "../../Assets/obj");

		/// <summary>
		///   The path where the compiled assets should be stored.
		/// </summary>
		public static readonly string TargetDirectory = Path.Combine(Environment.CurrentDirectory, "Assets");

		/// <summary>
		///   Get the asset list assembly.
		/// </summary>
		public static Assembly AssetListAssembly
		{
			get
			{
				if (_assetListAssembly == null)
					_assetListAssembly = Assembly.LoadFile(AssetListPath);

				return _assetListAssembly;
			}
		}
	}
}