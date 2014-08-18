namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using CSharp;
	using Fonts;
	using Pegasus.Assets;
	using Platform.Logging;

	/// <summary>
	///     Compiles texture-based fonts.
	/// </summary>
	[UsedImplicitly]
	internal sealed class FontCompiler : AssetCompiler<FontAsset>
	{
		/// <summary>
		///     The freetype library instance that is to generate the font textures.
		/// </summary>
		private readonly FreeTypeLibrary _freeType = new FreeTypeLibrary();

		/// <summary>
		///     The parser that is used to parse the font definitions.
		/// </summary>
		private readonly ConfigurationFileParser _parser = new ConfigurationFileParser(new Dictionary<string, Func<string, object>>
		{
			{ "File", s => s },
			{ "Family", s => s },
			{ "Size", s => Int32.Parse(s) },
			{ "Aliased", s => Boolean.Parse(s) },
			{ "Bold", s => Boolean.Parse(s) },
			{ "Italic", s => Boolean.Parse(s) },
			{ "Characters", ParseCharacterRanges },
			{ "InvalidChar", s => s[0] }
		});

		/// <summary>
		///     A value indicating whether the font loader must be regenerated.
		/// </summary>
		private bool _regenerateFontLoader;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public FontCompiler()
		{
			SupportsMultithreading = false;
		}

		/// <summary>
		///     Parses the character range.
		/// </summary>
		/// <param name="range">The range that should be parsed.</param>
		private static IEnumerable<char> ParseCharacterRanges(string range)
		{
			Assert.ArgumentNotNullOrWhitespace(range);

			var ranges = range.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var r in ranges)
			{
				var pair = r.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
				Assert.That(pair.Length == 2, "Invalid character range '{0}'.", range);

				var begin = 0;
				var end = 0;
				if (!Int32.TryParse(pair[0], out begin) || !Int32.TryParse(pair[1], out end))
					Log.Die("Invalid character range '{0}'.", range);

				for (var i = begin; i <= end; ++i)
					yield return (char)i;
			}
		}

		/// <summary>
		///     Gets the path of the temporary font map file.
		/// </summary>
		/// <param name="asset">The asset the path should be returned for.</param>
		private static string GetFontMapPath(Asset asset)
		{
			return asset.RelativePathWithoutExtension + ".Fontmap.png";
		}

		/// <summary>
		///     Compiles all assets of the compiler's asset source type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		public override void Compile(IEnumerable<Asset> assets)
		{
			base.Compile(assets);

			if (_regenerateFontLoader)
				GenerateFontLoader(assets.OfType<FontAsset>().ToArray());
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		protected override void Compile(FontAsset asset, BufferWriter writer)
		{
			WriteAssetHeader(writer, (byte)AssetType.Font);
			_regenerateFontLoader = true;

			// Read the font configuration
			var configuration = _parser.Parse(asset.SourcePath);

			var fontFile = Path.Combine(asset.SourceDirectory, (string)configuration["File"]);
			var size = (int)configuration["Size"];
			var antialiased = !(bool)configuration["Aliased"];
			var renderMode = antialiased ? RenderMode.Antialiased : RenderMode.Aliased;
			var bold = (bool)configuration["Bold"];
			var italic = (bool)configuration["Italic"];
			var characters = (IEnumerable<char>)configuration["Characters"];
			var invalidChar = (char)configuration["InvalidChar"];

			Assert.That(!bold, "Bold fonts are currently not supported.");
			Assert.That(!italic, "Italic fonts are currently not supported.");

			// Verify that the font file actually exists
			if (!File.Exists(fontFile))
				Log.Die("Unable to locate '{0}'.", configuration["File"]);

			// Initialize the font data structures
			var fontPtr = _freeType.CreateFont(fontFile);
			using (var font = new Font(fontPtr, size, bold, italic, renderMode, characters, invalidChar))
			using (var fontMap = new FontMap(font, GetFontMapPath(asset)))
			{
				// Write the font map
				fontMap.Compile(writer);

				// Write the font metadata
				writer.WriteUInt16(font.LineHeight);

				// Write the glyph metadata
				writer.WriteUInt16((ushort)font.Glyphs.Count());

				var isFirst = true;
				foreach (var glyph in font.Glyphs)
				{
					// Glyphs are identified by their character ASCII id, except for the invalid character
					// that is guaranteed to by the first glyph, which must lie at index 0
					if (isFirst)
					{
						writer.WriteByte(0);
						isFirst = false;
					}
					else
						writer.WriteByte((byte)glyph.Character);

					// Write the font map texture coordinates in pixels
					var area = fontMap.GetGlyphArea(glyph.Character);
					writer.WriteInt16((short)area.Left);
					writer.WriteInt16((short)area.Top);
					writer.WriteUInt16((ushort)area.Width);
					writer.WriteUInt16((ushort)area.Height);

					// Write the glyph offsets
					writer.WriteInt16((short)glyph.OffsetX);
					writer.WriteInt16((short)(font.Baseline - glyph.OffsetY));
					writer.WriteInt16((short)glyph.AdvanceX);
				}

				// Write the kerning information, if any
				if (!font.HasKerning)
				{
					writer.WriteUInt16(0);
					return;
				}

				var pairs = (from left in font.Glyphs
							 from right in font.Glyphs
							 let offset = font.GetKerning(left, right)
							 where offset != 0
							 select new { Left = left, Right = right, Offset = offset }).ToArray();

				Assert.That(pairs.Length < UInt16.MaxValue, "Too many kerning pairs.");
				writer.WriteUInt16((ushort)pairs.Length);

				foreach (var pair in pairs)
				{
					writer.WriteUInt16(pair.Left.Character);
					writer.WriteUInt16(pair.Right.Character);
					writer.WriteInt16((short)pair.Offset);
				}
			}
		}

		/// <summary>
		///     Removes the compiled asset and all temporary files written by the compiler.
		/// </summary>
		/// <param name="asset">The asset that should be cleaned.</param>
		protected override void Clean(FontAsset asset)
		{
			File.Delete(Path.Combine(Configuration.TempDirectory, GetFontMapPath(asset)));
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public override void Dispose()
		{
			_freeType.Dispose();
		}

		/// <summary>
		///     Generates the font loader class.
		/// </summary>
		/// <param name="assets">The font assets that have been compiled.</param>
		private void GenerateFontLoader(FontAsset[] assets)
		{
			var writer = new CodeWriter();
			writer.WriterHeader();

			writer.AppendLine("namespace {0}", Configuration.AssetsProject.RootNamespace);
			writer.AppendBlockStatement(() =>
			{
				writer.AppendLine("using System;");
				writer.AppendLine("using Pegasus;");
				writer.AppendLine("using Pegasus.Framework.UserInterface;");
				writer.AppendLine("using Pegasus.Assets;");
				writer.AppendLine("using Pegasus.Platform.Logging;");
				writer.AppendLine("using Pegasus.Platform.Graphics;");
				writer.NewLine();

				writer.AppendLine("/// <summary>");
				writer.AppendLine("///     Provides a method to search for a font based on certain font settings.");
				writer.AppendLine("/// </summary>");
				writer.AppendLine("internal class FontLoader : IFontLoader");
				writer.AppendBlockStatement(() =>
				{
					writer.AppendLine("/// <summary>");
					writer.AppendLine("///     The assets manager that is used to load the fonts.");
					writer.AppendLine("/// </summary>");
					writer.AppendLine("private AssetsManager _assets;");

					writer.NewLine();
					writer.AppendLine("/// <summary>");
					writer.AppendLine("///     Initializes a new instance.");
					writer.AppendLine("/// </summary>");
					writer.AppendLine("/// <param name=\"assets\">The assets manager that should be used to load the fonts.</param>");
					writer.AppendLine("public FontLoader(AssetsManager assets)");
					writer.AppendBlockStatement(() =>
					{
						writer.AppendLine("Assert.ArgumentNotNull(assets);");
						writer.AppendLine("_assets = assets;");
					});

					writer.NewLine();
					writer.AppendLine("/// <summary>");
					writer.AppendLine("///     Sets the next font loader that is used to load the font if the current loader fails to");
					writer.AppendLine("///     load an appropriate font.");
					writer.AppendLine("/// </summary>");
					writer.AppendLine("public IFontLoader Next {{ private get; set; }}");

					writer.NewLine();
					writer.AppendLine("/// <summary>");
					writer.AppendLine("///     Gets the font matching the given font settings.");
					writer.AppendLine("/// </summary>");
					writer.AppendLine("/// <param name=\"fontFamily\">The family of the font that should be returned.</param>");
					writer.AppendLine("/// <param name=\"size\">The size of the font that should be returned.</param>");
					writer.AppendLine("/// <param name=\"bold\">Indicates whether the font should be bold.</param>");
					writer.AppendLine("/// <param name=\"italic\">Indicates whether the font should be italic.</param>");
					writer.AppendLine("/// <param name=\"aliased\">Indicates whether the font should be aliased.</param>");
					writer.AppendLine("public Font LoadFont(string fontFamily, int size, bool bold, bool italic, bool aliased)");

					writer.AppendBlockStatement(() =>
					{
						var fonts = assets.Select(font => _parser.Parse(font.SourcePath));

						writer.AppendLine("Assert.ArgumentNotNullOrWhitespace(fontFamily);");
						writer.NewLine();

						writer.AppendLine("AssetIdentifier<Font>? font = null;");
						writer.AppendLine("switch (fontFamily)");
						writer.AppendBlockStatement(() =>
						{
							foreach (var family in fonts.GroupBy(font => font["family"]))
							{
								writer.AppendLine("case \"{0}\":", family.Key);
								writer.IncreaseIndent();
								writer.AppendLine("switch (size)");
								writer.AppendBlockStatement(() =>
								{
									foreach (var size in family.GroupBy(font => font["size"]))
									{
										writer.AppendLine("case {0}:", size.Key);
										writer.IncreaseIndent();
										foreach (var font in size)
										{
											var sourceFile = (string)font[ConfigurationFileParser.SourceFile];
											var asset = assets.Single(a => a.SourcePath == sourceFile);

											writer.AppendLine("if (bold == {0} && italic == {1} && aliased == {2})",
												((bool)font["bold"]).ToString().ToLower(),
												((bool)font["italic"]).ToString().ToLower(),
												((bool)font["aliased"]).ToString().ToLower());
											writer.IncreaseIndent();
											writer.AppendLine("font = {0}.{1}.{2};",
												Configuration.AssetsProject.RootNamespace,
												Path.GetDirectoryName(asset.RelativePath).Replace("/", "."),
												asset.FileNameWithoutExtension.Replace(" ", ""));
											writer.DecreaseIndent();
										}
										writer.AppendLine("break;");
										writer.DecreaseIndent();
									}
								});
								writer.AppendLine("break;");
								writer.DecreaseIndent();
							}
						});

						writer.NewLine();
						writer.AppendLine("if (font == null && Next != null)");
						writer.IncreaseIndent();
						writer.AppendLine("return Next.LoadFont(fontFamily, size, bold, italic, aliased);");
						writer.DecreaseIndent();
						writer.AppendLine("else if (font == null)");
						writer.IncreaseIndent();
						writer.AppendLine("Log.Die(\"Unable to find a font with family = '{{0}}', size = {{1}}, bold = {{2}}, " +
										  "italic = {{3}}, aliased = {{4}}.\", fontFamily, size, bold, italic, aliased);");
						writer.DecreaseIndent();

						writer.NewLine();
						writer.AppendLine("return _assets.Load(font.Value);");
					});
				});
			});

			File.WriteAllText(Configuration.CSharpFontLoaderFile, writer.ToString());
		}
	}
}