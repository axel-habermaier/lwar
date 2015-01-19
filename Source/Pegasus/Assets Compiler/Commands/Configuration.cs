namespace Pegasus.AssetsCompiler.Commands
{
	using System;
	using Utilities;

	/// <summary>
	///     Provides access to the asset compiler configuration.
	/// </summary>
	internal static class Configuration
	{
		/// <summary>
		///     The file extension of compiled asset bundles.
		/// </summary>
		public const string BundleFileExtension = ".pak";

		/// <summary>
		///     The compiled asset file version supported by the application.
		/// </summary>
		public const ushort AssetFileVersion = 2;

		/// <summary>
		///     Gets or sets the path where the temporary asset files should be stored.
		/// </summary>
		public static string TempDirectory { get; set; }

		/// <summary>
		///     Gets or sets the base path that all asset source paths are relative to.
		/// </summary>
		public static string BasePath { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the texture is a cube map.
		/// </summary>
		public static bool Debug { get; set; }

		/// <summary>
		///     Gets or sets the path of a directory where generated C# Xaml code should be stored.
		/// </summary>
		public static string XamlCodePath { get; set; }

		/// <summary>
		///     Gets or sets the path of a directory where generated C# effect code should be stored.
		/// </summary>
		public static string EffectCodePath { get; set; }

		/// <summary>
		///     Gets or sets the path of a directory where generated C# bundle code should be stored.
		/// </summary>
		public static string BundleCodePath { get; set; }

		/// <summary>
		///     Gets or sets the root namespace of generated C# classes.
		/// </summary>
		public static string RootNamespace { get; set; }

		/// <summary>
		///     Gets or sets a value indicating which platform the bundle is compiled for.
		/// </summary>
		public static PlatformType Platform { get; set; }

		/// <summary>
		///     Gets or sets the default visibility of generated C# classes.
		/// </summary>
		public static string Visibility { get; set; }
	}
}