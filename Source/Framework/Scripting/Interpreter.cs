using System;

namespace Pegasus.Framework.Scripting
{
	using Parsing;

	/// <summary>
	///   Interprets user-provided input to set and view cvars and invoke commands.
	/// </summary>
	internal class Interpreter : DisposableObject
	{
		/// <summary>
		///   The command registry that is used to look up commands referenced by a user request.
		/// </summary>
		private readonly CommandRegistry _commandRegistry;

		/// <summary>
		///   The parser that is used to parse the user requests.
		/// </summary>
		private readonly RequestParser _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="cvarRegistry">The cvar registry that should be used to look up cvars referenced by a user request.</param>
		/// <param name="commandRegistry">
		///   The command registry that should be used to look up commands referenced by a user request.
		/// </param>
		public Interpreter(CvarRegistry cvarRegistry, CommandRegistry commandRegistry)
		{
			Assert.ArgumentNotNull(cvarRegistry, () => cvarRegistry);
			Assert.ArgumentNotNull(commandRegistry, () => commandRegistry);

			_commandRegistry = commandRegistry;
			_parser = new RequestParser(cvarRegistry, _commandRegistry);

			_commandRegistry.OnExecute += OnExecuteCommand;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_commandRegistry.OnExecute -= OnExecuteCommand;
		}

		/// <summary>
		///   Executes the given user-provided input.
		/// </summary>
		/// <param name="input">The input that should be executed.</param>
		public void Execute(string input)
		{
			Assert.ArgumentNotNullOrWhitespace(input, () => input);

			var reply = _parser.Parse(input);

			if (reply.Status == ReplyStatus.Success)
				reply.Result.Execute();
			else
				Log.Error(reply.Errors.ErrorMessage);
		}

		/// <summary>
		///   Invoked when the execute command is used.
		/// </summary>
		/// <param name="input">The input that should be executed.</param>
		private void OnExecuteCommand(string input)
		{
			if (!String.IsNullOrWhiteSpace(input))
				Execute(input);
		}
	}
}