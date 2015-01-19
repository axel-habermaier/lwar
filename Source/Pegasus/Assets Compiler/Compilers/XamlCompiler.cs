namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.IO;
	using System.Xml.Linq;
	using Assets;
	using Commands;
	using Utilities;
	using Xaml;

	/// <summary>
	///     Compiles Xaml assets into C# code targeting the Pegasus UI library.
	/// </summary>
	internal class XamlCompiler : AssetCompiler<XamlAsset>
	{
		/// <summary>
		///     Provides type information for the Xaml compiler.
		/// </summary>
		private XamlTypeInfoProvider _typeInfo;

		/// <summary>
		///     Loads the type information provided by the Xaml project.
		/// </summary>
		/// <param name="fileName">The file that contains the type information.</param>
		public void LoadTypeInfo(string fileName)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);
			_typeInfo = new XamlTypeInfoProvider(fileName);
		}

		/// <summary>
		///     Creates an asset instance for the given XML element or returns null if the type of the asset is not
		///     supported by the compiler.
		/// </summary>
		/// <param name="assetMetadata">The metadata of the asset that should be compiled.</param>
		protected override XamlAsset CreateAsset(XElement assetMetadata)
		{
			if (assetMetadata.Name == "Xaml")
				return new XamlAsset(assetMetadata);

			return null;
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		protected override void Compile(XamlAsset asset, AssetWriter writer)
		{
			var serializer = new XamlToCSharpSerializer(_typeInfo);

			var xamlFile = new XamlFile(asset.AbsoluteSourcePath, _typeInfo);
			if (xamlFile.Root != null)
			{
				var className = Path.GetFileNameWithoutExtension(asset.SourcePath);
				var namespaceName = Path.GetDirectoryName(asset.SourcePath).Replace("/", ".").Replace("\\", ".");
				serializer.SerializeToCSharp(xamlFile, namespaceName, className);
				File.WriteAllText(Path.Combine(Configuration.XamlCodePath, asset.FileNameWithoutExtension + ".ui.cs"), serializer.GetGeneratedCode());
			}
		}
	}
}