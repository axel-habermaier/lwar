namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Parses a 64-bit float point number.
	/// </summary>
	public class Float64Parser : NumberParser<double>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Float64Parser()
			: base(true, true)
		{
		}

		/// <summary>
		///     Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<double> ConvertToNumber(string number)
		{
			return Success(Double.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}