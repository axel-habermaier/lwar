using System;

namespace Pegasus.AssetsCompiler
{
    using System.IO;
    using System.Linq;
    using Framework;
    using Framework.Platform;

	/// <summary>
    ///   Processes fonts.
    /// </summary>
    public sealed class FontProcessor : AssetProcessor
    {
        /// <summary>
        ///   Returns true if the processor can process a file with the given extension.
        /// </summary>
        /// <param name="extension">The extension of the file that should be processed.</param>
        public override bool CanProcess(string extension)
        {
            return extension == ".fnt";
        }

        /// <summary>
        ///   Processes the given file, writing the compiled output to the given target destination.
        /// </summary>
        /// <param name="source">The source file that should be processed.</param>
        /// <param name="sourceRelative">The path to the source file relative to the Assets root directory.</param>
        /// <param name="writer">The writer that should be used to write the compiled asset file.</param>
		public override void Process(string source, string sourceRelative, BufferWriter writer)
        {
            Assert.ArgumentNotNullOrWhitespace(source, () => source);
            Assert.ArgumentNotNull(writer, () => writer);

            var parser = new XmlParser(source);
            ProcessCommon(parser, writer);
            ProcessCharacters(parser, writer);
            ProcessKerning(parser, writer);
            ProcessTexture(parser, writer, Path.GetDirectoryName(sourceRelative));
        }

        /// <summary>
        ///   Finds the texture name and writes it to the compiled asset file.
        /// </summary>
		private void ProcessTexture(XmlParser parser, BufferWriter writer, string path)
        {
            var pages = parser.FindElement(parser.Root, "pages");
            var pagesList = parser.FindElements(pages, "page");
            if (pagesList.Count() != 1)
                throw new InvalidOperationException("Expected exactly one page.");

            var page = pagesList.Single();
            var textureFile = parser.ReadAttributeString(page, "file");
            writer.WriteString(Path.Combine(path, Path.GetFileNameWithoutExtension(textureFile)));
        }

        /// <summary>
        ///   Processes the relevant common attributes.
        /// </summary>
		private void ProcessCommon(XmlParser parser, BufferWriter writer)
        {
            var common = parser.FindElement(parser.Root, "common");
            writer.WriteUInt16(parser.ReadAttributeUInt16(common, "scaleW"));
            writer.WriteUInt16(parser.ReadAttributeUInt16(common, "scaleH"));
            writer.WriteUInt16(parser.ReadAttributeUInt16(common, "lineHeight"));
        }

        /// <summary>
        ///   Processes the character information.
        /// </summary>
		private void ProcessCharacters(XmlParser parser, BufferWriter writer)
        {
            var chars = parser.FindElement(parser.Root, "chars");
            var numGlyphs = parser.ReadAttributeUInt32(chars, "count");

            var lowestGlyphId = parser.FindElements(chars, "char").Min(e => parser.ReadAttributeUInt32(e, "id"));
            var highestGlyphId = parser.FindElements(chars, "char").Max(e => parser.ReadAttributeUInt32(e, "id"));

            if (lowestGlyphId > UInt16.MaxValue)
                throw new InvalidOperationException(String.Format("Glyph id exceeds limits: {0}", lowestGlyphId));

            if (highestGlyphId > UInt16.MaxValue)
                throw new InvalidOperationException(String.Format("Glyph id exceeds limits: {0}", highestGlyphId));

            if (parser.FindElements(chars, "char").All(e => parser.ReadAttributeUInt32(e, "id") != '.'))
                throw new InvalidOperationException("The font must support the dot '.' character.");

            writer.WriteUInt16((ushort)numGlyphs);
            writer.WriteUInt16((ushort)lowestGlyphId);
            writer.WriteUInt16((ushort)highestGlyphId);
            foreach (var glyph in parser.FindElements(chars, "char"))
            {
                writer.WriteUInt16((ushort)parser.ReadAttributeUInt32(glyph, "id"));
                writer.WriteUInt16(parser.ReadAttributeUInt16(glyph, "width"));
                writer.WriteUInt16(parser.ReadAttributeUInt16(glyph, "height"));
                writer.WriteInt16(parser.ReadAttributeInt16(glyph, "xoffset"));
                writer.WriteInt16(parser.ReadAttributeInt16(glyph, "yoffset"));
                writer.WriteInt16(parser.ReadAttributeInt16(glyph, "xadvance"));
                writer.WriteUInt16(parser.ReadAttributeUInt16(glyph, "x"));
                writer.WriteUInt16(parser.ReadAttributeUInt16(glyph, "y"));
            }
        }

        /// <summary>
        ///   Processes the kerning information.
        /// </summary>
		private void ProcessKerning(XmlParser parser, BufferWriter writer)
        {
            if (!parser.HasElement(parser.Root, "kernings"))
            {
                writer.WriteUInt16(0);
                return;
            }

            var kernings = parser.FindElement(parser.Root, "kernings");
            var count = parser.ReadAttributeUInt32(kernings, "count");

            if (count > UInt16.MaxValue)
                throw new InvalidOperationException(String.Format("Too many kerning pairs: {0}", count));

            writer.WriteUInt16((ushort)count);
            var kerningPairs = parser.FindElements(kernings, "kerning")
                                     .OrderBy(e => parser.ReadAttributeUInt16(e, "first"))
                                     .ThenBy(e => parser.ReadAttributeUInt16(e, "first"));

            foreach (var kerning in kerningPairs)
            {
                writer.WriteUInt16(parser.ReadAttributeUInt16(kerning, "first"));
                writer.WriteUInt16(parser.ReadAttributeUInt16(kerning, "second"));
                writer.WriteInt16(parser.ReadAttributeInt16(kerning, "amount"));
            }
        }
    }
}