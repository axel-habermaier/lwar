using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;
	using Platform.Logging;

	/// <summary>
	///   Represents a command with one parameter.
	/// </summary>
	public sealed class Command<T> : ICommand
	{
		/// <summary>
		///   The representation the command's parameter.
		/// </summary>
		private readonly CommandParameter _parameter;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The external name of the command that is used to refer to the command in the console, for instance.</param>
		/// <param name="description">A string describing the usage and the purpose of the command.</param>
		/// <param name="systemOnly">Indicates whether the command can only be invoked by the system and not via the console.</param>
		/// <param name="parameter">The representation the command's parameter.</param>
		public Command(string name, string description, bool systemOnly, CommandParameter parameter)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.ArgumentNotNullOrWhitespace(description);

			Name = name;
			Description = description;
			SystemOnly = systemOnly;

			_parameter = parameter;
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
			get { yield return _parameter; }
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
			Assert.ArgumentSatisfies(parameters.Length == 1, "Argument count mismatch.");

			if (userInvoked && SystemOnly)
				Log.Warn("'{0}' can only be invoked by the application.", Name);
			else
				Invoke((T)parameters[0]);
		}

		/// <summary>
		///   Invokes the command.
		/// </summary>
		/// <param name="value">The value of the parameter of the command.</param>
		public void Invoke(T value)
		{
			if (!_parameter.Validate(value))
			{
				Log.Info("{0}", Help.GetHint(Name));
				return;
			}

			if (Invoked != null)
				Invoked(value);
		}

		/// <summary>
		///   Raised when the command has been invoked.
		/// </summary>
		public event Action<T> Invoked;
	}
}