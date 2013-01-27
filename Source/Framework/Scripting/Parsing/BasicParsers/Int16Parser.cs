using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Globalization;

	/// <summary>
	///   Parses a signed 16-bit integer.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class Int16Parser<TUserState> : NumberParser<short, TUserState>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Int16Parser()
			: base(true, false, TypeDescription.GetDescription<short>())
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