using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.IO;
	using System.Linq;
	using Assets;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Logging;
	using Framework.Platform.Memory;

	/// <summary>
	///   Compiles fonts.
	/// </summary>
	[UsedImplicitly]
	internal sealed class FontCompiler : AssetCompiler<FontAsset>
	{
		/// <summary>
		///   The XML parser that is used to parse the font.
		/// </summary>
		private XmlParser _parser;

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void Compile(FontAsset asset, BufferWriter buffer)
		{
			_parser = new XmlParser(asset.SourcePath);

			ProcessCommon(buffer);
			ProcessCharacters(buffer);
			ProcessKerning(buffer);
			ProcessTexture(asset, buffer);
		}

		/// <summary>
		///   Removes the compiled asset and all temporary files written by the compiler.
		/// </summary>
		/// <param name="asset">The asset that should be cleaned.</param>
		protected override void Clean(FontAsset asset)
		{
			_parser = new XmlParser(asset.SourcePath);

			var path = GetTexturePath(asset);
			if (path == null)
				return;

			using (var texture = new Texture2DAsset(path) { Mipmaps = false })
				new Texture2DCompiler().Clean(new[] { texture });
		}

		/// <summary>
		///   Gets the path to the font's texture.
		/// </summary>
		/// <param name="asset">The asset that should be used to get the path.</param>
		private string GetTexturePath(Asset asset)
		{
			var pages = _parser.FindElement(_parser.Root, "pages");
			var pagesList = _parser.FindElements(pages, "page").ToArray();
			if (pagesList.Length != 1)
				return null;

			var page = pagesList.Single();
			var textureFile = _parser.ReadAttributeString(page, "file");
			return Path.Combine(Path.GetDirectoryName(asset.RelativePath), textureFile);
		}

		/// <summary>
		///   Finds the texture name and writes it to the compiled asset file.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		private void ProcessTexture(Asset asset, BufferWriter buffer)
		{
			var path = GetTexturePath(asset);
			if (path == null)
				Log.Die("Could not retrieve texture path from font. Maybe more than one texture is used.");

			using (var texture = new Texture2DAsset(path) { Mipmaps = false })
				new Texture2DCompiler().CompileSingle(texture, buffer);
		}

		/// <summary>
		///   Processes the relevant common attributes.
		/// </summary>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		private void ProcessCommon(BufferWriter buffer)
		{
			var common = _parser.FindElement(_parser.Root, "common");
			buffer.WriteUInt16(_parser.ReadAttributeUInt16(common, "scaleW"));
			buffer.WriteUInt16(_parser.ReadAttributeUInt16(common, "scaleH"));
			buffer.WriteUInt16(_parser.ReadAttributeUInt16(common, "lineHeight"));
		}

		/// <summary>
		///   Processes the character information.
		/// </summary>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		private void ProcessCharacters(BufferWriter buffer)
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

			buffer.WriteUInt16((ushort)numGlyphs);
			buffer.WriteUInt16((ushort)lowestGlyphId);
			buffer.WriteUInt16((ushort)highestGlyphId);
			foreach (var glyph in _parser.FindElements(chars, "char"))
			{
				buffer.WriteUInt16((ushort)_parser.ReadAttributeUInt32(glyph, "id"));
				buffer.WriteUInt16(_parser.ReadAttributeUInt16(glyph, "width"));
				buffer.WriteUInt16(_parser.ReadAttributeUInt16(glyph, "height"));
				buffer.WriteInt16(_parser.ReadAttributeInt16(glyph, "xoffset"));
				buffer.WriteInt16(_parser.ReadAttributeInt16(glyph, "yoffset"));
				buffer.WriteInt16(_parser.ReadAttributeInt16(glyph, "xadvance"));
				buffer.WriteUInt16(_parser.ReadAttributeUInt16(glyph, "x"));
				buffer.WriteUInt16(_parser.ReadAttributeUInt16(glyph, "y"));
			}
		}

		/// <summary>
		///   Processes the kerning information.
		/// </summary>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		private void ProcessKerning(BufferWriter buffer)
		{
			if (!_parser.HasElement(_parser.Root, "kernings"))
			{
				buffer.WriteUInt16(0);
				return;
			}

			var kernings = _parser.FindElement(_parser.Root, "kernings");
			var count = _parser.ReadAttributeUInt32(kernings, "count");

			if (count > UInt16.MaxValue)
				Log.Die("Too many kerning pairs: {0}", count);

			buffer.WriteUInt16((ushort)count);
			var kerningPairs = _parser.FindElements(kernings, "kerning")
									  .OrderBy(e => _parser.ReadAttributeUInt16(e, "first"))
									  .ThenBy(e => _parser.ReadAttributeUInt16(e, "first"));

			foreach (var kerning in kerningPairs)
			{
				buffer.WriteUInt16(_parser.ReadAttributeUInt16(kerning, "first"));
				buffer.WriteUInt16(_parser.ReadAttributeUInt16(kerning, "second"));
				buffer.WriteInt16(_parser.ReadAttributeInt16(kerning, "amount"));
			}
		}
	}
}