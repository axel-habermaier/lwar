namespace Pegasus.Scripting
{
	using System;
	using System.Collections.Generic;
	using Platform.Logging;

	/// <summary>
	///     Represents a command with three parameters.
	/// </summary>
	public sealed class Command<T1, T2, T3> : ICommand
	{
		/// <summary>
		///     The representation the command's first parameter.
		/// </summary>
		private readonly CommandParameter _parameter1;

		/// <summary>
		///     The representation the command's second parameter.
		/// </summary>
		private readonly CommandParameter _parameter2;

		/// <summary>
		///     The representation the command's third parameter.
		/// </summary>
		private readonly CommandParameter _parameter3;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="name">The external name of the command that is used to refer to the command in the console, for instance.</param>
		/// <param name="description">A string describing the usage and the purpose of the command.</param>
		/// <param name="systemOnly">Indicates whether the command can only be invoked by the system and not via the console.</param>
		/// <param name="parameter1">The representation of the command's first parameter.</param>
		/// <param name="parameter2">The representation of the command's second parameter.</param>
		/// <param name="parameter3">The representation of the command's third parameter.</param>
		public Command(string name, string description, bool systemOnly,
					   CommandParameter parameter1, CommandParameter parameter2, CommandParameter parameter3)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.ArgumentNotNullOrWhitespace(description);

			Name = name;
			Description = description;
			SystemOnly = systemOnly;

			_parameter1 = parameter1;
			_parameter2 = parameter2;
			_parameter3 = parameter3;
		}

		/// <summary>
		///     Gets a value indicating whether the command can only be invoked by the system and not via the console.
		/// </summary>
		public bool SystemOnly { get; private set; }

		/// <summary>
		///     Gets the external name of the command that is used to refer to the command in the console, for instance.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     Gets a string describing the usage and the purpose of the command.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		///     Gets the command's parameters.
		/// </summary>
		public IEnumerable<CommandParameter> Parameters
		{
			get
			{
				yield return _parameter1;
				yield return _parameter2;
				yield return _parameter3;
			}
		}

		/// <summary>
		///     Invokes the command, extracting the command's parameters (if any) from the given parameters array.
		/// </summary>
		/// <param name="parameters">The parameters that should be used to invoke the command.</param>
		/// <param name="userInvoked">If true, indicates that the command was invoked by the user (e.g., via the console).</param>
		void ICommand.Invoke(object[] parameters, bool userInvoked)
		{
			Assert.ArgumentNotNull(parameters);
			Assert.ArgumentSatisfies(parameters.Length == 3, "Argument count mismatch.");

			if (userInvoked && SystemOnly)
				Log.Warn("'{0}' can only be invoked by the application.", Name);
			else
				Invoke((T1)parameters[0], (T2)parameters[1], (T3)parameters[2]);
		}

		/// <summary>
		///     Invokes the command.
		/// </summary>
		/// <param name="value1">The value of the first parameter of the command.</param>
		/// <param name="value2">The value of the second parameter of the command.</param>
		/// <param name="value3">The value of the third parameter of the command.</param>
		public void Invoke(T1 value1, T2 value2, T3 value3)
		{
			if (!_parameter1.Validate(value1) | !_parameter2.Validate(value2) | !_parameter3.Validate(value3))
			{
				Log.Info("{0}", Help.GetHint(Name));
				return;
			}

			if (Invoked != null)
				Invoked(value1, value2, value3);
		}

		/// <summary>
		///     Raised when the command has been invoked.
		/// </summary>
		public event Action<T1, T2, T3> Invoked;
	}
}