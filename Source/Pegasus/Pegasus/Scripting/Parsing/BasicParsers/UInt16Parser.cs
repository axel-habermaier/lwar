﻿namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Parses an unsigned 16-bit integer.
	/// </summary>
	public class UInt16Parser : NumberParser<ushort>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public UInt16Parser()
			: base(false, false)
		{
		}

		/// <summary>
		///     Converts the parsed digits into a number.
		/// </summary>
		/// <param name="number">The number as a string that should be converted to its numerical representation.</param>
		protected override Reply<ushort> ConvertToNumber(string number)
		{
			return Success(ushort.Parse(number, CultureInfo.InvariantCulture));
		}
	}
}