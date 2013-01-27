using System;

namespace Pegasus.Framework.Scripting
{
	using Parsing;
	using Platform;

	/// <summary>
	///   Interprets user-provided input to set and view cvars and invoke commands.
	/// </summary>
	internal class Interpreter : DisposableObject
	{
		/// <summary>
		///   The parser that is used to parse the user commands.
		/// </summary>
		private static readonly RequestParser Parser = new RequestParser();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Interpreter()
		{
			Commands.Execute.Invoked += OnExecuteCommand;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.Execute.Invoked -= OnExecuteCommand;
		}

		/// <summary>
		///   Executes the given user-provided input.
		/// </summary>
		/// <param name="input">The input that should be executed.</param>
		public void Execute(string input)
		{
			Assert.ArgumentNotNullOrWhitespace(input, () => input);

			var reply = Parser.Parse(input);

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