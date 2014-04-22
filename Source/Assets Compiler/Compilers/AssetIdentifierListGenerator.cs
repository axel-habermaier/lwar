namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using CSharp;
	using Platform;

	/// <summary>
	///     Generates a class that contains the identifiers of all compiled assets.
	/// </summary>
	internal class AssetIdentifierListGenerator
	{
		/// <summary>
		///     The assets that have been compiled.
		/// </summary>
		private readonly AssetInfo[] _assets;

		/// <summary>
		///  Indicates whether the asset list has changed since the last compilation.
		/// </summary>
		private readonly bool _hasChanged = true;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="assets">The assets that have been compiled.</param>
		public AssetIdentifierListGenerator(IEnumerable<Asset> assets)
		{
			Assert.ArgumentNotNull(assets);

			var i = 0u;
			_assets = assets.Where(asset => asset.IdentifierType != null && asset.IdentifierName != null)
							.Select(asset => new AssetInfo
							{
								Asset = asset,
								Name = GetAssetPath(asset),
								IdentifierType = asset.IdentifierType,
								Identifier = i++
							})
							.ToArray();

			try
			{
				var path = Path.Combine(Configuration.TempDirectory, String.Format("AssetList.txt"));
				var assetList = String.Join(Environment.NewLine, _assets.OrderBy(a => a.Name).Select(a => a.Name));
				if (!File.Exists(path) || File.ReadAllText(path) != assetList)
					File.WriteAllText(path, assetList);
				else
					_hasChanged = false;
			}
			catch (IOException)
			{
				// We have to recompile
			}
		}

		/// <summary>
		///     Gets the path for the asset that is used to build up the list of nested namespaces. Places shaders in a sub-namespace.
		/// </summary>
		/// <param name="asset">The asset the path should be generated for.</param>
		private static string GetAssetPath(Asset asset)
		{
			if (asset is ShaderAsset)
				return ShaderAsset.GetAssetIdentifier(asset.RelativeDirectory, asset.FileNameWithoutExtension);

			return asset.RelativePathWithoutExtension;
		} 

		/// <summary>
		///     Generates the asset list code.
		/// </summary>
		/// <param name="assetsNamespace">The name of the assets namespace.</param>
		public void Generate(string assetsNamespace)
		{
			if (!_hasChanged)
				return;
			 
			var writer = new CodeWriter();
			writer.WriterHeader();
			writer.AppendLine("using System;");
			writer.AppendLine("using Pegasus.Platform.Assets;");
			writer.NewLine();

			writer.AppendLine("namespace {0}", assetsNamespace);
			writer.AppendBlockStatement(() => GenerateRecursive(writer, "", _assets, false));

			File.WriteAllText(Configuration.CSharpAssetIdentifiersFile, writer.ToString());
		}

		/// <summary>
		///     Descends the folder hierarchy and generates an equivalent hierarchy of nested static classes for the asset
		///     identifiers.
		/// </summary>
		/// <param name="writer">The writer that should be used to write the generated code.</param>
		/// <param name="className">The name of the class that should be generated.</param>
		/// <param name="assets">The assets that should be placed in the generated class or one of its nested classes.</param>
		/// <param name="encloseWithClass">If false, the assets are placed in the current context directly.</param>
		private static void GenerateRecursive(CodeWriter writer, string className, IEnumerable<AssetInfo> assets, bool encloseWithClass)
		{
			Action generate = () =>
			{
				var currentAssets = assets.Where(asset => !asset.Name.Contains("/")).ToArray();
				foreach (var asset in currentAssets)
					writer.AppendLine("public static AssetIdentifier<{0}> {1} {{ get; private set; }}",
									  asset.IdentifierType, EscapeName(asset.Asset.IdentifierName));

				var groups = assets.Where(asset => asset.Name.Contains("/")).GroupBy(asset => asset.Name.Split('/')[0]).ToArray();
				if (groups.Length > 0 && currentAssets.Length > 0)
					writer.NewLine();

				for (var i = 0; i < groups.Length; ++i)
				{
					var group = groups[i];
					GenerateRecursive(writer, EscapeName(group.Key), group.Select(asset => asset.RemoveTopLevel()), true);

					if (i + 1 < groups.Length)
						writer.NewLine();
				}

				if (encloseWithClass)
				{
					writer.NewLine();
					writer.AppendLine("static {0}()", className);
					writer.AppendBlockStatement(() =>
					{
						foreach (var asset in currentAssets)
							writer.AppendLine("{0} = new AssetIdentifier<{1}>(\"{2}.{3}{7}\", {4}, {5}, AssetType.{6});",
											  EscapeName(asset.Asset.IdentifierName), asset.IdentifierType,
											  asset.Asset.RelativePathWithoutExtension, Configuration.AssetsProject.Name,
											  Configuration.AssetProjectIdentifier, asset.Identifier, asset.Asset.AssetType,
											  PlatformInfo.AssetExtension);
					});
				}
			};

			if (encloseWithClass)
			{
				writer.AppendLine("public static class {0}", className);
				writer.AppendBlockStatement(generate);
			}
			else
				generate();
		}

		/// <summary>
		///     Escapes the name in order to form a valid C# identifier.
		/// </summary>
		/// <param name="name">The name that should be escaped.</param>
		private static string EscapeName(string name)
		{
			return name.Replace(".", "_")
					   .Replace(" ", "");
		}

		/// <summary>
		///     Provides information about an asset.
		/// </summary>
		private struct AssetInfo
		{
			/// <summary>
			///     The asset the information is stored for.
			/// </summary>
			public Asset Asset;

			/// <summary>
			///     The identifier type of the asset.
			/// </summary>
			public string IdentifierType;

			/// <summary>
			///     The unique identifier of the asset.
			/// </summary>
			public uint Identifier;

			/// <summary>
			///     The remaining name of the asset.
			/// </summary>
			public string Name;

			/// <summary>
			///     Removes the top level directory from the current asset name and returns a new asset info instance.
			/// </summary>
			public AssetInfo RemoveTopLevel()
			{
				return new AssetInfo
				{
					Asset = Asset,
					IdentifierType = IdentifierType,
					Name = Name.Substring(Name.IndexOf('/') + 1),
					Identifier = Identifier
				};
			}
		}
	}
}