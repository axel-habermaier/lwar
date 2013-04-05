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
		///   Gets or sets the cvar's value.
		/// </summary>
		object Value { get; set; }

		/// <summary>
		///   Gets the cvar's value as a string.
		/// </summary>
		string StringValue { get; }

		/// <summary>
		///   Indicates whether the cvar's value is persisted across sessions.
		/// </summary>
		bool Persistent { get; }
	}
}