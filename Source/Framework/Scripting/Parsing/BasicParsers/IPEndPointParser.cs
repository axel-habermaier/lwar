using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Net;

	/// <summary>
	///   Parses an IP endpoint in the format of IP address [:port].
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class IPEndPointParser<TUserState> : CombinedParser<IPEndPoint, TUserState>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public IPEndPointParser()
		{
			var ipParser = new IPAddressParser<TUserState>();

			var ip = Between(ipParser, Character('['), Character(']')) | ipParser;
			var port = (~Character(':') + UInt16).Optional(0);
			Parser = Pipe(ip, port, (i, p) => new IPEndPoint(i, p));
		}
	}
}