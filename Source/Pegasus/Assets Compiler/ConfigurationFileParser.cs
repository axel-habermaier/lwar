namespace Pegasus.AssetsCompiler
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Platform.Logging;
	using Utilities;

	/// <summary>
	///     Parses configuration files similar to Window's .ini file format.
	/// </summary>
	internal class ConfigurationFileParser
	{
		/// <summary>
		///     The key that is used to store the path of the source file in the configuration dictionary.
		/// </summary>
		public const string SourceFile = "___sourceFileName";

		/// <summary>
		///     Indicates the required keys and the function that is used to convert their values into .NET objects.
		/// </summary>
		private readonly Dictionary<string, Func<string, object>> _requiredKeys;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="requiredKeys">
		///     Indicates the required keys and the function that should be used to convert their values
		///     into .NET objects.
		/// </param>
		public ConfigurationFileParser(Dictionary<string, Func<string, object>> requiredKeys)
		{
			Assert.ArgumentNotNull(requiredKeys);
			_requiredKeys = requiredKeys.Select(pair => new { Key = pair.Key.ToLower(), pair.Value })
										.ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		/// <summary>
		///     Parses the given configuration file, validates that all required keys are present with correctly types values and
		///     returns all key-value pairs.
		/// </summary>
		/// <param name="file">The configuration file that should be parsed.</param>
		public Dictionary<string, object> Parse(string file)
		{
			Assert.ArgumentNotNullOrWhitespace(file);

			var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			var lineNumber = 0;

			result.Add(SourceFile, file);

			foreach (var line in File.ReadAllLines(file))
			{
				++lineNumber;

				// Ignore empty lines
				if (String.IsNullOrWhiteSpace(line))
					continue;

				// Ignore comment lines
				if (line.Trim().StartsWith(";"))
					continue;

				// Find the equals sign
				var equalSign = line.IndexOf("=", StringComparison.Ordinal);
				if (equalSign == -1)
					Log.Die("Line {0} does not contain an equal sign '='.", lineNumber);

				// Extract the key
				var key = line.Substring(0, equalSign).Trim();
				Func<string, object> converter = null;
				if (!_requiredKeys.TryGetValue(key.ToLower(), out converter))
					Log.Die("Line {0} contains unexpected key '{1}'.", lineNumber, key);

				if (result.ContainsKey(key))
					Log.Die("Unexpected redefinition of key '{0}' in line {1}.", key, lineNumber);

				// Extract the value
				var valueString = line.Substring(equalSign + 1).Trim();
				object value = null;
				try
				{
					value = converter(valueString);
				}
				catch (Exception e)
				{
					Log.Die("Failed to parse value of key '{0}' in line {1}: {2}", key, lineNumber, e.Message);
				}

				// Add the key-value-pair to the result
				result.Add(key, value);
			}

			return result;
		}
	}
}