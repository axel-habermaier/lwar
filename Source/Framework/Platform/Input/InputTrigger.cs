using System;

namespace Pegasus.Framework.Platform.Input
{
	/// <summary>
	///   Represents an input trigger that is triggered if a logical input device is in a certain state.
	/// </summary>
	public abstract class InputTrigger : IEquatable<InputTrigger>
	{
		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		public abstract bool Equals(InputTrigger other);

		/// <summary>
		///   Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var trigger = obj as InputTrigger;
			if (trigger == null)
				return false;

			return Equals(trigger);
		}

		/// <summary>
		///   Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///   Evaluates the trigger, returning true to indicate that the trigger has fired.
		/// </summary>
		/// <param name="device">The logical input device that should be used to evaluate the trigger.</param>
		internal abstract bool Evaluate(LogicalInputDevice device);

		/// <summary>
		///   Sets the logical input device the logical input is currently registered on.
		/// </summary>
		/// <param name="device">
		///   The logical input device the logical input is currently registered on. Null should be passed to
		///   indicate that the logical input is currently not registered on any device.
		/// </param>
		internal virtual void IsRegisteredOn(LogicalInputDevice device)
		{
		}

		/// <summary>
		///   Constructs a chord, i.e., a trigger that triggers if and only if both of its constituting triggers trigger.
		/// </summary>
		/// <param name="left">The first sub-trigger.</param>
		/// <param name="right">The second sub-trigger.</param>
		public static InputTrigger operator &(InputTrigger left, InputTrigger right)
		{
			Assert.ArgumentNotNull(left, () => left);
			Assert.ArgumentNotNull(right, () => right);

			return new BinaryInputTrigger(BinaryInputTriggerType.Chord, left, right);
		}

		/// <summary>
		///   Constructs a chord that triggers only for the first frame in which both of its sub-triggers trigger. The chord
		///   triggers again only after at least one of its two sub-triggers has not triggered for the duration of at least one
		///   frame.
		/// </summary>
		/// <param name="left">The first sub-trigger.</param>
		/// <param name="right">The second sub-trigger.</param>
		public static InputTrigger operator +(InputTrigger left, InputTrigger right)
		{
			Assert.ArgumentNotNull(left, () => left);
			Assert.ArgumentNotNull(right, () => right);

			return new BinaryInputTrigger(BinaryInputTriggerType.ChordOnce, left, right);
		}

		/// <summary>
		///   Constructs an input alias, i.e., a trigger that triggers if and only if at least one of its two constituting triggers
		///   triggers.
		/// </summary>
		/// <param name="left">The first sub-trigger.</param>
		/// <param name="right">The second sub-trigger.</param>
		public static InputTrigger operator |(InputTrigger left, InputTrigger right)
		{
			Assert.ArgumentNotNull(left, () => left);
			Assert.ArgumentNotNull(right, () => right);

			return new BinaryInputTrigger(BinaryInputTriggerType.Alias, left, right);
		}
	}
}