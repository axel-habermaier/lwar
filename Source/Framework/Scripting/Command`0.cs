using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;
	using System.Linq;

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
		public Command(string name, string description)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.ArgumentNotNullOrWhitespace(description);

			Name = name;
			Description = description;
		}

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
		void ICommand.Invoke(object[] parameters)
		{
			Assert.ArgumentNotNull(parameters);
			Assert.ArgumentSatisfies(parameters.Length == 0, "Argument count mismatch.");

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