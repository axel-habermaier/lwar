using System;

namespace Pegasus.Scripting
{
	using System.Collections.Generic;
	using System.Linq;
	using Platform.Logging;

	/// <summary>
	///   Represents a parameterless command.
	/// </summary>
	public sealed class Command : ICommand
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The external name of the command that is used to refer to the command in the console, for instance.</param>
		/// <param name="description">A string describing the usage and the purpose of the command.</param>
		/// <param name="systemOnly">Indicates whether the command can only be invoked by the system and not via the console.</param>
		public Command(string name, string description, bool systemOnly)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.ArgumentNotNullOrWhitespace(description);

			Name = name;
			Description = description;
			SystemOnly = systemOnly;
		}

		/// <summary>
		///   Gets a value indicating whether the command can only be invoked by the system and not via the console.
		/// </summary>
		public bool SystemOnly { get; private set; }

		/// <summary>
		///   Gets the command's parameters.
		/// </summary>
		public IEnumerable<CommandParameter> Parameters
		{
			get { return Enumerable.Empty<CommandParameter>(); }
		}

		/// <summary>
		///   Gets the external name of the command that is used to refer to the command in the console, for instance.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets a string describing the usage and the purpose of the command.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		///   Invokes the command, extracting the command's parameters (if any) from the given parameters array.
		/// </summary>
		/// <param name="parameters">The parameters that should be used to invoke the command.</param>
		/// <param name="userInvoked">If true, indicates that the command was invoked by the user (e.g., via the console).</param>
		void ICommand.Invoke(object[] parameters, bool userInvoked)
		{
			Assert.ArgumentNotNull(parameters);
			Assert.ArgumentSatisfies(parameters.Length == 0, "Argument count mismatch.");

			if (userInvoked && SystemOnly)
				Log.Warn("'{0}' can only be invoked by the application.", Name);
			else
				Invoke();
		}

		/// <summary>
		///   Invokes the command.
		/// </summary>
		public void Invoke()
		{
			if (Invoked != null)
				Invoked();
		}

		/// <summary>
		///   Raised when the command has been invoked.
		/// </summary>
		public event Action Invoked;
	}
}