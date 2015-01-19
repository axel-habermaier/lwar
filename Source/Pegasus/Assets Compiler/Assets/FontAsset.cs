namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Linq;
	using Utilities;

	/// <summary>
	///     Represents a font asset that requires compilation.
	/// </summary>
	internal class FontAsset : Asset
	{
		/// <summary>
		///     The runtime name of the asset.
		/// </summary>
		private readonly string _runtimeName;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="metadata">The metadata of the asset.</param>
		public FontAsset(XElement metadata)
			: base(metadata, "File")
		{
			Family = GetStringMetadata("Family");
			Size = GetIntMetadata("Size");
			Italic = GetBoolMetadata("Italic");
			Bold = GetBoolMetadata("Bold");
			Aliased = GetBoolMetadata("Aliased");
			_runtimeName = GetStringMetadata("RuntimeName");

			var invalidChar = GetStringMetadata("InvalidChar");
			if (invalidChar.Length != 1)
				Log.Die("Invalid string given for InvalidChar attribute: '{0}'.", invalidChar);

			InvalidChar = invalidChar[0];
			CharacterRange = ParseCharacterRanges(GetStringMetadata("Characters"));
		}

		/// <summary>
		///     Gets the font family.
		/// </summary>
		public string Family { get; private set; }

		/// <summary>
		///     Gets the size of the font.
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the font is italic.
		/// </summary>
		public bool Italic { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the font is bold.
		/// </summary>
		public bool Bold { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the font is aliased.
		/// </summary>
		public bool Aliased { get; private set; }

		/// <summary>
		///     Gets the character that should be printed when an invalid character is detected.
		/// </summary>
		public char InvalidChar { get; private set; }

		/// <summary>
		///     Gets the range of characters supported by the font.
		/// </summary>
		public IEnumerable<char> CharacterRange { get; private set; }

		/// <summary>
		///     Gets the runtime type (without the namespace) of the asset.
		/// </summary>
		public override string RuntimeType
		{
			get { return "Pegasus.UserInterface.Font"; }
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public override byte AssetType
		{
			get { return 3; }
		}

		/// <summary>
		///     Gets the runtime name of the asset.
		/// </summary>
		public override string RuntimeName
		{
			get { return _runtimeName; }
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