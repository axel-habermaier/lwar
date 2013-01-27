using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Globalization;

	/// <summary>
	///   Parses a signed 8-bit integer.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class Int8Parser<TUserState> : NumberParser<sbyte, TUserState>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Int8Parser()
			: base(true, false, TypeDescription.GetDescription<sbyte>())
		{
		}

		/// <summary>
		///   Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<sbyte> ConvertToNumber(string number)
		{
			return Success(SByte.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}