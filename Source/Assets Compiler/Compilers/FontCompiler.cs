using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.Drawing.Imaging;
	using System.IO;
	using Assets;
	using Framework.Platform;
	using Framework.Platform.Logging;
	using Framework.Platform.Memory;
	using FreeType;

	/// <summary>
	///   Compiles texture-based fonts.
	/// </summary>
	[UsedImplicitly]
	internal sealed class FontCompiler : AssetCompiler<FontAsset>
	{
		/// <summary>
		///   The freetype library instance that is to generate the font textures.
		/// </summary>
		private readonly FreeTypeLibrary _freeType = new FreeTypeLibrary();

		/// <summary>
		///   The parser that is used to parse the font definitions.
		/// </summary>
		private readonly ConfigurationFileParser _parser = new ConfigurationFileParser(new Dictionary<string, Func<string, object>>
		{
			{ "file", s => s },
			{ "size", s => Int32.Parse(s) },
			{ "antialiased", s => Boolean.Parse(s) },
		});

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void Compile(FontAsset asset, BufferWriter buffer)
		{
			var configuration = _parser.Parse(asset.SourcePath);
			var fontFile = Path.Combine(asset.SourceDirectory, (string)configuration["file"]);
			var size = (int)configuration["size"];
			var antialiased = (bool)configuration["antialiased"];

			using (var font = _freeType.CreateFont(fontFile, 0))
			{
				font.Size = size;
				using (var bitmap = font.GetGlyphBitmap('w', antialiased ? RenderMode.Antialiased : RenderMode.Aliased))
					bitmap.Save(asset.TempPathWithoutExtension + "w.png", ImageFormat.Png);
			}
		}

		/// <summary>
		///   Finds the texture name and writes it to the compiled asset file.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		private void ProcessTexture(Asset asset, BufferWriter buffer)
		{
			var path = ""; // TODO
			if (path == null)
				Log.Die("Could not retrieve texture path from font. Maybe more than one texture is used.");

			using (var texture = new Texture2DAsset(path) { Mipmaps = false })
				new Texture2DCompiler().CompileSingle(texture, buffer);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_freeType.SafeDispose();
		}
	}
}