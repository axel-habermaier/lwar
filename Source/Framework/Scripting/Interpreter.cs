using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;
	using System.Linq;
	using Parsing;
	using Platform.Memory;

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
		private readonly InstructionParser _parser;

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
			_parser = new InstructionParser(_commands, cvars);

			_commands.OnExecute += OnExecute;
			_commands.OnProcess += OnProcess;
			_commands.OnPersist += OnPersist;
			_commands.OnCommands += OnListCommands;
			_commands.OnCvars += OnListCvars;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_commands.OnExecute -= OnExecute;
			_commands.OnProcess -= OnProcess;
			_commands.OnPersist -= OnPersist;
			_commands.OnCommands -= OnListCommands;
			_commands.OnCvars -= OnListCvars;
		}

		/// <summary>
		///   Executes the given user-provided input.
		/// </summary>
		/// <param name="input">The input that should be executed.</param>
		private void OnExecute(string input)
		{
			Assert.ArgumentNotNull(input, () => input);

			if (String.IsNullOrWhiteSpace(input))
				return;

			var reply = _parser.Parse(input);

			if (reply.Status == ReplyStatus.Success)
				reply.Result.Execute();
			else
				Log.Error("{0}", reply.Errors.ErrorMessage);
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
			var configFile = new ConfigurationFile(_parser, _appName, fileName);
			configFile.Persist(_cvars.AllInstances.Where(cvar => cvar.Persistent));
		}

		/// <summary>
		///   Invoked when all commands with a matching name should be listed.
		/// </summary>
		/// <param name="pattern">The name pattern of the commands that should be listed.</param>
		private void OnListCommands(string pattern)
		{
			var commands = PatternMatches(_commands.AllInstances, command => command.Name, pattern).ToArray();
			if (commands.Length == 0)
				Log.Warn("No commands found matching search pattern '{0}'.", pattern);

			foreach (var command in commands)
				Log.Info("'{0}': {1}", command.Name, command.Description);
		}

		/// <summary>
		///   Invoked when all cvars with a matching name should be listed.
		/// </summary>
		/// <param name="pattern">The name pattern of the cvars that should be listed.</param>
		private void OnListCvars(string pattern)
		{
			var cvars = PatternMatches(_cvars.AllInstances, cvar => cvar.Name, pattern).ToArray();
			if (cvars.Length == 0)
				Log.Warn("No cvars found matching search pattern '{0}'.", pattern);

			foreach (var cvar in cvars)
				Log.Info("'{0}': {1}", cvar.Name, cvar.Description);
		}

		/// <summary>
		///   Returns an ordered sequence of all elements of the source sequence, whose selected property matches the given
		///   pattern.
		/// </summary>
		/// <typeparam name="T">The type of the items that should be checked.</typeparam>
		/// <param name="source">The items that should be checked.</param>
		/// <param name="selector">The selector function that selects the item property that should be checked.</param>
		/// <param name="pattern">The pattern that should be checked.</param>
		private static IEnumerable<T> PatternMatches<T>(IEnumerable<T> source, Func<T, string> selector, string pattern)
		{
			Assert.ArgumentNotNull(source, () => source);
			Assert.ArgumentNotNull(selector, () => selector);
			Assert.ArgumentNotNullOrWhitespace(pattern, () => pattern);

			if (pattern.Trim() == "*")
				return source.OrderBy(selector);

			return source.Where(item => selector(item).ToLower().Contains(pattern.ToLower())).OrderBy(selector);
		}
	}
}