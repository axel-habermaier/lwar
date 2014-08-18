namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using Scripting.Parsing;
	using Scripting.Parsing.BasicParsers;

	/// <summary>
	///     Parses the Xaml data binding markup extension syntax.
	/// </summary>
	internal class DataBindingParser : CombinedParser<XamlDataBinding>
	{
		/// <summary>
		///     Stores the parse results.
		/// </summary>
		private XamlDataBinding _dataBinding = new XamlDataBinding();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public DataBindingParser()
		{
			var ws = ~new WhiteSpacesParser();
			var comma = ~(Character(',') + ws);
			var openBrace = ~(Character('{') + ws);
			var closeBrace = ~(Character('}') + ws);

			var path = String(c => Char.IsLetter(c) || c == '_', c => Char.IsLetterOrDigit(c) || c == '_' || c == '.', "path");
			var pathProperty = ~(String("Path") + ~ws + ~Character('=') + ws).Optional("") + path + ws;

			var converter = Between(String(c => c != '}', "Converter"), openBrace, closeBrace);
			var converterProperty = ~(String("Converter") + ~ws + ~Character('=') + ws) + converter + ws;

			var mode = String("OneWayToSource") | String("OneWay") | String("TwoWay");
			var modeProperty = ~(String("Mode") + ~ws + ~Character('=') + ws) + mode + ws;

			var property = converterProperty.Apply(p => _dataBinding.Converter = p) |
						   modeProperty.Apply(p => _dataBinding.BindingMode = p) |
						   pathProperty.Apply(p => _dataBinding.Path = p);

			var properties = SeparatedBy1(property, comma);
			Parser = Between(properties, openBrace + String("Binding") + ws, closeBrace).Apply(_ => _dataBinding);
		}
	}
}