﻿namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using Assets;
	using Platform.Memory;

	/// <summary>
	///   Compiles Xaml assets into C# code targeting the Pegasus UI library.
	/// </summary>
	internal class XamlCompiler : AssetCompiler<XamlAsset>
	{
		//// TODO: REMOVE
		//public override bool Compile(IEnumerable<Asset> assets)
		//{
		//	foreach (var xaml in assets.OfType<XamlAsset>())
		//		File.Delete(xaml.HashPath);
		//	return base.Compile(assets);
		//}

		/// <summary>
		///   Compiles the asset.
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