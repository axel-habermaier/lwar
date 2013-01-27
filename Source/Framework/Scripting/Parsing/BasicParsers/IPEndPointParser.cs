using System;

namespace Pegasus.Framework.Scripting.Parsing.BasicParsers
{
	using System.Net;

	/// <summary>
	///   Parses an IP endpoint in the format of IP address [:port].
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class IPEndPointParser<TUserState> : Parser<IPEndPoint, TUserState>
	{
		/// <summary>
		///   The description that is displayed if the parser failed.
		/// </summary>
		internal const string Description = "IP address and port";

		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<IPEndPoint> Parse(InputStream<TUserState> inputStream)
		{
			var state = inputStream.State;

			string ipAddressString;
			IPAddress ipAddress;

			// IPv6 addresses start with a bracket
			if (inputStream.Peek() == '[')
			{
				var count = inputStream.Skip(c => Char.IsDigit(c) || c == '.' || c == '[' || c == ':' || (c >= 'a' && c <= 'f'));
				if (count == 0 || inputStream.Peek() != ']')
				{
					inputStream.State = state;
					return Expected(Description);
				}

				ipAddressString = inputStream.Substring(state.Position + 1, inputStream.State.Position - state.Position - 1);

				if (!inputStream.Skip("]:"))
				{
					inputStream.State = state;
					return Expected(Description);
				}
			}
			else
			{
				var count = inputStream.Skip(c => Char.IsDigit(c) || c == '.');
				if (count == 0 || inputStream.Peek() != ':' || inputStream.EndOfInput)
				{
					inputStream.State = state;
					return Expected(Description);
				}

				ipAddressString = inputStream.Substring(state.Position, inputStream.State.Position - state.Position);
				inputStream.Skip(1);
			}

			// Convert the IP address, excluding the brackets
			try
			{
				ipAddress = IPAddress.Parse(ipAddressString);
			}
			catch (FormatException)
			{
				inputStream.State = state;
				return Expected(Description);
			}

			var reply = UInt16.Parse(inputStream);
			if (reply.Status != ReplyStatus.Success)
			{
				inputStream.State = state;
				return Expected(Description);
			}

			return Success(new IPEndPoint(ipAddress, reply.Result));
		}
	}
}