using System;

namespace Pegasus.Framework.Scripting
{
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
		///   Gets or sets the cvar's value.
		/// </summary>
		object Value { get; set; }

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
		///   Sets the cvar's current value to the deferred one.
		/// </summary>
		void SetDeferredValue();
	}
}