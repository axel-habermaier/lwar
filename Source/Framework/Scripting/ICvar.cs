using System;

namespace Pegasus.Framework.Scripting
{
	/// <summary>
	///   Represents a configurable value.
	/// </summary>
	public interface ICvar
	{
		/// <summary>
		///   Gets the external name of the cvar that is used to refer to the cvar in the console, for instance.
		/// </summary>
		string Name { get; }

		/// <summary>
		///   Gets the type of the cvar's value.
		/// </summary>
		Type ValueType { get; }

		/// <summary>
		///   Gets a string describing the usage and the purpose of the cvar.
		/// </summary>
		string Description { get; }

		/// <summary>
		///   Sets the cvar's value to the given value.
		/// </summary>
		/// <param name="value">The new value of the cvar.</param>
		void SetValue(object value);
	}
}