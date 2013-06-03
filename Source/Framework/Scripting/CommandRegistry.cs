using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	///   Provides access to all commands.
	/// </summary>
	public static class CommandRegistry
	{
		/// <summary>
		///   The registered commands.
		/// </summary>
		private static readonly Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();

		/// <summary>
		///   Gets all registered commands.
		/// </summary>
		internal static IEnumerable<ICommand> All
		{
			get { return Commands.Values; }
		}

		/// <summary>
		///   Registers the given command.
		/// </summary>
		/// <param name="command">The command that should be registered.</param>
		public static void Register(ICommand command)
		{
			Assert.ArgumentNotNull(command);
			Assert.NotNullOrWhitespace(command.Name);
			Assert.That(!Commands.ContainsKey(command.Name), "A command with the name '{0}' has already been registered.", command.Name);
			Assert.That(CvarRegistry.All.All(cvar => cvar.Name != command.Name),
						"A cvar with the name '{0}' has already been registered.", command.Name);

			Commands.Add(command.Name, command);
		}

		/// <summary>
		///   Finds the command with the given name. Returns false if no such command is found.
		/// </summary>
		/// <param name="name">The name of the command that should be returned.</param>
		/// <param name="command">The command with the given name, if it is found.</param>
		internal static bool TryFind(string name, out ICommand command)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			return Commands.TryGetValue(name, out command);
		}

		/// <summary>
		///   Resolves the command reference to the parameterless command with given name.
		/// </summary>
		/// <param name="name">The name of the command that should be returned.</param>
		public static Command Resolve(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.That(Commands.ContainsKey(name), "Could not resolve command '{0}'. Rerun the T4 templates.", name);
			Assert.That(Commands[name] is Command, "Command '{0}' has an unexpected type. Rerun the T4 templates.", name);

			return Commands[name] as Command;
		}

		/// <summary>
		///   Resolves the command reference to the command with the given parameter type and name.
		/// </summary>
		/// <param name="name">The name of the command that should be returned.</param>
		public static Command<T> Resolve<T>(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.That(Commands.ContainsKey(name), "Could not resolve command '{0}'. Rerun the T4 templates.", name);
			Assert.That(Commands[name] is Command<T>, "Command '{0}' has an unexpected type. Rerun the T4 templates.", name);

			return Commands[name] as Command<T>;
		}

		/// <summary>
		///   Resolves the command reference to the command with the given parameter types and name.
		/// </summary>
		/// <param name="name">The name of the command that should be returned.</param>
		public static Command<T1, T2> Resolve<T1, T2>(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.That(Commands.ContainsKey(name), "Could not resolve command '{0}'. Rerun the T4 templates.", name);
			Assert.That(Commands[name] is Command<T1, T2>, "Command '{0}' has an unexpected type. Rerun the T4 templates.", name);

			return Commands[name] as Command<T1, T2>;
		}

		/// <summary>
		///   Resolves the command reference to the command with the given parameter types and name.
		/// </summary>
		/// <param name="name">The name of the command that should be returned.</param>
		public static Command<T1, T2, T3> Resolve<T1, T2, T3>(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.That(Commands.ContainsKey(name), "Could not resolve command '{0}'. Rerun the T4 templates.", name);
			Assert.That(Commands[name] is Command<T1, T2, T3>, "Command '{0}' has an unexpected type. Rerun the T4 templates.", name);

			return Commands[name] as Command<T1, T2, T3>;
		}
	}
}