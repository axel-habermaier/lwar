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
	///   for the type checks of the arguments. Secondly, the requirement that the cvar/command name and all arguments are
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
			Assert.ArgumentNotNull(cvars);
			Assert.ArgumentNotNull(commands);

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
		private Reply<Instruction> Parse(InputStream<None> inputStream, ICommand command)
		{
			var parameters = command.Parameters.ToArray();
			var values = new object[parameters.Length];

			for (var i = 0; i < parameters.Length; ++i)
			{
				// We've reached the end of the input, and all subsequent parameters should use their default value, we're done
				if (inputStream.WhiteSpaceUntilEndOfInput() && parameters[i].HasDefaultValue)
				{
					for (var j = i; j < parameters.Length; ++j)
						values[j] = parameters[j].DefaultValue;

					break;
				}
				
				// We've reached the end of the input, but we're missing at least one parameter
				if (inputStream.WhiteSpaceUntilEndOfInput() && !parameters[i].HasDefaultValue)
				{
					inputStream.SkipWhiteSpaces(); // To get the correct column in the error message
					return Errors(new ErrorMessage(ErrorType.Expected, TypeDescription.GetDescription(parameters[i].Type)),
								  new ErrorMessage(ErrorType.Message, Help.GetHint(command.Name)));
				}

				// The argument must be separated from the previous one by at least one white space character
				if (!Char.IsWhiteSpace(inputStream.Peek()))
					return Expected("whitespace");

				inputStream.SkipWhiteSpaces();

				var reply = new TypeParser<None>(parameters[i].Type).Parse(inputStream);
				if (reply.Status == ReplyStatus.Success)
					values[i] = reply.Result;
				else
					return ForwardError(reply, Help.GetHint(command.Name));
			}

			return EndOfInstruction.Apply(_ => new Instruction(command, values)).Parse(inputStream);
		}
	}
}