namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Linq;
	using System.Xml.Linq;
	using Assets;
	using Fonts;
	using Utilities;

	/// <summary>
	///     Compiles texture-based fonts.
	/// </summary>
	[UsedImplicitly]
	internal sealed class FontCompiler : AssetCompiler<FontAsset>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public FontCompiler()
			: base(singleThreaded: true)
		{
		}

		/// <summary>
		///     Creates an asset instance for the given XML element or returns null if the type of the asset is not
		///     supported by the compiler.
		/// </summary>
		/// <param name="assetMetadata">The metadata of the asset that should be compiled.</param>
		protected override FontAsset CreateAsset(XElement assetMetadata)
		{
			if (assetMetadata.Name == "Font")
				return new FontAsset(assetMetadata);

			return null;
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The writer the compilation output should be appended to.</param>
		protected override void Compile(FontAsset asset, AssetWriter buffer)
		{
			Assert.That(!asset.Bold, "Bold fonts are currently not supported.");
			Assert.That(!asset.Italic, "Italic fonts are currently not supported.");

			var renderMode = asset.Aliased ? RenderMode.Aliased : RenderMode.Antialiased;

			// Initialize the font data structures
			using (var freeType = new FreeTypeLibrary())
			using (var font = freeType.CreateFont(asset.AbsoluteSourcePath, asset.Size, asset.Bold, asset.Italic, renderMode,
				asset.CharacterRange, asset.InvalidChar))
			{
				// Write the font map
				var fontMap = new FontMap(font, asset.SourcePath + asset.RuntimeName + ".Fontmap.png");
				fontMap.Compile(buffer);

				// Write the font metadata
				buffer.WriteString(asset.Family);
				buffer.WriteInt32(asset.Size);
				buffer.WriteBoolean(asset.Bold);
				buffer.WriteBoolean(asset.Italic);
				buffer.WriteBoolean(asset.Aliased);
				buffer.WriteUInt16(font.LineHeight);

				// Write the glyph metadata
				buffer.WriteUInt16((ushort)font.Glyphs.Count());

				foreach (var glyph in font.Glyphs)
				{
					// Glyphs are identified by their character ASCII id, except for the invalid character, which must lie at index 0
					if (glyph.Character == asset.InvalidChar)
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

				// Write the kerning information, if any
				if (!font.HasKerning)
				{
					buffer.WriteUInt16(0);
					return;
				}

				var pairs = (from left in font.Glyphs
							 from right in font.Glyphs
							 let offset = font.GetKerning(left, right)
							 where offset != 0
							 select new { Left = left, Right = right, Offset = offset }).ToArray();

				Assert.That(pairs.Length < UInt16.MaxValue, "Too many kerning pairs.");
				buffer.WriteUInt16((ushort)pairs.Length);

				foreach (var pair in pairs)
				{
					buffer.WriteUInt16(pair.Left.Character);
					buffer.WriteUInt16(pair.Right.Character);
					buffer.WriteInt16((short)pair.Offset);
				}
			}
		}
	}
}