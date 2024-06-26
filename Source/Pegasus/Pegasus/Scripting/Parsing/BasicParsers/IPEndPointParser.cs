﻿namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;
	using Platform.Network;

	/// <summary>
	///     Parses an IP endpoint in the format of IP address [:port].
	/// </summary>
	public class IPEndPointParser : CombinedParser<IPEndPoint>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public IPEndPointParser()
		{
			var ipParser = new IPAddressParser();

			var ip = Between(ipParser, Character('['), Character(']')) | ipParser;
			var port = (~Character(':') + Parsers.UInt16).Optional(0);
			Parser = Pipe(ip, port, (i, p) => new IPEndPoint(i, p));
		}
	}
}