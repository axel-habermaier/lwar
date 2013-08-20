using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Globalization;

	/// <summary>
	///   Parses a 32-bit floating point number.
	/// </summary>
	public class Float32Parser : NumberParser<float>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Float32Parser()
			: base(true, true)
		{
		}

		/// <summary>
		///   Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<float> ConvertToNumber(string number)
		{
			return Success(Single.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}