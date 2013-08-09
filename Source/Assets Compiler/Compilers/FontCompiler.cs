using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Linq;
	using Assets;
	using Framework.Platform;
	using Framework.Platform.Logging;
	using Framework.Platform.Memory;
	using Fonts;

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
			// Read the font configuration
			var configuration = _parser.Parse(asset.SourcePath);

			var fontFile = Path.Combine(asset.SourceDirectory, (string)configuration["file"]);
			var size = (int)configuration["size"];
			var antialiased = (bool)configuration["antialiased"];
			var renderMode = antialiased ? RenderMode.Antialiased : RenderMode.Aliased;

			// Verify that the font file actually exists
			if (!File.Exists(fontFile))
				Log.Die("Unable to locate '{0}'.", configuration["file"]);

			// Initialize the font data structures
			using (var font = _freeType.CreateFont(fontFile, size, renderMode))
			using (var fontMap = new FontMap(font, asset.RelativePathWithoutExtension + "_fontmap.png"))
			{
				// Write the font map
				fontMap.Compile(buffer);

				// Write the font metadata
				buffer.WriteUInt16(font.LineHeight);

				// Write the glyph metadata
				buffer.WriteUInt16((ushort)font.Glyphs.Count());

				foreach (var glyph in font.Glyphs)
				{
					// Glyphs are identified by their character ASCII id, except for '□', which must lie at index 0
					if (glyph.Character == '□')
						buffer.WriteByte(0);
					else
						buffer.WriteByte((byte)glyph.Character);

					// Write the font map texture coordinates in pixels
					var area = fontMap.GetGlyphArea(glyph.Character);
					buffer.WriteInt16((short)area.Left);
					buffer.WriteInt16((short)area.Top);
					buffer.WriteUInt16((ushort)area.Width);
					buffer.WriteUInt16((ushort)area.Height);
					
					// Write the glyph offsets
					buffer.WriteInt16((short)glyph.OffsetX);
					buffer.WriteInt16((short)(font.Baseline - glyph.OffsetY));
					buffer.WriteInt16((short)glyph.AdvanceX);
				}
			}
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