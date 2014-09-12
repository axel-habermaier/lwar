namespace Pegasus.Scripting.Parsing.Combinators
{
	using System;
	using System.Linq;
	using Framework.UserInterface.Input;

	/// <summary>
	///     Parses a configurable input of the form 'Key.A', 'Key.A+Control', 'Mouse.Left+Alt+Shift', etc.
	/// </summary>
	internal class ConfigurableInputParser : CombinedParser<ConfigurableInput>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ConfigurableInputParser()
		{
			var ws = ~Parsers.WhiteSpaces;
			var dot = Character('.') + ws;
			var plus = Character('+') + ws;
			var openBracket = ~(Character('[') + ws);
			var closeBracket = ~Character(']'); // no end ws!

			var keySelector = String("Key", ignoreCase: true) + ws + ~dot;
			var mouseSelector = String("Mouse", ignoreCase: true) + ws + ~dot;

			var key = ~keySelector + EnumerationLiteral<Key>(ignoreCase: true) + ws;
			var mouseButton = ~mouseSelector + EnumerationLiteral<MouseButton>(ignoreCase: true) + ws;
			var modifier = EnumerationLiteral<KeyModifiers>(ignoreCase: true) + ws;

			var modifiers = (~plus + SeparatedBy1(modifier, plus))
				.Apply(modifierList => modifierList.Aggregate(KeyModifiers.None, (current, m) => current | m))
				.Optional(KeyModifiers.None);

			var keyInput = key.Apply(k => new ConfigurableInput(k, KeyModifiers.None));
			var mouseInput = mouseButton.Apply(m => new ConfigurableInput(m, KeyModifiers.None));
			var modifiedInput = Pipe(keyInput | mouseInput, modifiers, (input, m) => input.WithModifiers(m));

			Parser = Between(modifiedInput | modifiedInput, openBracket, closeBracket);
		}
	}
}