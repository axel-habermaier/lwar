using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Net;

	/// <summary>
	///   Parses an IPv4 or IPv6 address.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class IPAddressParser<TUserState> : Parser<IPAddress, TUserState>
	{
		/// <summary>
		///   The description that is displayed if the parser failed.
		/// </summary>
		internal const string Description = "IP address (either IPv4 or IPv6)";

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<IPAddress> Parse(InputStream<TUserState> inputStream)
		{
			var state = inputStream.State;

			var length = inputStream.Skip(c => !Char.IsWhiteSpace(c));
			var ipAddress = inputStream.Substring(state.Position, length);

			// Convert the IP address, excluding the brackets
			try
			{
				return Success(IPAddress.Parse(ipAddress));
			}
			catch (FormatException)
			{
				inputStream.State = state;
				return Expected(Description);
			}
		}
	}
}