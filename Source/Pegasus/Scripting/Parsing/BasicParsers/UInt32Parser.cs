namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;
	using System.Globalization;

	/// <summary>
	///   Parses an unsigned 32-bit integer.
	/// </summary>
	public class UInt32Parser : NumberParser<uint>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public UInt32Parser()
			: base(false, false)
		{
		}

		/// <summary>
		///   Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<uint> ConvertToNumber(string number)
		{
			return Success(uint.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}