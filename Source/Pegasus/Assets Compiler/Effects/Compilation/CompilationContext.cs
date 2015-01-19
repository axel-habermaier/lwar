namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.ComponentModel;
	using Commands;
	using Utilities;

	/// <summary>
	///     Provides metadata for the effects compilation.
	/// </summary>
	internal class CompilationContext
	{
		/// <summary>
		///     The prefix that is used for reserved shader identifiers.
		/// </summary>
		public const string ReservedIdentifierPrefix = "_";

		/// <summary>
		///     The prefix that is used for internally generated shader identifiers.
		/// </summary>
		public const string ReservedInternalIdentifierPrefix = "_pg_";

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static CompilationContext()
		{
			switch (Configuration.Platform)
			{
				case PlatformType.Windows:
					CompileGlsl = true;
					goto case PlatformType.WinRT;
				case PlatformType.WindowsPhone:
				case PlatformType.WinRT:
					CheckFxcAvailability();
					CompileHlsl = true;
					break;
				case PlatformType.Linux:
					CompileGlsl = true;
					break;
				default:
					throw new InvalidOperationException("Unknown platform type.");
			}
		}

		/// <summary>
		///     Gets or sets the writer that should be used to write the compiled output.
		/// </summary>
		public AssetWriter Writer { get; set; }

		/// <summary>
		///     Gets or sets the path for temporarily created files during compilation.
		/// </summary>
		public string TempPath { get; set; }

		/// <summary>
		///     Gets a value indicating whether HLSL shaders should be compiled.
		/// </summary>
		public static bool CompileHlsl { get; private set; }

		/// <summary>
		///     Gets a value indicating whether OpenGL3 GLSL shaders should be compiled.
		/// </summary>
		public static bool CompileGlsl { get; private set; }

		/// <summary>
		///     Checks whether the HLSL compiler is available and disable HLSL shader compilation if the compiler cannot be
		///     invoked.
		/// </summary>
		private static void CheckFxcAvailability()
		{
			try
			{
				using (var fxc = new ExternalProcess("fxc", "/?"))
					fxc.Run();
			}
			catch (Win32Exception e)
			{
				if (e.NativeErrorCode == 2)
					Log.Die("HLSL shaders cannot be compiled as fxc.exe could not be found (fxc.exe must be in the system path).");
				else
					Log.Die("Unable to invoke the HLSL compiler: {0}", e.Message);
			}
		}
	}
}