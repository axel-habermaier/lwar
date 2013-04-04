using System;

namespace Pegasus.Framework.Scripting
{
	using Platform;

	/// <summary>
	///   Represents a command with three parameters.
	/// </summary>
	public sealed class Command<T1, T2, T3> : ICommand
	{
		/// <summary>
		///   The types of the command's parameters.
		/// </summary>
		private static readonly Type[] CachedParameterTypes = new[] { typeof(T1), typeof(T2), typeof(T3) };

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
		///   Gets the types of the command's parameters.
		/// </summary>
		Type[] ICommand.ParameterTypes
		{
			get { return CachedParameterTypes; }
		}

		/// <summary>
		///   Invokes the command, extracting the command's parameters (if any) from the given parameters array.
		/// </summary>
		/// <param name="parameters">The parameters that should be used to invoke the command.</param>
		void ICommand.Invoke(object[] parameters)
		{
			Assert.ArgumentNotNull(parameters, () => parameters);
			Assert.ArgumentSatisfies(parameters.Length == 3, () => parameters, "Argument count mismatch.");
			Invoke((T1)parameters[0], (T2)parameters[1], (T3)parameters[2]);
		}

		/// <summary>
		///   Gets a representation of the command's signature, e.g. Name [type of parameter 1] [type of parameter 2] ...
		/// </summary>
		public string Signature
		{
			get
			{
				return String.Format("{0} [{1}] [{2}] [{3}]", Name, TypeDescription.GetDescription<T1>(),
									 TypeDescription.GetDescription<T2>(),
									 TypeDescription.GetDescription<T3>());
			}
		}

		/// <summary>
		///   Invokes the command.
		/// </summary>
		/// <param name="parameter1">The value of the first parameter of the command.</param>
		/// <param name="parameter2">The value of the second parameter of the command.</param>
		/// <param name="parameter3">The value of the third parameter of the command.</param>
		public void Invoke(T1 parameter1, T2 parameter2, T3 parameter3)
		{
			if (Invoked != null)
				Invoked(parameter1, parameter2, parameter3);
		}

		/// <summary>
		///   Raised when the command has been invoked.
		/// </summary>
		public event Action<T1, T2, T3> Invoked;
	}
}