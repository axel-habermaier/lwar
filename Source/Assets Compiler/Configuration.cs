namespace Pegasus.AssetsCompiler
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Assets;
	using Platform;
	using Platform.Logging;

	/// <summary>
	///   Provides access to the asset compiler configuration.
	/// </summary>
	internal static class Configuration
	{
		/// <summary>
		///   The prefix that is used for reserved shader identifiers.
		/// </summary>
		public const string ReservedIdentifierPrefix = "_";

		/// <summary>
		///   The prefix that is used for internally generated shader identifiers.
		/// </summary>
		public const string ReservedInternalIdentifierPrefix = "_pg_";

		/// <summary>
		///   The asset list assembly.
		/// </summary>
		private static Assembly _assetListAssembly;

		/// <summary>
		///   The assets project file.
		/// </summary>
		private static AssetsProject _assetsProject;

		/// <summary>
		///   Gets the path to the compiled assets project.
		/// </summary>
		public static string AssetListPath
		{
			get { return AssetsProject.CompiledAssemblyPath; }
		}

		/// <summary>
		///   Gets the path to the C# file that should contain the generated effect code.
		/// </summary>
		public static string CSharpEffectFile
		{
			get { return Path.Combine(SourceDirectory, "Effects", "Effects.cs"); }
		}

		/// <summary>
		///   Gets the path to the C# file that should contain the C# code generated from the Xaml files.
		/// </summary>
		public static string CSharpXamlFile
		{
			get { return Path.Combine(SourceDirectory, "UserInterface.cs"); }
		}

		/// <summary>
		///   Gets the path to the C# file that should contain the generated asset identifiers.
		/// </summary>
		public static string CSharpAssetIdentifiersFile
		{
			get { return Path.Combine(SourceDirectory, "AssetIdentifiers.cs"); }
		}

		/// <summary>
		///   Gets the path to the C# file that should contain the font loader class.
		/// </summary>
		public static string CSharpFontLoaderFile
		{
			get { return Path.Combine(SourceDirectory, "Fonts", "FontLoader.cs"); }
		}

		/// <summary>
		///   Gets or set the path to the assets project that is compiled.
		/// </summary>
		public static string AssetsProjectPath { get; set; }

		/// <summary>
		///   Gets the path to the source assets.
		/// </summary>
		public static string SourceDirectory
		{
			get { return AssetsProject.SourceDirectory; }
		}

		/// <summary>
		///   Gets the path where the temporary asset files should be stored.
		/// </summary>
		public static string TempDirectory
		{
			get { return AssetsProject.TempDirectory; }
		}

		/// <summary>
		///   Gets the path where the compiled assets should be stored.
		/// </summary>
		public static string TargetDirectory
		{
			get { return AssetsProject.TargetDirectory; }
		}

		/// <summary>
		///   Gets or sets a value indicating whether Xaml files should be the only kind of asset that is compiled.
		/// </summary>
		public static bool XamlFilesOnly { get; set; }

		/// <summary>
		///   Gets or sets the unique identifier of all asset files contained in the compiled assets project.
		/// </summary>
		public static int UniqueFileIdentifier { get; set; }

		/// <summary>
		///   Get the asset list assembly.
		/// </summary>
		public static Assembly AssetListAssembly
		{
			get
			{
				if (!File.Exists(AssetListPath))
					Log.Die("Unable to load asset list assembly '{0}'.", AssetListPath);

				if (_assetListAssembly == null)
					_assetListAssembly = Assembly.LoadFile(AssetListPath);

				return _assetListAssembly;
			}
		}

		/// <summary>
		///   Gets the assets project file.
		/// </summary>
		public static AssetsProject AssetsProject
		{
			get
			{
				if (_assetsProject == null)
					_assetsProject = new AssetsProject(AssetsProjectPath);

				return _assetsProject;
			}
		}

		/// <summary>
		///   Gets a value indicating whether HLSL shaders should be compiled.
		/// </summary>
		public static bool CompileHlsl { get; private set; }

		/// <summary>
		///   Checks whether the HLSL compiler is available and disable HLSL shader compilation if the compiler cannot be invoked.
		/// </summary>
		public static void CheckFxcAvailability()
		{
			try
			{
				using (var fxc = new ExternalProcess("fxc", "/?"))
					fxc.Run();
				CompileHlsl = true;
			}
			catch (Win32Exception e)
			{
				if (e.NativeErrorCode == 2)
					Log.Warn("HLSL shaders will not be compiled as fxc.exe could not be found (fxc.exe must be in the system path). " +
							 "On Linux, HLSL shader compilation is not supported.");
				else
					Log.Error("Unable to invoke the HLSL compiler; HLSL shaders will not be compiled: {0}", e.Message);

				CompileHlsl = false;
			}
		}

		/// <summary>
		///   Gets the types that should be checked when searching all relevant types via reflection.
		/// </summary>
		public static IEnumerable<Type> GetReflectionTypes()
		{
			var assetCompilerTypes = Assembly.GetExecutingAssembly().GetTypes();
			if (XamlFilesOnly)
				return assetCompilerTypes;

			var assetListTypes = AssetListAssembly.GetTypes();
			return assetListTypes.Union(assetCompilerTypes);
		}
	}
}