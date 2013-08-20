using System;

namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System.Globalization;

	/// <summary>
	///   Parses a signed 16-bit integer.
	/// </summary>
	public class Int16Parser : NumberParser<short>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Int16Parser()
			: base(true, false)
		{
		}

		/// <summary>
		///   Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<short> ConvertToNumber(string number)
		{
			return Success(short.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}