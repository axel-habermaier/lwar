namespace Pegasus.Scripting
{
	using System;
	using System.Linq;
	using Parsing;
	using Parsing.Combinators;

	/// <summary>
	///     Parses an instruction.
	/// </summary>
	/// <remarks>
	///     This parser is written in a very low-level way. There are two reasons for this: First of all, the parser is
	///     highly dynamic, meaning that it does not parse a statically defined grammar. Instead, depending on the cvar name or
	///     command name that is found at the beginning of the input, the remainder of the parser is configured dynamically to
	///     parse exactly the possible arguments of a cvar or command instruction. Therefore, the parser is already responsible
	///     for the type checks of the arguments. Secondly, the requirement that the cvar/command name and all arguments are
	///     separated by white space requires some backtracking by the parser, making it harder to generate good error
	///     messages. Most of the complexity of this parser is a result of the generation of good error messages.
	/// </remarks>
	internal class InstructionParser : Parser<Instruction>
	{
		/// <summary>
		///     A parser for identifiers.
		/// </summary>
		private static readonly Parser<string> Identifier = String(c => Char.IsLetter(c) || c == '_',
																   c => Char.IsLetterOrDigit(c) || c == '_', "identifier");

		/// <summary>
		///     Skips any number of whitespaces and then expects the end of the input.
		/// </summary>
		private static readonly SkipParser EndOfInstruction = ~(WhiteSpaces + ~EndOfInput);

		/// <summary>
		///     Parses the given input string and returns the instruction.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		public override Reply<Instruction> Parse(InputStream inputStream)
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
			if (CvarRegistry.TryFind(name, out cvar))
				return Parse(inputStream, cvar);

			// Check if a command has been referenced and if so, return the appropriate instruction
			ICommand command;
			if (CommandRegistry.TryFind(name, out command))
				return Parse(inputStream, command);

			// If the name refers to neither a cvar nor a command, return an error message
			inputStream.State = state;
			return Message(System.String.Format("Unknown command '{0}'.", name));
		}

		/// <summary>
		///     Parses a cvar instruction.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		/// <param name="cvar">The cvar referenced by the instruction.</param>
		private Reply<Instruction> Parse(InputStream inputStream, ICvar cvar)
		{
			var cvarDisplay = EndOfInstruction.Apply(_ => new Instruction(cvar, null));

			var cvarSet = (~WhiteSpaces1 + TypeRegistry.GetParser(cvar.ValueType) + EndOfInstruction)
				.Apply(v => new Instruction(cvar, v));

			var cvarParser = Attempt(cvarDisplay) | cvarSet;
			var reply = cvarParser.Parse(inputStream);
			if (reply.Status == ReplyStatus.Success)
				return Success(reply.Result);

			var type = string.Format("Cvar type: {0}", TypeRegistry.GetDescription(cvar.ValueType));
			var examples = string.Format("Examples of valid inputs: {0}, ...", string.Join(", ", TypeRegistry.GetExamples(cvar.ValueType)));
			return ForwardError(reply, type, examples, Help.GetHint(cvar.Name));
		}

		/// <summary>
		///     Parses a command invocation.
		/// </summary>
		/// <param name="inputStream">The input stream that should be parsed.</param>
		/// <param name="command">The command referenced by the instruction.</param>
		private Reply<Instruction> Parse(InputStream inputStream, ICommand command)
		{
			var parameters = command.Parameters.ToArray();
			var values = new object[parameters.Length];

			for (var i = 0; i < parameters.Length; ++i)
			{
				// We've reached the end of the input, and all subsequent parameters should use their default value, so we're done
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
					var examples = string.Format("Examples of valid inputs: {0}, ...", string.Join(", ", TypeRegistry.GetExamples(parameters[i].Type)));

					return Errors(new ErrorMessage(ErrorType.Expected, TypeRegistry.GetDescription(parameters[i].Type)),
								  new ErrorMessage(ErrorType.Message, examples),
								  new ErrorMessage(ErrorType.Message, Help.GetHint(command.Name)));
				}

				// The argument must be separated from the previous one by at least one white space character
				if (!Char.IsWhiteSpace(inputStream.Peek()))
					return Expected("whitespace");

				inputStream.SkipWhiteSpaces();

				var reply = TypeRegistry.GetParser(parameters[i].Type).Parse(inputStream);
				if (reply.Status == ReplyStatus.Success)
					values[i] = reply.Result;
				else
				{
					var parameterType = string.Format("Parameter type: {0}", TypeRegistry.GetDescription(parameters[i].Type));
					var examples = string.Format("Examples of valid inputs: {0}, ...", string.Join(", ", TypeRegistry.GetExamples(parameters[i].Type)));
					return ForwardError(reply, parameterType, examples, Help.GetHint(command.Name));
				}
			}

			return EndOfInstruction.Apply(_ => new Instruction(command, values)).Parse(inputStream);
		}
	}
}