using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Globalization;

	/// <summary>
	///   Parses a 64-bit float point number.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class Float64Parser<TUserState> : NumberParser<double, TUserState>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Float64Parser()
			: base(true, true, TypeDescription.GetDescription<double>())
		{
		}

		/// <summary>
		///   Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<double> ConvertToNumber(string number)
		{
			return Success(Double.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}