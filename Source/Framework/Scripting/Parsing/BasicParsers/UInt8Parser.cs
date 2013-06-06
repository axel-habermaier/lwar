using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Globalization;

	/// <summary>
	///   Parses an unsigned 8-bit integer.
	/// </summary>
	public class UInt8Parser : NumberParser<byte>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public UInt8Parser()
			: base(false, false)
		{
		}

		/// <summary>
		///   Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<byte> ConvertToNumber(string number)
		{
			return Success(Byte.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}