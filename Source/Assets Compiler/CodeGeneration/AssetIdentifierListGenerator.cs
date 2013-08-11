using System;

namespace Pegasus.AssetsCompiler.CodeGeneration
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using Framework;

	/// <summary>
	///   Generates a class that contains the identifiers of all compiled assets.
	/// </summary>
	internal class AssetIdentifierListGenerator
	{
		/// <summary>
		///   The assets that have been compiled.
		/// </summary>
		private readonly AssetInfo[] _assets;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="assets">The assets that have been compiled.</param>
		public AssetIdentifierListGenerator(List<Asset> assets)
		{
			Assert.ArgumentNotNull(assets);
			_assets = assets.Where(asset => asset.IdentifierType != null && asset.IdentifierName != null)
							.Select(asset => new AssetInfo
							{
								Asset = asset,
								Name = asset.RelativePathWithoutExtension,
								IdentifierType = asset.IdentifierType
							})
							.ToArray();
		}

		/// <summary>
		///   Generates the asset list code.
		/// </summary>
		/// <param name="namespaceName">The name of the namespace the classes should be placed in.</param>
		public void Generate(string namespaceName)
		{
			var writer = new CodeWriter();
			writer.WriterHeader("//");
			writer.AppendLine("using System;");
			writer.AppendLine("using Pegasus.Framework.Platform.Assets;");
			writer.Newline();

			writer.AppendLine("namespace {0}", namespaceName);
			writer.AppendBlockStatement(() => GenerateRecursive(writer, "", _assets, false));

			File.WriteAllText(Configuration.CSharpAssetIdentifiersFile, writer.ToString());
		}

		/// <summary>
		///   Descends the folder hierarchy and generates an equivalent hierarchy of nested static classes for the asset
		///   identifiers.
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
					writer.AppendLine("public static AssetIdentifier<{0}> {1} = \"{2}\";",
									  asset.IdentifierType, EscapeName(asset.Asset.IdentifierName), asset.Asset.RelativePathWithoutExtension);

				var groups = assets.Where(asset => asset.Name.Contains("/")).GroupBy(asset => asset.Name.Split('/')[0]).ToArray();
				if (groups.Length > 0 && currentAssets.Length > 0)
					writer.Newline();

				for (var i = 0; i < groups.Length; ++i)
				{
					var group = groups[i];
					GenerateRecursive(writer, EscapeName(group.Key), group.Select(asset => asset.RemoveTopLevel()), true);

					if (i + 1 < groups.Length)
						writer.Newline();
				}
			};

			if (encloseWithClass)
			{
				writer.AppendLine("internal static class {0}", className);
				writer.AppendBlockStatement(generate);
			}
			else
				generate();
		}

		/// <summary>
		///   Escapes the name in order to form a valid C# identifier.
		/// </summary>
		/// <param name="name">The name that should be escaped.</param>
		private static
			string EscapeName(string name)
		{
			return name.Replace(".", "_")
					   .Replace(" ", "");
		}

		/// <summary>
		///   Provides information about an asset.
		/// </summary>
		private struct AssetInfo
		{
			/// <summary>
			///   The asset the information is stored for.
			/// </summary>
			public Asset Asset;

			/// <summary>
			///   The identifier type of the asset.
			/// </summary>
			public string IdentifierType;

			/// <summary>
			///   The remaining name of the asset.
			/// </summary>
			public string Name;

			/// <summary>
			///   Removes the top level directory from the current asset name and returns a new asset info instance.
			/// </summary>
			public AssetInfo RemoveTopLevel()
			{
				return new AssetInfo { Asset = Asset, IdentifierType = IdentifierType, Name = Name.Substring(Name.IndexOf('/') + 1) };
			}
		}
	}
}