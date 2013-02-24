using System;

namespace Pegasus.Framework.Platform.Assets.Compilation
{
	using System.IO;
	using System.Linq;

	/// <summary>
	///   Compiles fonts.
	/// </summary>
	public sealed class FontCompiler : AssetCompiler
	{
		/// <summary>
		///   The XML parser that is used to parse the font.
		/// </summary>
		private XmlParser _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		public FontCompiler(string asset)
			: base(asset)
		{
		}

		/// <summary>
		///   Gets a description of the asset type that the compiler supports.
		/// </summary>
		internal override string AssetType
		{
			get { return "Fonts"; }
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		protected override void CompileCore()
		{
			_parser = new XmlParser(Asset.SourcePath);

			ProcessCommon();
			ProcessCharacters();
			ProcessKerning();
			ProcessTexture();
		}

		/// <summary>
		///   Finds the texture name and writes it to the compiled asset file.
		/// </summary>
		private void ProcessTexture()
		{
			var pages = _parser.FindElement(_parser.Root, "pages");
			var pagesList = _parser.FindElements(pages, "page").ToArray();
			if (pagesList.Length != 1)
				Log.Die("Expected exactly one page.");

			var page = pagesList.Single();
			var textureFile = _parser.ReadAttributeString(page, "file");
			var texture = Path.Combine(Path.GetDirectoryName(Asset.RelativePath), textureFile);
			new Texture2DCompiler(texture).Compile(Buffer);
		}

		/// <summary>
		///   Processes the relevant common attributes.
		/// </summary>
		private void ProcessCommon()
		{
			var common = _parser.FindElement(_parser.Root, "common");
			Buffer.WriteUInt16(_parser.ReadAttributeUInt16(common, "scaleW"));
			Buffer.WriteUInt16(_parser.ReadAttributeUInt16(common, "scaleH"));
			Buffer.WriteUInt16(_parser.ReadAttributeUInt16(common, "lineHeight"));
		}

		/// <summary>
		///   Processes the character information.
		/// </summary>
		private void ProcessCharacters()
		{
			var chars = _parser.FindElement(_parser.Root, "chars");
			var numGlyphs = _parser.ReadAttributeUInt32(chars, "count");

			var lowestGlyphId = _parser.FindElements(chars, "char").Min(e => _parser.ReadAttributeUInt32(e, "id"));
			var highestGlyphId = _parser.FindElements(chars, "char").Max(e => _parser.ReadAttributeUInt32(e, "id"));

			if (lowestGlyphId > UInt16.MaxValue)
				Log.Die("Glyph id exceeds limits: {0}", lowestGlyphId);

			if (highestGlyphId > UInt16.MaxValue)
				Log.Die("Glyph id exceeds limits: {0}", highestGlyphId);

			if (_parser.FindElements(chars, "char").All(e => _parser.ReadAttributeUInt32(e, "id") != '.'))
				Log.Die("The font must support the dot '.' character.");

			Buffer.WriteUInt16((ushort)numGlyphs);
			Buffer.WriteUInt16((ushort)lowestGlyphId);
			Buffer.WriteUInt16((ushort)highestGlyphId);
			foreach (var glyph in _parser.FindElements(chars, "char"))
			{
				Buffer.WriteUInt16((ushort)_parser.ReadAttributeUInt32(glyph, "id"));
				Buffer.WriteUInt16(_parser.ReadAttributeUInt16(glyph, "width"));
				Buffer.WriteUInt16(_parser.ReadAttributeUInt16(glyph, "height"));
				Buffer.WriteInt16(_parser.ReadAttributeInt16(glyph, "xoffset"));
				Buffer.WriteInt16(_parser.ReadAttributeInt16(glyph, "yoffset"));
				Buffer.WriteInt16(_parser.ReadAttributeInt16(glyph, "xadvance"));
				Buffer.WriteUInt16(_parser.ReadAttributeUInt16(glyph, "x"));
				Buffer.WriteUInt16(_parser.ReadAttributeUInt16(glyph, "y"));
			}
		}

		/// <summary>
		///   Processes the kerning information.
		/// </summary>
		private void ProcessKerning()
		{
			if (!_parser.HasElement(_parser.Root, "kernings"))
			{
				Buffer.WriteUInt16(0);
				return;
			}

			var kernings = _parser.FindElement(_parser.Root, "kernings");
			var count = _parser.ReadAttributeUInt32(kernings, "count");

			if (count > UInt16.MaxValue)
				Log.Die("Too many kerning pairs: {0}", count);

			Buffer.WriteUInt16((ushort)count);
			var kerningPairs = _parser.FindElements(kernings, "kerning")
									  .OrderBy(e => _parser.ReadAttributeUInt16(e, "first"))
									  .ThenBy(e => _parser.ReadAttributeUInt16(e, "first"));

			foreach (var kerning in kerningPairs)
			{
				Buffer.WriteUInt16(_parser.ReadAttributeUInt16(kerning, "first"));
				Buffer.WriteUInt16(_parser.ReadAttributeUInt16(kerning, "second"));
				Buffer.WriteInt16(_parser.ReadAttributeInt16(kerning, "amount"));
			}
		}
	}
}