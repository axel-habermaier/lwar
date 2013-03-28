using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using Assets;
	using Framework;

	/// <summary>
	///   Provides extension methods for asset compilers and dependent types.
	/// </summary>
	internal static class Extensions
	{
		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="action">The compilation action that should be described.</param>
		/// <param name="asset">The asset for which the action should be described.</param>
		public static void Describe(this CompilationAction action, Asset asset)
		{
			switch (action)
			{
				case CompilationAction.Skip:
					Log.Info("Skipping '{0}' (no changes detected).", asset.RelativePath);
					break;
				case CompilationAction.Copy:
					Log.Info("Copying '{0}' to target directory (compilation skipped; no changes detected).", asset.RelativePath);
					break;
				case CompilationAction.Process:
					Log.Info("Compiling '{0}'...", asset.RelativePath);
					break;
				default:
					throw new InvalidOperationException("Unknown action type.");
			}
		}
	}
}