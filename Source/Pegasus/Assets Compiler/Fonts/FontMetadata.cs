namespace Pegasus.AssetsCompiler.Fonts
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Platform.Logging;
	using Utilities;

	/// <summary>
	///     Provides metadata about a font.
	/// </summary>
	internal struct FontMetadata
	{
		/// <summary>
		///     The parser that is used to parse the font definitions.
		/// </summary>
		private static readonly ConfigurationFileParser Parser = new ConfigurationFileParser(new Dictionary<string, Func<string, object>>
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
		///     Initializes a new instance.
		/// </summary>
		/// <param name="sourceDirectory">The assets source directory.</param>
		/// <param name="fontFile">The path to the font file relative to the source directory.</param>
		public FontMetadata(string sourceDirectory, string fontFile)
			: this()
		{
			Assert.ArgumentNotNullOrWhitespace(sourceDirectory);
			Assert.ArgumentNotNullOrWhitespace(fontFile);

			// Read the font configuration
			var configuration = Parser.Parse(fontFile);

			FontFile = Path.Combine(sourceDirectory, (string)configuration["File"]);
			Family = (string)configuration["Family"];
			Size = (int)configuration["Size"];
			Aliased = (bool)configuration["Aliased"];
			Bold = (bool)configuration["Bold"];
			Italic = (bool)configuration["Italic"];
			Characters = (IEnumerable<char>)configuration["Characters"];
			InvalidChar = (char)configuration["InvalidChar"];
			SourceFile = (string)configuration[ConfigurationFileParser.SourceFile];

			Assert.That(!Bold, "Bold fonts are currently not supported.");
			Assert.That(!Italic, "Italic fonts are currently not supported.");
		}

		/// <summary>
		///     Gets the source file that declares the font metadata.
		/// </summary>
		public string SourceFile { get; private set; }

		/// <summary>
		///     Gets the path to the file containing the font definition.
		/// </summary>
		public string FontFile { get; private set; }

		/// <summary>
		///     Gets the size of the font characters.
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		///     Gets the font family.
		/// </summary>
		public string Family { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the font is anti-aliased.
		/// </summary>
		public bool Aliased { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the font is bold.
		/// </summary>
		public bool Bold { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the font is italic.
		/// </summary>
		public bool Italic { get; private set; }

		/// <summary>
		///     Gets the character that should be used to represent an invalid character.
		/// </summary>
		public char InvalidChar { get; private set; }

		/// <summary>
		///     Gets the characters supported by the font.
		/// </summary>
		public IEnumerable<char> Characters { get; private set; }

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

				int begin;
				var end = 0;
				if (!Int32.TryParse(pair[0], out begin) || !Int32.TryParse(pair[1], out end))
					Log.Die("Invalid character range '{0}'.", range);

				for (var i = begin; i <= end; ++i)
					yield return (char)i;
			}
		}
	}
}