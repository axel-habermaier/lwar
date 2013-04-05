using System;

namespace Pegasus.Framework.Scripting
{
	using System.Linq;
	using Parsing;
	using Parsing.Combinators;

	/// <summary>
	///   Parses an instruction.
	/// </summary>
	/// <remarks>
	///   This parser is written in a very low-level way. There are two reasons for this: First of all, the parser is
	///   highly dynamic, meaning that it does not parse a statically defined grammar. Instead, depending on the cvar name or
	///   command name that is found at the beginning of the input, the remainder of the parser is configured dynamically to
	///   parse exactly the possible arguments of a cvar or command instruction. Therefore, the parser is already responsible
	///   for
	///   the type checks of the arguments. Secondly, the requirement that the cvar/command name and all arguments are
	///   separated by white space requires some backtracking by the parser, making it harder to generate good error
	///   messages. Most of the complexity of this parser is a result of the generation of good error messages.
	/// </remarks>
	internal class InstructionParser : Parser<Instruction, None>
	{
		/// <summary>
		///   A parser for identifiers.
		/// </summary>
		private static readonly Parser<string, None> Identifier = String(c => Char.IsLetter(c) || c == '_',
																		 c => Char.IsLetterOrDigit(c) || c == '_', "identifier");

		/// <summary>
		///   Skips any number of whitespaces and then expects the end of the input.
		/// </summary>
		private static readonly SkipParser<None> EndOfInstruction = ~(WhiteSpaces + ~EndOfInput);

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
		public InstructionParser(CommandRegistry commands, CvarRegistry cvars)
		{
			Assert.ArgumentNotNull(cvars, () => cvars);
			Assert.ArgumentNotNull(commands, () => commands);

			_cvars = cvars;
			_commands = commands;
		}

		/// <summary>
		///   Parses the given input string and returns the instruction.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<Instruction> Parse(InputStream<None> inputStream)
		{
			// Skip all leading white space
			inputStream.SkipWhiteSpaces();

			// Parse the cvar or command name
			var state = inputStream.State;
			var reply = Identifier.Parse(inputStream);
			if (reply.Status != ReplyStatus.Success)
				return ForwardError(reply);

			// Check if a cvar has been referenced and if so, return the appropriate instruction
			var name = reply.Result;
			ICvar cvar;
			if (_cvars.TryFind(name, out cvar))
				return Parse(inputStream, cvar);

			// Check if a command has been referenced and if so, return the appropriate instruction
			ICommand command;
			if (_commands.TryFind(name, out command))
				return Parse(inputStream, command);

			// If the name refers to neither a cvar nor a command, return an error message
			inputStream.State = state;
			return Message(System.String.Format("Unknown command '{0}'.", name));
		}

		/// <summary>
		///   Parses a cvar instruction.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		/// <param name="cvar">The cvar referenced by the instruction.</param>
		private static Reply<Instruction> Parse(InputStream<None> inputStream, ICvar cvar)
		{
			var cvarDisplay = EndOfInstruction.Apply(_ => new Instruction(cvar, null));

			var cvarSet = (~WhiteSpaces1 + new TypeParser<None>(cvar.ValueType) + EndOfInstruction)
				.Apply(v => new Instruction(cvar, v));

			var cvarParser = Attempt(cvarDisplay) | cvarSet;
			return cvarParser.Parse(inputStream);
		}

		/// <summary>
		///   Parses a command invocation.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		/// <param name="command">The command referenced by the instruction.</param>
		private static Reply<Instruction> Parse(InputStream<None> inputStream, ICommand command)
		{
			// Depending on whether the command actually has any parameters, modify the grammar to get better error messages
			Parser<Instruction, None> commandParser;
			if (!command.Parameters.Any())
				commandParser = EndOfInstruction.Apply(_ => new Instruction(command, new object[0]));
			else
			{
				var parser = new ArgumentListParser(command).Apply(parameters => new Instruction(command, parameters));
				commandParser = parser + ~EndOfInstruction;
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
				var parameters = _command.Parameters.ToArray();
				var values = new object[parameters.Length];

				for (var i = 0; i < parameters.Length; ++i)
				{
					// Show an unexpected end of input error message if we've reached the end of the input already (otherwise an 
					// 'expected white space' message would be shown)
					if (inputStream.EndOfInput && !parameters[i].HasDefaultValue)
						return Errors(ErrorMessage.UnexpectedEndOfInput,
									  new ErrorMessage(ErrorType.Message, GetHelpText(_command)),
									  new ErrorMessage(ErrorType.Expected, TypeDescription.GetDescription(parameters[i].Type)));

					// We've reached the end of the input, and all subsequent parameters should use their default value
					if (inputStream.WhiteSpaceUntilEndOfInput() && parameters[i].HasDefaultValue)
					{
						for (var j = i; j < parameters.Length; ++j)
							values[j] = parameters[j].DefaultValue;

						return Success(values);
					}

					// The argument must be separated from the previous input by at least one white space character
					if (!Char.IsWhiteSpace(inputStream.Peek()))
						return Expected("whitespace");

					inputStream.SkipWhiteSpaces();

					var reply = new TypeParser<None>(parameters[i].Type).Parse(inputStream);
					if (reply.Status == ReplyStatus.Success)
						values[i] = reply.Result;
					else
						return ForwardError(reply, GetHelpText(_command));
				}

				return Success(values);
			}

			/// <summary>
			///   Gets a help string for the given command.
			/// </summary>
			/// <param name="command">The command for which the help string should be returned.</param>
			private static string GetHelpText(ICommand command)
			{
				return string.Format("Use 'help \"{0}\"' for details about the usage of '{0}'.", command.Name);
			}
		}
	}
}