using System;

namespace Pegasus.Framework.Scripting
{
	using System.IO;
	using System.Linq;
	using System.Text;
	using Parsing;
	using Platform;

	/// <summary>
	///   Interprets user-provided input to set and view cvars and invoke commands.
	/// </summary>
	internal class Interpreter : DisposableObject
	{
		/// <summary>
		///   The file extension that is appended to all configuration files.
		/// </summary>
		private const string ConfigFileExtension = ".cfg";

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
		///   The parser that is used to parse the user requests.
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
			_parser = new RequestParser(cvars, _commands);

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
		}

		/// <summary>
		///   Invoked when the persistent cvars should be written to the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that the cvars should be written to.</param>
		private void OnPersist(string fileName)
		{
			if (String.IsNullOrWhiteSpace(fileName))
			{
				Log.Error("The file name cannot consist of whitespace only.");
				return;
			}

			fileName += ConfigFileExtension;
			var file = new AppFile(_appName, fileName);
			if (!file.IsValid)
			{
				Log.Error("'{0}' is not a valid file name.", fileName);
				return;
			}

			var builder = new StringBuilder();
			foreach (var cvar in _cvars.Instances.Where(cvar => cvar.Persistent))
				builder.AppendFormat("{0} {1};", cvar.Name, cvar.StringValue).AppendLine();

			try
			{
				file.Write(builder.ToString());
				Log.Info("'{0}' has been written.", fileName);
			}
			catch (IOException e)
			{
				Log.Error("Failed to persist cvars into '{0}': {1}", fileName, e.Message);
			}
		}
	}
}