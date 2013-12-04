namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using Assets;
	using CSharp;
	using Platform.Logging;
	using Platform.Memory;
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
		public override bool Compile(IEnumerable<Asset> assets)
		{
			// TODO: REMOVE
			//foreach (var xaml in assets.OfType<XamlAsset>())
				//File.Delete(xaml.HashPath);

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
					Hash.Compute(asset.SourcePath).WriteTo(asset.HashPath);

					var xamlFile = new XamlFile(asset.SourcePath, typeInfo);
					if (xamlFile.Root == null)
						continue;

					var className = Path.GetFileNameWithoutExtension(asset.RelativePath);
					var namespaceName = Path.GetDirectoryName(asset.RelativePath).Replace("/", ".").Replace("\\", ".");
					serializer.SerializeToCSharp(xamlFile, namespaceName, className);
				}

				File.WriteAllText(Configuration.CSharpXamlFile, serializer.GetGeneratedCode());
			}

			return true;
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

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void Compile(XamlAsset asset, BufferWriter buffer)
		{
			//var className = Path.GetFileNameWithoutExtension(asset.RelativePath);
			//var namespaceName = Path.GetDirectoryName(asset.RelativePath).Replace("/", ".").Replace("\\", ".");

			//var xamlFile = new XamlFile(asset.SourcePath);
			//if (xamlFile.Root == null)
			//	return;

			//var csharpSerializer = new XamlToCSharpSerializer(xamlFile, namespaceName, className);
			//buffer.Copy(Encoding.UTF8.GetBytes(csharpSerializer.GetGeneratedCode()));

			//var preprocessedXamlFile = XDocument.Parse(xamlFile.Root.ToString()).ToString();
			//var lines = preprocessedXamlFile.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

			//buffer.Copy(Encoding.UTF8.GetBytes(Environment.NewLine + Environment.NewLine));
			//foreach (var line in lines)
			//	buffer.Copy(Encoding.UTF8.GetBytes("// " + line + Environment.NewLine));
		}
	}
}