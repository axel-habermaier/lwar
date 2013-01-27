using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Globalization;

	/// <summary>
	///   Parses a signed 64-bit integer.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class Int64Parser<TUserState> : NumberParser<long, TUserState>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Int64Parser()
			: base(true, false, TypeDescription.GetDescription<long>())
		{
		}

		/// <summary>
		///   Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<long> ConvertToNumber(string number)
		{
			return Success(long.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}