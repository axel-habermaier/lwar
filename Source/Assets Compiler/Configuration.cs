using System;

namespace Pegasus.AssetsCompiler
{
	using System.ComponentModel;
	using System.IO;
	using System.Reflection;
	using Framework;
	using Framework.Platform;

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

		/// <summary>
		///   Gets a value indicating whether HLSL shaders should be compiled.
		/// </summary>
		public static bool CompileHlsl { get; private set; }

		/// <summary>
		///   Checks the prerequisites of the shader compiler.
		/// </summary>
		public static bool CheckShaderCompilationPrerequisites()
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
				{
					Log.Warn("HLSL shaders will not be compiled as fxc.exe could not be found.");
					switch (PlatformInfo.Platform)
					{
						case PlatformType.Linux:
							Log.Warn("HLSL shader compilation is not supported on Linux.");
							break;
						case PlatformType.Windows:
							Log.Warn("fxc.exe must be in the system path.");
							break;
						default:
							throw new InvalidOperationException("Unknown platform.");
					}
				}
				else
					Log.Error("Unable to invoke the HLSL compiler; HLSL shaders will not be compiled: {0}", e.Message);

				CompileHlsl = false;
			}

			try
			{
				using (var fsi = new ExternalProcess("fsi", "--help"))
					fsi.Run();
			}
			catch (Win32Exception e)
			{
				Log.Error("Unable to invoke F# Interactive. Check whether fsi.exe is in your path. The error was: {0}", e.Message);
				return false;
			}

			return true;
		}
	}
}