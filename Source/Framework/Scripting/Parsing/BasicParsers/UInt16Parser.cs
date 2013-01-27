using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Globalization;

	/// <summary>
	///   Parses an unsigned 16-bit integer.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class UInt16Parser<TUserState> : NumberParser<ushort, TUserState>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public UInt16Parser()
			: base(false, false, TypeDescription.GetDescription<ushort>())
		{
		}

		/// <summary>
		///   Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<ushort> ConvertToNumber(string number)
		{
			return Success(ushort.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}