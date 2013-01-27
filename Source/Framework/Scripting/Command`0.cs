using System;

namespace Pegasus.Framework.Scripting
{
	using Platform;

	/// <summary>
	///   Represents a parameterless command.
	/// </summary>
	public sealed class Command : ICommand
	{
		/// <summary>
		///   The types of the command's parameters. This is an empty array in this case, as the command does not have any
		///   parameters at all.
		/// </summary>
		private static readonly Type[] CachedParameterTypes = new Type[0];

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The external name of the command that is used to refer to the command in the console, for instance.</param>
		/// <param name="description">A string describing the usage and the purpose of the command.</param>
		public Command(string name, string description)
		{
			Assert.ArgumentNotNullOrWhitespace(name, () => name);
			Assert.ArgumentNotNullOrWhitespace(description, () => description);

			Name = name;
			Description = description;

			CommandRegistry.Register(this);
		}

		/// <summary>
		///   Gets the types of the command's parameters.
		/// </summary>
		Type[] ICommand.ParameterTypes
		{
			get { return CachedParameterTypes; }
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
		///   Gets a representation of the command's signature, e.g. Name [type of parameter 1] [type of parameter 2] ...
		/// </summary>
		public string Signature
		{
			get { return String.Format("{0} [no parameters]", Name); }
		}

		/// <summary>
		///   Invokes the command, extracting the command's parameters (if any) from the given parameters array.
		/// </summary>
		/// <param name="parameters">The parameters that should be used to invoke the command.</param>
		void ICommand.Invoke(object[] parameters)
		{
			Assert.ArgumentNotNull(parameters, () => parameters);
			Assert.ArgumentSatisfies(parameters.Length == 0, () => parameters, "Argument count mismatch.");
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