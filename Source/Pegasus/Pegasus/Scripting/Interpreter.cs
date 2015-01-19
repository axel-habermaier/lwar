namespace Pegasus.Scripting
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Parsing;
	using Platform;
	using Platform.Logging;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Interprets user-provided input to set and view cvars and invoke commands.
	/// </summary>
	internal class Interpreter : DisposableObject
	{
		/// <summary>
		///     The parser that is used to parse a user request.
		/// </summary>
		private readonly InstructionParser _parser;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Interpreter()
		{
			_parser = new InstructionParser();

			Commands.OnExecute += OnExecute;
			Commands.OnProcess += OnProcess;
			Commands.OnPersist += OnPersist;
			Commands.OnListCommands += OnListCommands;
			Commands.OnListCvars += OnListCvars;
			Commands.OnReset += OnResetCvar;
			Commands.OnPrintAppInfo += OnPrintAppInfo;
			Commands.OnToggle += OnToggle;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
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
			Commands.OnToggle -= OnToggle;
		}

		/// <summary>
		///     Toggles the value of a Boolean console variable.
		/// </summary>
		/// <param name="name">The name of the console variable whose value should be toggled.</param>
		private static void OnToggle(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);

			ICvar cvar;
			if (!CvarRegistry.TryFind(name, out cvar))
				Log.Warn("Unknown cvar '{0}'.", name);
			else
			{
				if (cvar.ValueType != typeof(bool))
					Log.Warn("Cvar '{0}' is not of type {1}.", name, TypeRegistry.GetDescription<bool>());
				else
					cvar.SetValue(!(bool)cvar.Value, true);
			}
		}

		/// <summary>
		///     Prints information about the application.
		/// </summary>
		private static void OnPrintAppInfo()
		{
			StringBuilder builder;
			using (StringBuilderPool.Allocate(out builder))
			{
				builder.AppendFormat("\nApplication Name:     {0}\n", Application.Current.Name);
				builder.AppendFormat("Operating System:     {0}\n", PlatformInfo.Platform);
				builder.AppendFormat("CPU Architecture:     {0}\n", IntPtr.Size == 8 ? "x64" : "x86");
				builder.AppendFormat("Graphics API:         {0}\n", Cvars.GraphicsApi);
				builder.AppendFormat("User File Directory:  {0}\n", FileSystem.UserDirectory);

				Log.Info("{0}", builder);
			}
		}

		/// <summary>
		///     Resets the cvar with the given name to its default value.
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
		///     Executes the given user-provided input.
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
		///     Invoked when the commands in the given file should be processed.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be processed.</param>
		private void OnProcess(string fileName)
		{
			var configFile = new ConfigurationFile(_parser, fileName);
			configFile.Process();
		}

		/// <summary>
		///     Invoked when the persistent cvars should be written to the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that the cvars should be written to.</param>
		private void OnPersist(string fileName)
		{
			var configFile = new ConfigurationFile(_parser, fileName);
			configFile.Persist(CvarRegistry.All.Where(cvar => cvar.Persistent && cvar.HasExplicitValue));
		}

		/// <summary>
		///     Invoked when all commands with a matching name should be listed.
		/// </summary>
		/// <param name="pattern">The name pattern of the commands that should be listed.</param>
		private static void OnListCommands(string pattern)
		{
			ListElements(CommandRegistry.All, pattern, command => command.Name, command => command.Description);
		}

		/// <summary>
		///     Invoked when all cvars with a matching name should be listed.
		/// </summary>
		/// <param name="pattern">The name pattern of the cvars that should be listed.</param>
		private static void OnListCvars(string pattern)
		{
			ListElements(CvarRegistry.All, pattern, cvar => cvar.Name, cvar => cvar.Description);
		}

		/// <summary>
		///     Lists all elements matching the given predicate.
		/// </summary>
		/// <typeparam name="T">The type of the elements that should be shown.</typeparam>
		/// <param name="source">The elements that should be shown.</param>
		/// <param name="pattern">The pattern that should be checked.</param>
		/// <param name="name">A selector that returns the name of an element.</param>
		/// <param name="description">A selector that returns the description of an element.</param>
		private static void ListElements<T>(IEnumerable<T> source, string pattern, Func<T, string> name, Func<T, string> description)
		{
			Assert.ArgumentNotNull(source);
			Assert.ArgumentNotNull(pattern);
			Assert.ArgumentNotNull(name);
			Assert.ArgumentNotNull(description);

			var elements = PatternMatches(source, name, pattern).ToArray();
			if (elements.Length == 0)
			{
				Log.Warn("No elements found matching search pattern '{0}'.", pattern);
				return;
			}

			StringBuilder builder;
			using (StringBuilderPool.Allocate(out builder))
			{
				builder.Append("\n");
				foreach (var element in elements)
					builder.AppendFormat("'\\lightgrey{0}\\\0': {1}\n", name(element), description(element));

				Log.Info("{0}", builder);
			}
		}

		/// <summary>
		///     Returns an ordered sequence of all elements of the source sequence, whose selected property matches the given
		///     pattern.
		/// </summary>
		/// <typeparam name="T">The type of the items that should be checked.</typeparam>
		/// <param name="source">The items that should be checked.</param>
		/// <param name="selector">The selector function that selects the item property that should be used for pattern matching.</param>
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