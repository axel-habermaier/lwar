using System;

namespace Pegasus.Framework.Platform.Input
{
	using System.Linq;
	using Scripting.Parsing;

	/// <summary>
	///   Parses input trigger expressions. The precedence of the three operators '+', '&', and '|' is encoded in the grammar,
	///   where the precedence of '+' is higher than the precedence of '&', which is higher than the precedence of '|'. All
	///   three operators are left-associative. As the parser rules correspond to C#'s operator rules, an expression
	///   parsed by the parser describes the same input trigger as if it had been written directly in C#.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class InputTriggerParser<TUserState> : CombinedParser<InputTrigger, TUserState>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public InputTriggerParser()
		{
			var ws = ~WhiteSpaces;
			var comma = ~(Character(',') + ws);
			var openParen = ~(Character('(') + ws);
			var closeParen = ~(Character(')') + ws);
			var openBracket = ~(Character('[') + ws);
			var closeBracket = ~Character(']'); // no end ws!
			var chordOnceOperator = Character('+') + ws;
			var chordOperator = Character('&') + ws;
			var aliasOperator = Character('|') + ws;

			// Define the parsers for mouse and key triggers (the parser does not support scan code triggers or cvar triggers)
			var keyTriggerType = EnumerationLiteral<KeyTriggerType>() + ws;
			var mouseTriggerType = EnumerationLiteral<MouseTriggerType>() + ws;

			var key = EnumerationLiteral<Key>() + ws;
			var mouseButton = EnumerationLiteral<MouseButton>() + ws;

			var keyTriggerParams = Pipe(key, comma + keyTriggerType, (k, t) => (InputTrigger)new KeyTrigger(t, k));
			var mouseTriggerParams = Pipe(mouseButton, comma + mouseTriggerType, (m, t) => (InputTrigger)new MouseTrigger(t, m));

			var keySelector = String("Key") + ws + openParen;
			var mouseSelector = String("Mouse") + ws + openParen;

			var keyTrigger = Between(keyTriggerParams, keySelector, closeParen);
			var mouseTrigger = Between(mouseTriggerParams, mouseSelector, closeParen);

			// Define the parsers for binary triggers; the grammar is as follows, where the operator precedence is encoded
			// into the grammar, with all operators being left-associative
			// trigger			::= aliasTrigger
			// aliasTrigger		::= chordTrigger ('|' chordTrigger)*
			// chordTrigger		::= chordOnceTrigger ('&' chordOnceTrigger)*
			// chordOnceTrigger ::= primaryTrigger ('+' primaryTrigger)*
			// primaryTrigger	::= '(' trigger ')' | keyTrigger | mouseTrigger

			// As trigger and primaryTrigger are mutually recursive, we use a reference parser for trigger to break the cycle
			var trigger = Reference<InputTrigger>();
			var primaryTrigger = ((openParen + trigger + closeParen) | keyTrigger | mouseTrigger) + ws;
			var chordOnceTrigger = CreateBinaryTriggerParser(primaryTrigger, chordOnceOperator, BinaryInputTriggerType.ChordOnce);
			var chordTrigger = CreateBinaryTriggerParser(chordOnceTrigger, chordOperator, BinaryInputTriggerType.Chord);
			var aliasTrigger = CreateBinaryTriggerParser(chordTrigger, aliasOperator, BinaryInputTriggerType.Alias);

			trigger.ReferencedParser = aliasTrigger;
			Parser = Between(trigger, openBracket, closeBracket);
		}

		/// <summary>
		///   Creates a parser for a binary trigger.
		/// </summary>
		/// <param name="operandParser">The parser for the binary trigger operands.</param>
		/// <param name="operatorParser">The parser for the binary trigger operator.</param>
		/// <param name="triggerType">The type of the binary input trigger.</param>
		private static Parser<InputTrigger, TUserState> CreateBinaryTriggerParser(Parser<InputTrigger, TUserState> operandParser,
																			Parser<char, TUserState> operatorParser,
																			BinaryInputTriggerType triggerType)
		{
			return SeparatedBy1(operandParser, operatorParser)
					   .Apply(triggers =>
							  triggers.Aggregate((binaries, next) => new BinaryInputTrigger(triggerType, binaries, next)))
				   + ~WhiteSpaces;
		}
	}
}