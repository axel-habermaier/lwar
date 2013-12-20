namespace Pegasus.Scripting
{
	using System;
	using System.Collections.Generic;
	using Validators;

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
		///   Gets the validators that are used to validate the values of the cvar.
		/// </summary>
		IEnumerable<ValidatorAttribute> Validators { get; }

		/// <summary>
		///   Gets a string describing the usage and the purpose of the cvar.
		/// </summary>
		string Description { get; }

		/// <summary>
		///   Gets the cvar's value.
		/// </summary>
		object Value { get; }

		/// <summary>
		///   Gets the deferred value of the cvar that will be set the next time it is updated. This property has no meaning
		///   if the cvar's update mode is immediate.
		/// </summary>
		object DeferredValue { get; }

		/// <summary>
		///   Gets the cvar's default value.
		/// </summary>
		object DefaultValue { get; }

		/// <summary>
		///   Indicates whether the cvar's value is persisted across sessions.
		/// </summary>
		bool Persistent { get; }

		/// <summary>
		///   Gets a value indicating whether the cvar has a deferred update pending.
		/// </summary>
		bool HasDeferredValue { get; }

		/// <summary>
		///   Gets the update mode of the cvar.
		/// </summary>
		UpdateMode UpdateMode { get; }

		/// <summary>
		///   Gets a value indicating whether the cvar can only be set by the system and not via the console.
		/// </summary>
		bool SystemOnly { get; }

		/// <summary>
		///   Sets the cvar's value.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		/// <param name="setByUser">If true, indicates that the cvar was set by the user (e.g., via the console).</param>
		void SetValue(object value, bool setByUser);

		/// <summary>
		///   Sets the cvar's current value to the deferred one.
		/// </summary>
		void SetDeferredValue();
	}
}