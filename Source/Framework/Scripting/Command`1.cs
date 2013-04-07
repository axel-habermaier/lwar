﻿using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;

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
		/// <param name="parameter">The representation the command's parameter.</param>
		public Command(string name, string description, CommandParameter parameter)
		{
			Assert.ArgumentNotNullOrWhitespace(name, () => name);
			Assert.ArgumentNotNullOrWhitespace(description, () => description);

			Name = name;
			Description = description;
			_parameter = parameter;
		}

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
		void ICommand.Invoke(object[] parameters)
		{
			Assert.ArgumentNotNull(parameters, () => parameters);
			Assert.ArgumentSatisfies(parameters.Length == 1, () => parameters, "Argument count mismatch.");

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