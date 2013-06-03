using System;

namespace Pegasus.Framework.Scripting
{
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///   Explains the usage of the cvar and command system.
	/// </summary>
	internal class Help : DisposableObject
	{
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
				Log.Info("   Type 'list_cvars' to list all available cvars.");
				Log.Info("Commands:");
				Log.Info("   Type 'command-name value1 value2 ...' to invoke the command with parameters value1, value2, ... " +
						 "Optional parameters can be omitted at the end of the command invocation.");
				Log.Info("   Type 'help command-name' to view a description of the usage and purpose of the command.");
				Log.Info("   Type 'list_commands' to list all available commands.");
			}
			else if (CvarRegistry.TryFind(name, out cvar))
			{
				var deferredValue = String.Empty;
				if (cvar.UpdateMode != UpdateMode.Immediate && cvar.HasDeferredValue)
					deferredValue = String.Format(", pending update: '{0}'", TypeRepresentation.ToString(cvar.DeferredValue));

				Log.Info("'{0}' : {1} = '{2}' (default: '{3}'{5}): {4}", cvar.Name, TypeDescription.GetDescription(cvar.ValueType),
						 TypeRepresentation.ToString(cvar.Value), TypeRepresentation.ToString(cvar.DefaultValue), cvar.Description, deferredValue);
			}
			else if (CommandRegistry.TryFind(name, out command))
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
			Commands.OnHelp -= OnHelp;
		}
	}
}