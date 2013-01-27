using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;

	/// <summary>
	///   Provides access to all command instances.
	/// </summary>
	public static class CommandRegistry
	{
		/// <summary>
		///   The command instances.
		/// </summary>
		private static readonly Dictionary<string, ICommand> RegisteredCommands = new Dictionary<string, ICommand>();

		/// <summary>
		///   Registers the given command.
		/// </summary>
		/// <param name="command">The command that should be registered.</param>
		internal static void Register(ICommand command)
		{
			Assert.ArgumentNotNull(command, () => command);
			Assert.ArgumentSatisfies(!String.IsNullOrWhiteSpace(command.Name), () => command, "Invalid command name.");
			Assert.That(CvarRegistry.Find(command.Name) == null, "A cvar with the same name already exists.");
			Assert.That(!RegisteredCommands.ContainsKey(command.Name), "A command called '{0}' has already been registered.",
						command.Name);

			RegisteredCommands.Add(command.Name, command);
		}

		/// <summary>
		///   Finds the command instance with the given name. Returns null if no such command exists.
		/// </summary>
		/// <param name="name">The name of the command that should be returned.</param>
		internal static ICommand Find(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name, () => name);

			ICommand command;
			if (!RegisteredCommands.TryGetValue(name, out command))
				return null;

			return command;
		}
	}
}