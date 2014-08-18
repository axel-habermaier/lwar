namespace Pegasus.Scripting.Parsing
{
	using System;
	using BasicParsers;

	/// <summary>
	///     An abstract base class for the implementation of basic parsers and combined parsers.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser result.</typeparam>
	public abstract partial class Parser<TResult>
	{
		/// <summary>
		///     Parses a single digit character.
		/// </summary>
		protected static readonly Parser<char> Digit = new DigitParser();

		/// <summary>
		///     Parses a Boolean value.
		/// </summary>
		protected static readonly Parser<bool> Boolean = new BooleanParser();

		/// <summary>
		///     Parses a 32-bit floating point number.
		/// </summary>
		protected static readonly Parser<float> Float32 = new Float32Parser();

		/// <summary>
		///     Parses a 64-bit floating point number.
		/// </summary>
		protected static readonly Parser<double> Float64 = new Float64Parser();

		/// <summary>
		///     Parses a signed 32-bit integer.
		/// </summary>
		protected static readonly Parser<int> Int32 = new Int32Parser();

		/// <summary>
		///     Parses an unsigned 32-bit integer.
		/// </summary>
		protected static readonly Parser<uint> UInt32 = new UInt32Parser();

		/// <summary>
		///     Parses a signed 64-bit integer.
		/// </summary>
		protected static readonly Parser<long> Int64 = new Int64Parser();

		/// <summary>
		///     Parses an unsigned 64-bit integer.
		/// </summary>
		protected static readonly Parser<ulong> UInt64 = new UInt64Parser();

		/// <summary>
		///     Parses a signed 16-bit integer.
		/// </summary>
		protected static readonly Parser<short> Int16 = new Int16Parser();

		/// <summary>
		///     Parses an unsigned 16-bit integer.
		/// </summary>
		protected static readonly Parser<ushort> UInt16 = new UInt16Parser();

		/// <summary>
		///     Parses a signed 8-bit integer.
		/// </summary>
		protected static readonly Parser<sbyte> Int8 = new Int8Parser();

		/// <summary>
		///     Parses an unsigned 8-bit integer.
		/// </summary>
		protected static readonly Parser<byte> UInt8 = new UInt8Parser();

		/// <summary>
		///     Parses a single letter character.
		/// </summary>
		protected static readonly Parser<char> Letter = new LetterParser();

		/// <summary>
		///     Parses the end of the input.
		/// </summary>
		protected static readonly Parser<None> EndOfInput = new EndOfInputParser();

		/// <summary>
		///     Parses zero or more white space characters.
		/// </summary>
		protected static readonly Parser<None> WhiteSpaces = new WhiteSpacesParser();

		/// <summary>
		///     Parses one or more white space characters.
		/// </summary>
		protected static readonly Parser<None> WhiteSpaces1 = new WhiteSpaces1Parser();

		/// <summary>
		///     Parses a string literal enclosed in double quotes. Double quotes can be used inside the string literal if they are
		///     escaped by a backslash '\"'.
		/// </summary>
		protected static readonly Parser<string> QuotedStringLiteral = new QuotedStringParser();
	}
}