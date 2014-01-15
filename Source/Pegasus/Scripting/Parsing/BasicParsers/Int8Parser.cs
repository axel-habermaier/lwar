namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Parses a signed 8-bit integer.
	/// </summary>
	public class Int8Parser : NumberParser<sbyte>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Int8Parser()
			: base(true, false)
		{
		}

		/// <summary>
		///     Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<sbyte> ConvertToNumber(string number)
		{
			return Success(SByte.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}