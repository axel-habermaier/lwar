using System;

namespace Pegasus.Framework.Scripting
{
	/// <summary>
	///   Explains the usage of the cvar and command system.
	/// </summary>
	internal class Help : DisposableObject
	{
		/// <summary>
		///   The command registry that is used to look up commands.
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
		/// <param name="cvars">The command registry that should be used to look up commands.</param>
		public Help(CommandRegistry commands, CvarRegistry cvars)
		{
			Assert.ArgumentNotNull(commands, () => commands);
			Assert.ArgumentNotNull(cvars, () => cvars);

			_commands = commands;
			_cvars = cvars;

			_commands.OnHelp += OnHelp;
		}

		/// <summary>
		///   Invoked when a description of the cvar or command with the given name should be displayed.
		/// </summary>
		/// <param name="name">The name of the cvar or the command for which the help should be displayed.</param>
		private void OnHelp(string name)
		{
			ICvar cvar;
			ICommand command;

			name = name.Trim();
			if (String.IsNullOrWhiteSpace(name))
			{
				Log.Info("Use the console to set and view cvars and to invoke commands.");
				Log.Info("Cvars:");
				Log.Info("   Type 'cvar-name' to view the current value of the cvar.");
				Log.Info("   Type 'cvar-name value' to set a cvar to a new value.");
				Log.Info("   Type 'help cvar-name' to view a description of the usage and purpose of the cvar.");
				Log.Info("Commands:");
				Log.Info("   Type 'command-name value1 value2 ...' to invoke the command with parameters value1, value2, ... " +
						 "Optional parameters can be omitted at the end of the command invocation.");
				Log.Info("   Type 'help command-name' to view a description of the usage and purpose of the command.");
			}
			else if (_cvars.TryFind(name, out cvar))
			{
				Log.Info("'{0}' : {1} = '{2}' (default: '{3}'): {4}", cvar.Name, TypeDescription.GetDescription(cvar.ValueType),
						 cvar.StringValue, cvar.DefaultValue, cvar.Description);
			}
			else if (_commands.TryFind(name, out command))
			{
				Log.Info("'{0}': {1}", command.Name, command.Description);
				foreach (var parameter in command.Parameters)
				{
					var type = TypeDescription.GetDescription(parameter.Type);
					var defaultValue = String.Empty;
					if (parameter.HasDefaultValue)
						defaultValue = String.Format(" = '{0}'", TypeRepresentation.ToString(parameter.DefaultValue));

					Log.Info("    {0} : [{1}]{3}  {2}", parameter.Name, type, parameter.Description, defaultValue);
				}
			}
			else
				Log.Error("'{0}' is not a cvar or command.", name);
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
			_commands.OnHelp -= OnHelp;
		}
	}
}