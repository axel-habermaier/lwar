﻿namespace Pegasus.Scripting
{
	using System;
	using System.Linq;
	using System.Text;
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///   Explains the usage of the cvar and command system.
	/// </summary>
	internal class Help : DisposableObject
	{
		/// <summary>
		///   A cached string builder instance that is used to build up the help texts.
		/// </summary>
		private readonly StringBuilder _builder = new StringBuilder();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Help()
		{
			Commands.OnHelp += OnHelp;
		}

		/// <summary>
		///   Invoked when a description of the cvar or command with the given name should be displayed.
		/// </summary>
		/// <param name="name">The name of the cvar or the command for which the help should be displayed.</param>
		private void OnHelp(string name)
		{
			Assert.ArgumentNotNull(name);

			ICvar cvar;
			ICommand command;

			name = name.Trim();
			_builder.Clear();

			if (String.IsNullOrWhiteSpace(name))
				PrintHelp();
			else if (CvarRegistry.TryFind(name, out cvar))
				PrintCvarHelp(cvar);
			else if (CommandRegistry.TryFind(name, out command))
				PrintCommandHelp(command);
			else
				Log.Error("'{0}' is neither a cvar nor a command.", name);
		}

		/// <summary>
		///   Prints the help for the console system.
		/// </summary>
		private void PrintHelp()
		{
			_builder.Append("\nUse the console to set and view cvars and to invoke commands.\n");
			_builder.Append("Cvars:\n");
			_builder.Append("   Type '<cvar-name>' to view the current value of the cvar.\n");
			_builder.Append("   Type '<cvar-name> <value>' to set a cvar to a new value.\n");
			_builder.Append("   Type 'help <cvar-name>' to view a description of the usage and purpose of the cvar.\n");
			_builder.Append("   Type 'list_cvars' to list all available cvars.\n");
			_builder.Append("Commands:\n");
			_builder.Append("   Type '<command-name> <value1> <value2> ...' to invoke the command with parameters value1, value2, ... " +
							"Optional parameters can be omitted at the end of the command invocation.\n");
			_builder.Append("   Type 'help <command-name>' to view a description of the usage and purpose of the command.\n");
			_builder.Append("   Type 'list_commands' to list all available commands.\n");

			Log.Info(_builder.ToString());
		}

		/// <summary>
		///   Prints the help for the given cvar.
		/// </summary>
		/// <param name="cvar">The cvar the help should be printed for.</param>
		private void PrintCvarHelp(ICvar cvar)
		{
			_builder.AppendFormat("\nCvar:          {0}\n", cvar.Name);
			_builder.AppendFormat("Description:   {0}\n", cvar.Description);
			_builder.AppendFormat("Type:          {0} (e.g., {1}, ...)\n", TypeRegistry.GetDescription(cvar.ValueType),
								  String.Join(", ", TypeRegistry.GetExamples(cvar.ValueType)));
			_builder.AppendFormat("Default Value: {0}\\\0\n", TypeRegistry.ToString(cvar.DefaultValue));
			_builder.AppendFormat("Current Value: {0}\\\0\n", TypeRegistry.ToString(cvar.Value));

			if (cvar.UpdateMode != UpdateMode.Immediate && cvar.HasDeferredValue)
				_builder.AppendFormat("Pending Value: {0}\\\0\n", TypeRegistry.ToString(cvar.DeferredValue));

			if (cvar.Validators.Any())
				_builder.AppendFormat("Remarks:       {0}\n", String.Join("; ", cvar.Validators.Select(v => v.Description)));

			_builder.AppendFormat("Update Mode:   {0}\n", cvar.UpdateMode.ToDisplayString());
			_builder.AppendFormat("Persistent:    {0}\n", cvar.Persistent ? "yes" : "no");
			_builder.AppendFormat("User Access:   {0}\n", cvar.SystemOnly ? "read" : "read/write");

			Log.Info(_builder.ToString());
		}

		/// <summary>
		///   Prints the help for the given command.
		/// </summary>
		/// <param name="command">The command the help should be printed for.</param>
		private void PrintCommandHelp(ICommand command)
		{
			_builder.AppendFormat("\nCommand:     {0}\n", command.Name);
			_builder.AppendFormat("Description: {0}\n", command.Description);
			_builder.AppendFormat("Invocation:  {0}", command.SystemOnly ? "application only" : "user or application");

			if (command.Parameters.Any())
				_builder.Append("\n\nParameters:\n");

			var first = true;
			foreach (var parameter in command.Parameters)
			{
				if (first)
					first = false;
				else
					_builder.Append("\n\n");

				_builder.AppendFormat("    Parameter:     {0}\n", parameter.Name);
				_builder.AppendFormat("    Description:   {0}\n", parameter.Description);
				_builder.AppendFormat("    Type:          {0} (e.g., {1}, ...)", TypeRegistry.GetDescription(parameter.Type),
									  String.Join(", ", TypeRegistry.GetExamples(parameter.Type)));

				if (parameter.Validators.Any())
					_builder.AppendFormat("\n    Remarks:       {0}", String.Join("; ", parameter.Validators.Select(v => v.Description)));

				if (parameter.HasDefaultValue)
					_builder.AppendFormat("\n    Default Value: {0}", TypeRegistry.ToString(parameter.DefaultValue));
			}

			_builder.Append("\n");
			Log.Info(_builder.ToString());
		}

		/// <summary>
		///   Gets a string that contains a hint about the usage of help.
		/// </summary>
		/// <param name="name">The cvar or command name that should be used in the hint.</param>
		public static string GetHint(string name)
		{
			return String.Format("Use 'help {0}' for details about the usage of cvar or command '{0}'.", name);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.OnHelp -= OnHelp;
		}
	}
}