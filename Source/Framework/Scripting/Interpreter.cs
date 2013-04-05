using System;

namespace Pegasus.Framework.Scripting
{
	using System.Linq;
	using Parsing;

	/// <summary>
	///   Interprets user-provided input to set and view cvars and invoke commands.
	/// </summary>
	internal class Interpreter : DisposableObject
	{
		/// <summary>
		///   The name of the application.
		/// </summary>
		private readonly string _appName;

		/// <summary>
		///   The command registry that is used to look up commands.
		/// </summary>
		private readonly CommandRegistry _commands;

		/// <summary>
		///   The cvar registry that is used to look up cvars.
		/// </summary>
		private readonly CvarRegistry _cvars;

		/// <summary>
		///   The parser that is used to parse a user request.
		/// </summary>
		private readonly RequestParser _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="appName">The name of the application.</param>
		/// <param name="commands">The command registry that should be used to look up commands.</param>
		/// <param name="cvars">The cvar registry that should be used to look up cvars.</param>
		public Interpreter(string appName, CommandRegistry commands, CvarRegistry cvars)
		{
			Assert.ArgumentNotNullOrWhitespace(appName, () => appName);
			Assert.ArgumentNotNull(cvars, () => cvars);
			Assert.ArgumentNotNull(commands, () => commands);

			_appName = appName;
			_commands = commands;
			_cvars = cvars;
			_parser = new RequestParser(_commands, cvars);

			_commands.OnExecute += OnExecuteCommand;
			_commands.OnProcess += OnProcess;
			_commands.OnPersist += OnPersist;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_commands.OnExecute -= OnExecuteCommand;
			_commands.OnProcess -= OnProcess;
			_commands.OnPersist -= OnPersist;
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
		///   Invoked when the given string should be executed.
		/// </summary>
		/// <param name="input">The input that should be executed.</param>
		private void OnExecuteCommand(string input)
		{
			if (!String.IsNullOrWhiteSpace(input))
				Execute(input);
		}

		/// <summary>
		///   Invoked when the commands in the given file should be processed.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be processed.</param>
		private void OnProcess(string fileName)
		{
			var configFile = new ConfigurationFile(_parser, _appName, fileName);
			configFile.Process();
		}

		/// <summary>
		///   Invoked when the persistent cvars should be written to the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that the cvars should be written to.</param>
		private void OnPersist(string fileName)
		{
			var configFile = new ConfigurationFile(_parser,_appName, fileName);
			configFile.Persist(_cvars.Instances.Where(cvar => cvar.Persistent));
		}
	}
}