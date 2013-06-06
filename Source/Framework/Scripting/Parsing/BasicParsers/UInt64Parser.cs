using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Globalization;

	/// <summary>
	///   Parses an unsigned 64-bit integer.
	/// </summary>
	public class UInt64Parser : NumberParser<ulong>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public UInt64Parser()
			: base(false, false)
		{
		}

		/// <summary>
		///   Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<ulong> ConvertToNumber(string number)
		{
			return Success(ulong.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}