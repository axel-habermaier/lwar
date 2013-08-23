using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using Assets;
	using CodeGeneration;
	using Platform.Memory;
	using UserInterface.Markup;

	/// <summary>
	///   Compiles Xaml assets into C# code targeting the Pegasus UI library.
	/// </summary>
	internal class XamlCompiler : AssetCompiler<XamlAsset>
	{
		// TODO: REMOVE
		public override bool Compile(IEnumerable<Asset> assets)
		{
			foreach (var xaml in assets.OfType<XamlAsset>())
				File.Delete(xaml.HashPath);
			return base.Compile(assets);
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void Compile(XamlAsset asset, BufferWriter buffer)
		{
			var className = Path.GetFileNameWithoutExtension(asset.RelativePath);
			var namespaceName = asset.RelativePath.Substring(0, asset.RelativePath.Length - asset.FileName.Length - 1);

			var writer = new CodeWriter();
			writer.WriterHeader("//");

			var xamlFile = new XamlFile(asset.SourcePath);
			xamlFile.GenerateCode(writer, namespaceName.Replace("/", "."), className);

			buffer.Copy(Encoding.UTF8.GetBytes(writer.ToString()));
		}
	}
}