using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Parsing;
	using Platform;
	using Platform.Logging;
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
		///   The parser that is used to parse a user request.
		/// </summary>
		private readonly InstructionParser _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="appName">The name of the application.</param>
		public Interpreter(string appName)
		{
			Assert.ArgumentNotNullOrWhitespace(appName);

			_appName = appName;
			_parser = new InstructionParser();

			Commands.OnExecute += OnExecute;
			Commands.OnProcess += OnProcess;
			Commands.OnPersist += OnPersist;
			Commands.OnListCommands += OnListCommands;
			Commands.OnListCvars += OnListCvars;
			Commands.OnReset += OnResetCvar;
			Commands.OnPrintAppInfo += OnPrintAppInfo;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.OnExecute -= OnExecute;
			Commands.OnProcess -= OnProcess;
			Commands.OnPersist -= OnPersist;
			Commands.OnListCommands -= OnListCommands;
			Commands.OnListCvars -= OnListCvars;
			Commands.OnReset -= OnResetCvar;
			Commands.OnPrintAppInfo -= OnPrintAppInfo;
		}

		/// <summary>
		///   Prints information about the application.
		/// </summary>
		private void OnPrintAppInfo()
		{
			var builder = new StringBuilder();
			builder.AppendFormat("Application Name: {0}\n", _appName);
			builder.AppendFormat("Operating System: {0}\n", PlatformInfo.Platform);
			builder.AppendFormat("CPU architecture: {0}\n", IntPtr.Size == 8 ? "x64" : "x86");
			builder.AppendFormat("Graphics API:     {0}\n", PlatformInfo.GraphicsApi);
			builder.AppendFormat("File Path:        {0}", AppFile.Folder);

			Log.Info("{0}", builder);
		}

		/// <summary>
		///   Resets the cvar with the given name to its default value.
		/// </summary>
		/// <param name="name">The name of the cvar that should be reset to its default value.</param>
		private static void OnResetCvar(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);

			ICvar cvar;
			if (!CvarRegistry.TryFind(name, out cvar))
				Log.Warn("Unknown cvar '{0}'.", name);
			else
				cvar.SetValue(cvar.DefaultValue, true);
		}

		/// <summary>
		///   Executes the given user-provided input.
		/// </summary>
		/// <param name="input">The input that should be executed.</param>
		private void OnExecute(string input)
		{
			Assert.ArgumentNotNull(input);

			if (String.IsNullOrWhiteSpace(input))
				return;

			var reply = _parser.Parse(input);

			if (reply.Status == ReplyStatus.Success)
				reply.Result.Execute(true);
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
			configFile.Persist(CvarRegistry.All.Where(cvar => cvar.Persistent));
		}

		/// <summary>
		///   Invoked when all commands with a matching name should be listed.
		/// </summary>
		/// <param name="pattern">The name pattern of the commands that should be listed.</param>
		private static void OnListCommands(string pattern)
		{
			var commands = PatternMatches(CommandRegistry.All, command => command.Name, pattern).ToArray();
			if (commands.Length == 0)
				Log.Warn("No commands found matching search pattern '{0}'.", pattern);

			foreach (var command in commands)
				Log.Info("'{0}': {1}", command.Name, command.Description);
		}

		/// <summary>
		///   Invoked when all cvars with a matching name should be listed.
		/// </summary>
		/// <param name="pattern">The name pattern of the cvars that should be listed.</param>
		private static void OnListCvars(string pattern)
		{
			var cvars = PatternMatches(CvarRegistry.All, cvar => cvar.Name, pattern).ToArray();
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
			Assert.ArgumentNotNull(source);
			Assert.ArgumentNotNull(selector);
			Assert.ArgumentNotNullOrWhitespace(pattern);

			if (pattern.Trim() == "*")
				return source.OrderBy(selector);

			return source.Where(item => selector(item).ToLower().Contains(pattern.ToLower())).OrderBy(selector);
		}
	}
}