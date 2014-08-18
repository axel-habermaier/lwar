namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using Platform.Logging;
	using Xaml;

	/// <summary>
	///     Compiles Xaml assets into C# code targeting the Pegasus UI library.
	/// </summary>
	internal class XamlCompiler : AssetCompiler<XamlAsset>
	{
		/// <summary>
		///     Compiles all assets of the compiler's asset source type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		public override void Compile(IEnumerable<Asset> assets)
		{
			var xamlAssets = assets.OfType<XamlAsset>().ToArray();

			if (DetermineAction(xamlAssets) == CompilationAction.Skip)
				Log.Info("Skipping compilation of Xaml files (no changes detected).");
			else
			{
				var typeInfo = new XamlTypeInfoProvider(Path.Combine(Configuration.SourceDirectory, "TypeInfo.xml"));
				var serializer = new XamlToCSharpSerializer(typeInfo);

				foreach (var asset in xamlAssets)
				{
					Log.Info("Compiling '{0}'...", asset.RelativePath);

					var xamlFile = new XamlFile(asset.SourcePath, typeInfo);
					if (xamlFile.Root != null)
					{
						var className = Path.GetFileNameWithoutExtension(asset.RelativePath);
						var namespaceName = Path.GetDirectoryName(asset.RelativePath).Replace("/", ".").Replace("\\", ".");
						serializer.SerializeToCSharp(xamlFile, namespaceName, className);
					}

					Hash.Compute(asset.SourcePath).WriteTo(asset.HashPath);
				}

				File.WriteAllText(Configuration.CSharpXamlFile, serializer.GetGeneratedCode());
			}
		}

		/// <summary>
		///     Removes the compiled assets and all temporary files written by the compiler.
		/// </summary>
		/// <param name="assets">The assets that should be cleaned.</param>
		public override void Clean(IEnumerable<Asset> assets)
		{
			foreach (var asset in assets.OfType<XamlAsset>())
			{
				File.Delete(asset.TempPath);
				File.Delete(asset.HashPath);
			}
		}

		/// <summary>
		///     Checks whether any of the Xaml assets have changed.
		/// </summary>
		/// <param name="xamlAssets">The Xaml assets that should be checked to determine the compilation action.</param>
		private static CompilationAction DetermineAction(IEnumerable<XamlAsset> xamlAssets)
		{
			foreach (var asset in xamlAssets)
			{
				if (!File.Exists(asset.HashPath))
					return CompilationAction.Process;

				var oldHash = Hash.FromFile(asset.HashPath);
				var newHash = Hash.Compute(asset.SourcePath);

				if (oldHash != newHash)
					return CompilationAction.Process;
			}

			return CompilationAction.Skip;
		}
	}
}