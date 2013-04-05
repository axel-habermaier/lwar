using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;

	/// <summary>
	///   A common interface for commands with zero or more parameters.
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		///   Gets the external name of the command that is used to refer to the command in the console, for instance.
		/// </summary>
		string Name { get; }

		/// <summary>
		///   Gets a string describing the usage and the purpose of the command.
		/// </summary>
		string Description { get; }

		/// <summary>
		///   Gets the command's parameters.
		/// </summary>
		IEnumerable<CommandParameter> Parameters { get; }

		/// <summary>
		///   Invokes the command, extracting the command's parameters (if any) from the given parameters array.
		/// </summary>
		/// <param name="parameters">The parameters that should be used to invoke the command.</param>
		void Invoke(object[] parameters);
	}
}