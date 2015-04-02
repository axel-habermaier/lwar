namespace Pegasus.Scripting.Parsing.BasicParsers
{
	using System;
	using Platform.Network;

	/// <summary>
	///     Parses an IPv4 or IPv6 address.
	/// </summary>
	public class IPAddressParser : Parser<IPAddress>
	{
		/// <summary>
		///     The description that is displayed if the parser failed.
		/// </summary>
		internal const string Description = "IP address (either IPv4 or IPv6)";

		/// <summary>
		///     Parses the 'localhost' keyword.
		/// </summary>
		private readonly SkipStringParser _localHostParser = new SkipStringParser("localhost", ignoreCase: true);

		/// <summary>
		///     Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<IPAddress> Parse(InputStream inputStream)
		{
			var state = inputStream.State;
			IPAddress address;

			// Check for the 'localhost' keyword first
			if (_localHostParser.Parse(inputStream).Status == ReplyStatus.Success)
				return Success(IPAddress.LocalHost);

			// Try to find out whether it is an IPv4 or IPv6 address
			var ipv4 = inputStream.Skip(c => c != '.');
			inputStream.State = state;
			var ipv6 = inputStream.Skip(c => c != ':');
			inputStream.State = state;

			var isIPv6 = ipv6 < ipv4;
			var length = 0;

			if (isIPv6)
				length = inputStream.Skip(c => Char.IsDigit(c) || c == ':' || c == '.' || c == '%' ||
											   (Char.ToLower(c) >= 'a' && Char.ToLower(c) <= 'f'));
			else
				length = inputStream.Skip(c => Char.IsDigit(c) || c == '.');

			if (IPAddress.TryParse(inputStream.Substring(state.Position, length), out address))
				return Success(address);

			inputStream.State = state;
			return Expected(Description);
		}
	}
}