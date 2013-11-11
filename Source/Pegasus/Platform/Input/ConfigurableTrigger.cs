namespace Pegasus.Platform.Input
{
	using System;
	using Scripting;

	/// <summary>
	///   Represents a trigger with the actual trigger evaluation being performed by a trigger held by a cvar. The cvar's value
	///   can be changed at any time, changing the behavior of the configurable trigger accordingly.
	/// </summary>
	internal class ConfigurableTrigger : InputTrigger
	{
		/// <summary>
		///   The cvar that holds the actual trigger.
		/// </summary>
		private readonly Cvar<InputTrigger> _trigger;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="trigger">The cvar that holds the actual trigger.</param>
		internal ConfigurableTrigger(Cvar<InputTrigger> trigger)
		{
			_trigger = trigger;
		}

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		///   true if the current object is equal to other; otherwise, false.
		/// </returns>
		public override bool Equals(InputTrigger other)
		{
			var trigger = other as ConfigurableTrigger;
			if (trigger == null)
				return false;

			return _trigger == trigger._trigger;
		}

		/// <summary>
		///   Evaluates the trigger, returning true to indicate that the trigger has fired.
		/// </summary>
		/// <param name="device">The logical input device that should be used to evaluate the trigger.</param>
		internal override bool Evaluate(LogicalInputDevice device)
		{
			return _trigger.Value.Evaluate(device);
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("Cvar({0}: {1})", _trigger.Name, _trigger.Value);
		}
	}
}