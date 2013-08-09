using System;

namespace Pegasus.AssetsCompiler
{
	using System.ComponentModel;
	using System.IO;
	using System.Reflection;
	using Framework.Platform;
	using Framework.Platform.Logging;

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
		///   The path to the source assets.
		/// </summary>
		public static readonly string SourceDirectory = Path.Combine(Environment.CurrentDirectory, "../../Assets");

		/// <summary>
		///   The path to the assets list.
		/// </summary>
		public static readonly string AssetListPath = Path.Combine(Environment.CurrentDirectory, "Assets.dll");

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
		///   The path to the C# file that should contain the generated effect code.
		/// </summary>
		public static readonly string CSharpEffectFile = Path.Combine(SourceDirectory, "Effects", "Effects.cs");

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
	}
}