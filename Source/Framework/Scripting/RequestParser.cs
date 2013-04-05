using System;

namespace Pegasus.Framework.Scripting
{
	using Parsing;
	using Parsing.Combinators;
	using Requests;

	/// <summary>
	///   Parses a user request.
	/// </summary>
	/// <remarks>
	///   This parser is written in a very low-level way. There are two reasons for this: First of all, the parser is
	///   highly dynamic, meaning that it does not parse a statically defined grammar. Instead, depending on the cvar name or
	///   command name that is found at the beginning of the input, the remainder of the parser is configured dynamically to
	///   parse exactly the possible arguments of a cvar or command request. Therefore, the parser is already responsible for
	///   the type checks of the arguments. Secondly, the requirement that the cvar/command name and all arguments are
	///   separated by white space requires some backtracking by the parser, making it harder to generate good error
	///   messages. Most of the complexity of this parser is a result of the generation of good error messages.
	/// </remarks>
	internal class RequestParser : Parser<IRequest, None>
	{
		/// <summary>
		///   A parser for identifiers.
		/// </summary>
		private static readonly Parser<string, None> Identifier = String(c => Char.IsLetter(c) || c == '_',
																		 c => Char.IsLetterOrDigit(c) || c == '_', "identifier");

		/// <summary>
		///   Skips any number of whitespaces and then expects the end of the input.
		/// </summary>
		private static readonly SkipParser<None> EndOfRequest = ~(WhiteSpaces + ~EndOfInput);

		/// <summary>
		///   The command registry that is used to look up cvars.
		/// </summary>
		private readonly CommandRegistry _commands;

		/// <summary>
		///   The cvar registry that is used to look up cvars.
		/// </summary>
		private readonly CvarRegistry _cvars;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="commands">The command registry that should be used to look up commands.</param>
		/// <param name="cvars">The cvar registry that should be used to look up cvars.</param>
		public RequestParser(CommandRegistry commands, CvarRegistry cvars)
		{
			Assert.ArgumentNotNull(cvars, () => cvars);
			Assert.ArgumentNotNull(commands, () => commands);

			_cvars = cvars;
			_commands = commands;
		}

		/// <summary>
		///   Parses the given input string and returns the user command.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<IRequest> Parse(InputStream<None> inputStream)
		{
			// Skip all leading white space
			inputStream.SkipWhiteSpaces();

			// Parse the cvar or command name
			var state = inputStream.State;
			var reply = Identifier.Parse(inputStream);
			if (reply.Status != ReplyStatus.Success)
				return ForwardError(reply);

			// Check if a cvar has been referenced and if so, return the appropriate user request
			var name = reply.Result;
			ICvar cvar;
			if (_cvars.TryFind(name, out cvar))
				return Parse(inputStream, cvar);

			// Check if a command has been referenced and if so, return the appropriate user request
			ICommand command;
			if (_commands.TryFind(name, out command))
				return Parse(inputStream, command);

			// If the name refers to neither a cvar nor a command, return an error message
			inputStream.State = state;
			return Message(System.String.Format("Unknown command '{0}'.", name));
		}

		/// <summary>
		///   Parses a cvar user request.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		/// <param name="cvar">The cvar referenced by the user request.</param>
		private static Reply<IRequest> Parse(InputStream<None> inputStream, ICvar cvar)
		{
			var cvarDisplay = EndOfRequest.Apply<IRequest>(_ => new DisplayCvar(cvar));

			var cvarHelp = (~WhiteSpaces1 + Character('?') + EndOfRequest)
				.Apply<IRequest>(_ => new DescribeCvar(cvar));

			var cvarSet = (~WhiteSpaces1 + new TypeParser<None>(cvar.ValueType) + EndOfRequest)
				.Apply<IRequest>(v => new SetCvar(cvar, v));

			var cvarParser = Attempt(cvarDisplay) | Attempt(cvarHelp) | cvarSet;
			return cvarParser.Parse(inputStream);
		}

		/// <summary>
		///   Parses a command user request.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		/// <param name="command">The command referenced by the user request.</param>
		private Reply<IRequest> Parse(InputStream<None> inputStream, ICommand command)
		{
			var commandHelp = Character('?').Apply<IRequest>(_ => new DescribeCommand(command));

			// Depending on whether the command actually has any parameters, modify the grammar to get better error messages
			Parser<IRequest, None> commandParser;
			if (command.ParameterTypes.Length == 0)
			{
				var invokeCommand = EndOfRequest.Apply<IRequest>(_ => new InvokeCommand(command, new object[0]));
				commandParser = Attempt(invokeCommand) | (~WhiteSpaces1 + commandHelp + EndOfRequest);
			}
			else
			{
				// Show an unexpected end of input error message if we've reached the end of the input already (otherwise an 
				// 'expected white space' message would be shown)
				if (inputStream.EndOfInput)
					return UnexpectedEndOfInput();

				var invokeCommand = new ArgumentListParser(command)
					.Apply<IRequest>(parameters => new InvokeCommand(command, parameters));
				commandParser = ~WhiteSpaces1 + (commandHelp | invokeCommand) + EndOfRequest;
			}

			return commandParser.Parse(inputStream);
		}

		/// <summary>
		///   Parses an arguments list for a command invocation.
		/// </summary>
		private class ArgumentListParser : Parser<object[], None>
		{
			/// <summary>
			///   The command for which the argument list is parsed.
			/// </summary>
			private readonly ICommand _command;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="command">The command for which the argument list should be parsed.</param>
			public ArgumentListParser(ICommand command)
			{
				Assert.ArgumentNotNull(command, () => command);
				_command = command;
			}

			/// <summary>
			///   Parses the given input string and returns the command arguments.
			/// </summary>
			/// <param name="inputStream">The input stream that should be parsed.</param>
			public override Reply<object[]> Parse(InputStream<None> inputStream)
			{
				var types = _command.ParameterTypes;
				var parameters = new object[types.Length];

				for (var i = 0; i < parameters.Length; ++i)
				{
					// Show an unexpected end of input error message if we've reached the end of the input already (otherwise an 
					// 'expected white space' message would be shown)
					if (inputStream.EndOfInput)
						return Errors(ErrorMessage.UnexpectedEndOfInput, new ErrorMessage(ErrorType.Message, "Usage: " + _command.Signature),
									  new ErrorMessage(ErrorType.Expected, TypeDescription.GetDescription(types[i])));

					// The argument must be separated from the previous input by at least one white space character
					if (i != 0 && !Char.IsWhiteSpace(inputStream.Peek()))
						return Expected("whitespace");

					inputStream.SkipWhiteSpaces();

					var reply = new TypeParser<None>(types[i]).Parse(inputStream);
					if (reply.Status == ReplyStatus.Success)
						parameters[i] = reply.Result;
					else
						return ForwardError(reply, "Usage: " + _command.Signature);
				}

				return Success(parameters);
			}
		}
	}
}