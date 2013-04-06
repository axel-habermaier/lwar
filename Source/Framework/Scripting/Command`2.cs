using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;

	/// <summary>
	///   Represents a command with two parameters.
	/// </summary>
	public sealed class Command<T1, T2> : ICommand
	{
		/// <summary>
		///   The representation the command's first parameter.
		/// </summary>
		private readonly CommandParameter _parameter1;

		/// <summary>
		///   The representation the command's second parameter.
		/// </summary>
		private readonly CommandParameter _parameter2;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The external name of the command that is used to refer to the command in the console, for instance.</param>
		/// <param name="description">A string describing the usage and the purpose of the command.</param>
		/// <param name="parameter1">The representation of the command's first parameter.</param>
		/// <param name="parameter2">The representation of the command's second parameter.</param>
		public Command(string name, string description, CommandParameter parameter1, CommandParameter parameter2)
		{
			Assert.ArgumentNotNullOrWhitespace(name, () => name);
			Assert.ArgumentNotNullOrWhitespace(description, () => description);

			Name = name;
			Description = description;

			_parameter1 = parameter1;
			_parameter2 = parameter2;
		}

		/// <summary>
		///   Gets the command's parameters.
		/// </summary>
		public IEnumerable<CommandParameter> Parameters
		{
			get
			{
				yield return _parameter1;
				yield return _parameter2;
			}
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
		void ICommand.Invoke(object[] parameters)
		{
			Assert.ArgumentNotNull(parameters, () => parameters);
			Assert.ArgumentSatisfies(parameters.Length == 2, () => parameters, "Argument count mismatch.");

			Invoke((T1)parameters[0], (T2)parameters[1]);
		}

		/// <summary>
		///   Invokes the command.
		/// </summary>
		/// <param name="value1">The value of the first parameter of the command.</param>
		/// <param name="value2">The value of the second parameter of the command.</param>
		public void Invoke(T1 value1, T2 value2)
		{
			if (!_parameter1.Validate(value1) | !_parameter2.Validate(value2))
			{
				Log.Info(Help.GetHint(Name));
				return;
			}

			if (Invoked != null)
				Invoked(value1, value2);
		}

		/// <summary>
		///   Raised when the command has been invoked.
		/// </summary>
		public event Action<T1, T2> Invoked;
	}
}