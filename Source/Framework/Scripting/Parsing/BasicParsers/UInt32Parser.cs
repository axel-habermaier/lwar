using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Globalization;

	/// <summary>
	///   Parses an unsigned 32-bit integer.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class UInt32Parser<TUserState> : NumberParser<uint, TUserState>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public UInt32Parser()
			: base(false, false, TypeDescription.GetDescription<uint>())
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