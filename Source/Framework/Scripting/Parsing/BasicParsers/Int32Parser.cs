using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Globalization;

	/// <summary>
	///   Parses a signed 32-bit integer.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class Int32Parser<TUserState> : NumberParser<int, TUserState>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Int32Parser()
			: base(true, false, TypeDescription.GetDescription<int>())
		{
		}

		/// <summary>
		///   Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<int> ConvertToNumber(string number)
		{
			return Success(int.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}