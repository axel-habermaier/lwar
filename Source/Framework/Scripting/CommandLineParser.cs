using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;
	using Parsing;
	using Parsing.Combinators;
	using Requests;

	/// <summary>
	///   Parses the command line. The command line consists of a list of cvar set requests and/or verbs. For instance, the
	///   command line "compile -time_scale 0.01" returns a CommandLine instance where the CompileAssets field is set to true
	///   as well as a single set cvar request that sets the value of the time scale cvar to 0.01.
	/// </summary>
	internal class CommandLineParser : Parser<IEnumerable<SetCvar>, None>
	{
		/// <summary>
		///   Parses the given input string and returns the parser's reply.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<IEnumerable<SetCvar>> Parse(InputStream<None> inputStream)
		{
			var endOrWhitespace = ~(Attempt(WhiteSpaces + ~EndOfInput) | WhiteSpaces1);
			var appPath = ~((StringLiteral | String(c => c != ' ', "application path")) + ~endOrWhitespace);

			// Ignore the application path at the beginning of the command line
			var reply = appPath.Parse(inputStream);
			if (reply.Status != ReplyStatus.Success)
				return ForwardError(reply);

			var setCvars = new List<SetCvar>();
			var identifier = String(c => Char.IsLetter(c) || c == '_', c => Char.IsLetterOrDigit(c) || c == '_', "identifier");

			// Parse all cvar set requests until we reach the end of the command line
			while (!inputStream.EndOfInput)
			{
				inputStream.SkipWhiteSpaces();

				// Print an error if the cvar set request does not start with a '-'
				if (inputStream.Skip(c => c == '-') != 1)
					return Message("Expected a '-' followed by the name of a cvar.");

				var state = inputStream.State;

				// Parse the cvar identifier and get the cvar from the registry
				var cvarReply = identifier.Parse(inputStream);
				if (cvarReply.Status != ReplyStatus.Success)
					return ForwardError(cvarReply);

				var cvar = CvarRegistry.Find(cvarReply.Result);
				if (cvar == null)
				{
					// Reset the state so that the error is reported right after the '-'
					inputStream.State = state;
					return Message(string.Format("Unknown cvar '{0}'.", cvarReply.Result));
				}

				// Parse the cvar argument
				var argument = (~WhiteSpaces1 + new TypeParser<None>(cvar.ValueType) + ~endOrWhitespace);
				var argumentReply = argument.Parse(inputStream);
				if (argumentReply.Status != ReplyStatus.Success)
					return ForwardError(argumentReply);

				setCvars.Add(new SetCvar(cvar, argumentReply.Result));
			}

			return Success(setCvars);
		}
	}
}