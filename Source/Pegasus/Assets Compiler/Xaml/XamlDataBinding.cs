namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Text.RegularExpressions;

	/// <summary>
	///     Stores the parsed information of a Xaml data binding.
	/// </summary>
	internal struct XamlDataBinding
	{
		/// <summary>
		///     Gets or sets the property path of the data binding.
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		///     Gets or sets the name of the value converter that is used by the binding.
		/// </summary>
		public string Converter { get; set; }

		/// <summary>
		///     Gets or sets the binding mode of the data binding.
		/// </summary>
		public string BindingMode { get; set; }

		/// <summary>
		///     Gets or sets the fallback value.
		/// </summary>
		public string FallbackValue { get; set; }

		/// <summary>
		///     Parses the binding string.
		/// </summary>
		/// <param name="binding">The binding string that should be parsed.</param>
		public static XamlDataBinding Parse(string binding)
		{
			var regex =
				new Regex(
					@"\{Binding ((Path=(?<path>.+?))|(?<path>.+?))(, Mode=(?<mode>.+?))?(, FallbackValue='(?<fallbackValue>.+?)')?(, Converter=\{(?<converter>.+?)\})?\}");
			var match = regex.Match(binding);

			return new XamlDataBinding
			{
				Path = match.Groups["path"].Value,
				Converter = match.Groups["converter"].Success ? match.Groups["converter"].Value : null,
				BindingMode = match.Groups["mode"].Success ? match.Groups["mode"].Value : null,
				FallbackValue = match.Groups["fallbackValue"].Success ? ("\"" + match.Groups["fallbackValue"].Value + "\"") : null,
			};
		}
	}
}