﻿using System;

namespace Pegasus.Framework.Scripting.Parsing
{
	using System.Collections.Generic;
	using BasicParsers;
	using Combinators;

	/// <summary>
	///   An abstract base class for the implementation of basic parsers and combined parsers.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser result.</typeparam>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public abstract class Parser<TResult, TUserState>
	{
		#region Parse methods

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="input">The input string that should be parsed.</param>
		public Reply<TResult> Parse(string input)
		{
			var inputStream = new InputStream<TUserState>(input);
			var reply = Parse(inputStream);

			if (reply.Status != ReplyStatus.Success)
				reply.GenerateErrorMessage(inputStream);

			return reply;
		}

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public abstract Reply<TResult> Parse(InputStream<TUserState> inputStream);

		#endregion

		#region Parser implementation helper functions

		/// <summary>
		///   Creates a successful parser reply for the given result.
		/// </summary>
		/// <param name="result">The result that should be copied to the reply.</param>
		protected Reply<TResult> Success(TResult result)
		{
			return new Reply<TResult>(result);
		}

		/// <summary>
		///   Creates an error parser reply for an unexpected end of input.
		/// </summary>
		/// <param name="message">An optional additional message that should be included.</param>
		protected Reply<TResult> UnexpectedEndOfInput(string message = null)
		{
			ErrorMessageList errors;
			if (!string.IsNullOrWhiteSpace(message))
				errors = new ErrorMessageList(ErrorMessage.UnexpectedEndOfInput, new ErrorMessage(ErrorType.Message, message));
			else
				errors = new ErrorMessageList(ErrorMessage.UnexpectedEndOfInput);

			return new Reply<TResult>(ReplyStatus.Error, errors);
		}

		/// <summary>
		///   Creates an error parser reply for an expected input.
		/// </summary>
		/// <param name="message">A description of the expected input.</param>
		protected Reply<TResult> Expected(string message)
		{
			return new Reply<TResult>(ReplyStatus.Error, new ErrorMessage(ErrorType.Expected, message));
		}

		/// <summary>
		///   Creates an error parser reply for an uncategorized parser message.
		/// </summary>
		/// <param name="message">The message that should be returned.</param>
		protected Reply<TResult> Message(string message)
		{
			return new Reply<TResult>(ReplyStatus.Error, new ErrorMessage(ErrorType.Message, message));
		}

		/// <summary>
		///   Forwards the error message of another parser.
		/// </summary>
		/// <param name="reply">The reply that should be forwarded.</param>
		/// <param name="message">An optional additional message that should be included.</param>
		protected Reply<TResult> ForwardError<T>(Reply<T> reply, string message = null)
		{
			var errors = reply.Errors;
			if (!string.IsNullOrWhiteSpace(message))
				errors = new ErrorMessageList(errors, new ErrorMessage(ErrorType.Message, message));

			return new Reply<TResult>(reply.Status, errors);
		}

		/// <summary>
		///   Merges the two given error message lists.
		/// </summary>
		/// <param name="first">The first error message list that should be merged.</param>
		/// <param name="second">The second error message list that should be merged.</param>
		protected Reply<TResult> MergeErrors(ErrorMessageList first, ErrorMessageList second)
		{
			return new Reply<TResult>(ReplyStatus.Error, new ErrorMessageList(first, second));
		}

		/// <summary>
		///   Returns a reply containing the given error messages.
		/// </summary>
		/// <param name="errors">The error messages that should be contained in the reply.</param>
		protected Reply<TResult> Errors(params ErrorMessage[] errors)
		{
			return new Reply<TResult>(ReplyStatus.Error, new ErrorMessageList(errors));
		}

		#endregion

		#region Default parser instances

		/// <summary>
		///   Parses a single digit character.
		/// </summary>
		protected static readonly Parser<char, TUserState> Digit = new DigitParser<TUserState>();

		/// <summary>
		///   Parses a Boolean value.
		/// </summary>
		protected static readonly Parser<bool, TUserState> Boolean = new BooleanParser<TUserState>();

		/// <summary>
		///   Parses a 32-bit floating point number.
		/// </summary>
		protected static readonly Parser<float, TUserState> Float32 = new Float32Parser<TUserState>();

		/// <summary>
		///   Parses a 64-bit floating point number.
		/// </summary>
		protected static readonly Parser<double, TUserState> Float64 = new Float64Parser<TUserState>();

		/// <summary>
		///   Parses a signed 32-bit integer.
		/// </summary>
		protected static readonly Parser<int, TUserState> Int32 = new Int32Parser<TUserState>();

		/// <summary>
		///   Parses an unsigned 32-bit integer.
		/// </summary>
		protected static readonly Parser<uint, TUserState> UInt32 = new UInt32Parser<TUserState>();

		/// <summary>
		///   Parses a signed 64-bit integer.
		/// </summary>
		protected static readonly Parser<long, TUserState> Int64 = new Int64Parser<TUserState>();

		/// <summary>
		///   Parses an unsigned 64-bit integer.
		/// </summary>
		protected static readonly Parser<ulong, TUserState> UInt64 = new UInt64Parser<TUserState>();

		/// <summary>
		///   Parses a signed 16-bit integer.
		/// </summary>
		protected static readonly Parser<short, TUserState> Int16 = new Int16Parser<TUserState>();

		/// <summary>
		///   Parses an unsigned 16-bit integer.
		/// </summary>
		protected static readonly Parser<ushort, TUserState> UInt16 = new UInt16Parser<TUserState>();

		/// <summary>
		///   Parses a signed 8-bit integer.
		/// </summary>
		protected static readonly Parser<sbyte, TUserState> Int8 = new Int8Parser<TUserState>();

		/// <summary>
		///   Parses an unsigned 8-bit integer.
		/// </summary>
		protected static readonly Parser<byte, TUserState> UInt8 = new UInt8Parser<TUserState>();

		/// <summary>
		///   Parses a single letter character.
		/// </summary>
		protected static readonly Parser<char, TUserState> Letter = new LetterParser<TUserState>();

		/// <summary>
		///   Parses the end of the input.
		/// </summary>
		protected static readonly Parser<None, TUserState> EndOfInput = new EndOfInputParser<TUserState>();

		/// <summary>
		///   Parses zero or more white space characters.
		/// </summary>
		protected static readonly Parser<None, TUserState> WhiteSpaces = new WhiteSpacesParser<TUserState>();

		/// <summary>
		///   Parses one or more white space characters.
		/// </summary>
		protected static readonly Parser<None, TUserState> WhiteSpaces1 = new WhiteSpaces1Parser<TUserState>();

		/// <summary>
		///   Parses a string literal enclosed in double quotes. Double quotes can be used inside the string literal if they are
		///   escaped by a backslash '\"'.
		/// </summary>
		protected static readonly Parser<string, TUserState> QuotedStringLiteral = new QuotedStringParser<TUserState>();

		#endregion

		#region Basic parsers convenience functions

		/// <summary>
		///   Parses the given character.
		/// </summary>
		/// <param name="character">The character that should be parsed.</param>
		protected static Parser<char, TUserState> Character(char character)
		{
			return new CharacterParser<TUserState>(character);
		}

		/// <summary>
		///   Applies the referenced parser whenever it should parse any input and returns the referenced parser's result. This
		///   parser kind can be used to break cyclic parser dependencies or to optimize the construction of larger parsers that
		///   have certain configurable parts.
		/// </summary>
		/// <typeparam name="T">The type of the result returned by the referenced parser.</typeparam>
		protected static ReferenceParser<T, TUserState> Reference<T>()
		{
			return new ReferenceParser<T, TUserState>();
		}

		/// <summary>
		///   Parses a string that satisfies the given predicates. The parser fails if not at least one character can be parsed.
		/// </summary>
		/// <param name="firstCharacter">The predicate that the first character of the parsed string must satisfy.</param>
		/// <param name="otherCharacters">The predicate that all but the first character of the parsed string must satisfy.</param>
		/// <param name="description">A description describing the expected input in the case of a parser error.</param>
		protected static Parser<string, TUserState> String(Func<char, bool> firstCharacter, Func<char, bool> otherCharacters,
														   string description)
		{
			return new StringParser<TUserState>(firstCharacter, otherCharacters, description);
		}

		/// <summary>
		///   Parses a string that satisfies the given predicate. The parser fails if not at least one character can be parsed.
		/// </summary>
		/// <param name="characters">The predicate that all characters of the parsed string must satisfy.</param>
		/// <param name="description">A description describing the expected input in the case of a parser error.</param>
		protected static Parser<string, TUserState> String(Func<char, bool> characters, string description)
		{
			return new StringParser<TUserState>(characters, description);
		}

		/// <summary>
		///   Parses the given string.
		/// </summary>
		/// <param name="str">The string that should be parsed.</param>
		protected static Parser<string, TUserState> String(string str)
		{
			return new SkipStringParser<TUserState>(str);
		}

		/// <summary>
		///   Parses an enumeration literal.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration literal that should be parsed.</typeparam>
		protected static Parser<TEnum, TUserState> EnumerationLiteral<TEnum>()
			where TEnum : struct
		{
			return new EnumerationLiteralParser<TEnum, TUserState>();
		}

		#endregion

		#region Combinator convenience functions

		/// <summary>
		///   Applies the parser and replaces the parser's error message by the given description in case of failure. The
		///   given description is returned as an expected error.
		/// </summary>
		/// <param name="description">The description that should be returned in the case of failure.</param>
		public Parser<TResult, TUserState> Named(string description)
		{
			return new DescriptionParser<TResult, TUserState>(this, description);
		}

		/// <summary>
		///   Applies the parser and returns the result if it succeeds. If it does not succeed, the given default element is
		///   returned and the optional parser is successful anyway.
		/// </summary>
		/// <param name="defaultValue">The default value that should be returned when the given parser fails.</param>
		public Parser<TResult, TUserState> Optional(TResult defaultValue)
		{
			return new OptionalParser<TResult, TUserState>(this, defaultValue);
		}

		/// <summary>
		///   Applies the parser and passes the parser's result to the given function. The value returned by the function
		///   is returned as the reply of the apply parser.
		/// </summary>
		/// <typeparam name="TFuncResult">The type of the function's result.</typeparam>
		/// <param name="function">The function that is applied to the parser's result.</param>
		public Parser<TFuncResult, TUserState> Apply<TFuncResult>(Func<TResult, TFuncResult> function)
		{
			return new ApplyParser<TResult, TFuncResult, TUserState>(this, function);
		}

		/// <summary>
		///   Applies the given parser zero or more times and returns the parser's results as a list in the order of occurrence.
		///   The many parser parses as many occurrences of the given parser as possible. At the end of the sequence the given
		///   parser must fail without consuming any input; otherwise, the many parser fails with the error returned by the given
		///   parser.
		/// </summary>
		/// <typeparam name="T">The type of the given parser's result.</typeparam>
		/// <param name="parser">The parser that should be applied zero or more times.</param>
		protected static Parser<T[], TUserState> Many<T>(Parser<T, TUserState> parser)
		{
			return new ManyParser<T, TUserState>(parser);
		}

		/// <summary>
		///   Applies the given parser one or more times and returns the parser's results as a list in the order of occurrence.
		///   The many parser parses as many occurrences of the given parser as possible. At the end of the sequence the given
		///   parser must fail without consuming any input; otherwise, the many parser fails with the error returned by the given
		///   parser.
		/// </summary>
		/// <typeparam name="T">The type of the given parser's result.</typeparam>
		/// <param name="parser">The parser that should be applied zero or more times.</param>
		protected static Parser<T[], TUserState> Many1<T>(Parser<T, TUserState> parser)
		{
			return new Many1Parser<T, TUserState>(parser);
		}

		/// <summary>
		///   Attempts to apply the given parser, returning its reply on success. If the parser fails non-fatally, however, it
		///   resets the input stream state to where it was when the parser was applied. Therefore, the attempt parser implements a
		///   backtracking mechanism in the sense that the attemt parser fails without consuming input whenever the given parser
		///   fails with or without consuming input.
		/// </summary>
		/// <typeparam name="T">The type of the given parser's result.</typeparam>
		/// <param name="parser">The parser that should be attempted.</param>
		protected static Parser<T, TUserState> Attempt<T>(Parser<T, TUserState> parser)
		{
			return new AttemptParser<T, TUserState>(parser);
		}

		/// <summary>
		///   Applies the two parsers in sequence. The second parser is only applied if the first parser
		///   succeeds. The results of both parsers are passed to the given function, whose return value is the result
		///   of the pipe2 parser.
		/// </summary>
		/// <typeparam name="TResultFirst">The type of the first parser's result.</typeparam>
		/// <typeparam name="TResultSecond">The type of the second parser's result.</typeparam>
		/// <typeparam name="T">The type of the pipe2 parser's result.</typeparam>
		protected static Parser<T, TUserState> Pipe<TResultFirst, TResultSecond, T>(Parser<TResultFirst, TUserState> first,
																					Parser<TResultSecond, TUserState> second,
																					Func<TResultFirst, TResultSecond, T> function)
		{
			return new Pipe2Parser<TResultFirst, TResultSecond, T, TUserState>(first, second, function);
		}

		/// <summary>
		///   Applies the three parsers in sequence. The second parser is only applied if the first parser
		///   succeeds, and the third parser is only applied if the second once succeeds. The results of all three parsers are
		///   passed to the given function, whose return value is the result of the pipe3 parser.
		/// </summary>
		/// <typeparam name="TResultFirst">The type of the first parser's result.</typeparam>
		/// <typeparam name="TResultSecond">The type of the second parser's result.</typeparam>
		/// <typeparam name="TResultThird">The type of the third parser's result.</typeparam>
		/// <typeparam name="T">The type of the pipe3 parser's result.</typeparam>
		protected static Parser<T, TUserState> Pipe<TResultFirst, TResultSecond, TResultThird, T>(
			Parser<TResultFirst, TUserState> first,
			Parser<TResultSecond, TUserState> second,
			Parser<TResultThird, TUserState> third,
			Func<TResultFirst, TResultSecond, TResultThird, T> function)
		{
			return new Pipe3Parser<TResultFirst, TResultSecond, TResultThird, T, TUserState>(first, second, third, function);
		}

		/// <summary>
		///   Applies the four parsers in sequence. The second parser is only applied if the first parser
		///   succeeds, and the third parser is only applied if the second once succeeds, and so on. The results of all four
		///   parsers are passed to the given function, whose return value is the result of the pipe4 parser.
		/// </summary>
		/// <typeparam name="TResultFirst">The type of the first parser's result.</typeparam>
		/// <typeparam name="TResultSecond">The type of the second parser's result.</typeparam>
		/// <typeparam name="TResultThird">The type of the third parser's result.</typeparam>
		/// <typeparam name="TResultFourth">The type of the fourth parser's result.</typeparam>
		/// <typeparam name="T">The type of the pipe2 parser's result.</typeparam>
		protected static Parser<T, TUserState> Pipe<TResultFirst, TResultSecond, TResultThird, TResultFourth, T>(
			Parser<TResultFirst, TUserState> first, Parser<TResultSecond, TUserState> second,
			Parser<TResultThird, TUserState> third, Parser<TResultFourth, TUserState> fourth,
			Func<TResultFirst, TResultSecond, TResultThird, TResultFourth, T> function)
		{
			return new Pipe4Parser<TResultFirst, TResultSecond, TResultThird, TResultFourth, T, TUserState>(first, second, third,
																											fourth, function);
		}

		/// <summary>
		///   Parses as many occurrences (possibly zero) of the given parser as possible, where each occurrence is separated by the
		///   given separation parser. The sequence is expected to end without another occurrence of the separation parser.
		/// </summary>
		/// <typeparam name="T">The type of the parser's result.</typeparam>
		/// <typeparam name="TSeparate">The type of the separation parser's result.</typeparam>
		/// <param name="parser">The parser that is applied several times.</param>
		/// <param name="separationParser">The separation parser.</param>
		protected static Parser<List<T>, TUserState> SeparatedBy<T, TSeparate>(Parser<T, TUserState> parser,
																			   Parser<TSeparate, TUserState> separationParser)
		{
			return new SeparatedByParser<T, TSeparate, TUserState>(parser, separationParser);
		}

		/// <summary>
		///   Parses as many occurrences (at least one) of the given parser as possible, where each occurrence is separated by the
		///   given separation parser. The sequence is expected to end without another occurrence of the separation parser.
		/// </summary>
		/// <typeparam name="T">The type of the parser's result.</typeparam>
		/// <typeparam name="TSeparate">The type of the separation parser's result.</typeparam>
		/// <param name="parser">The parser that is applied several times.</param>
		/// <param name="separationParser">The separation parser.</param>
		protected static Parser<List<T>, TUserState> SeparatedBy1<T, TSeparate>(Parser<T, TUserState> parser,
																				Parser<TSeparate, TUserState> separationParser)
		{
			return new SeparatedBy1Parser<T, TSeparate, TUserState>(parser, separationParser);
		}

		/// <summary>
		///   Parses the single occurrence of the given parser, enclosed by a single occurrence of the given left and right
		///   parsers. The right parser is applied once the given parser returns.
		/// </summary>
		/// <typeparam name="T">The type of the parser's result.</typeparam>
		/// <typeparam name="TLeft">The type of the left separation parser's result.</typeparam>
		/// <typeparam name="TRight">The type of the right separation parser's result.</typeparam>
		/// <param name="parser">The parser that is applied.</param>
		/// <param name="leftParser">The left separation parser.</param>
		/// <param name="rightParser">The right separation parser.</param>
		protected static Parser<T, TUserState> Between<T, TLeft, TRight>(Parser<T, TUserState> parser,
																		 Parser<TLeft, TUserState> leftParser,
																		 Parser<TRight, TUserState> rightParser)
		{
			return new BetweenParser<T, TLeft, TRight, TUserState>(parser, leftParser, rightParser);
		}

		#endregion

		#region Overloaded operators

		/// <summary>
		///   Applies the first parser and the second parser in sequence, returning the result of the first parser.
		/// </summary>
		/// <param name="first">The first parser that should be applied to the input.</param>
		/// <param name="second">The second parser that should be applied to the input.</param>
		public static Parser<TResult, TUserState> operator +(Parser<TResult, TUserState> first, SkipParser<TUserState> second)
		{
			return new SequenceFirstParser<TResult, None, TUserState>(first, second);
		}

		/// <summary>
		///   Applies the first parser and the second parser in sequence, returning the result of the second parser.
		/// </summary>
		/// <param name="first">The first parser that should be applied to the input.</param>
		/// <param name="second">The second parser that should be applied to the input.</param>
		public static Parser<TResult, TUserState> operator +(SkipParser<TUserState> first, Parser<TResult, TUserState> second)
		{
			return new SequenceSecondParser<None, TResult, TUserState>(first, second);
		}

		/// <summary>
		///   Applies the first parser to the input and returns its result if it succeeds; otherwise, applies the second parser and
		///   returns its results.
		/// </summary>
		/// <param name="first">The first parser that should be applied to the input.</param>
		/// <param name="second">The second parser that should be applied to the input.</param>
		public static Parser<TResult, TUserState> operator |(Parser<TResult, TUserState> first, Parser<TResult, TUserState> second)
		{
			return new AlternativesParser<TResult, TUserState>(first, second);
		}

		/// <summary>
		///   Constructs a new parser that skips over the input parsed by the given parser, ignoring the parser's result.
		/// </summary>
		/// <param name="parser">The parser whose return value is ignored.</param>
		public static SkipParser<TUserState> operator ~(Parser<TResult, TUserState> parser)
		{
			return new SkipParser<TResult, TUserState>(parser);
		}

		/// <summary>
		///   Applies the given parser and replaces the parser's error message by the given description in case of failure. The
		///   given description is returned as an expected error.
		/// </summary>
		/// <param name="parser">The parser whose description should be overwritten.</param>
		/// <param name="description">The description that should be returned as an expected error in the case of failure.</param>
		public static Parser<TResult, TUserState> operator %(Parser<TResult, TUserState> parser, string description)
		{
			return new DescriptionParser<TResult, TUserState>(parser, description);
		}

		#endregion
	}
}