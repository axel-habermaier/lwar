using System;

namespace Pegasus.Platform.Input
{
	/// <summary>
	///   Represents a binary trigger that combines two sub-triggers.
	/// </summary>
	internal sealed class BinaryInputTrigger : InputTrigger
	{
		/// <summary>
		///   The left sub-trigger.
		/// </summary>
		private readonly InputTrigger _left;

		/// <summary>
		///   The right sub-trigger.
		/// </summary>
		private readonly InputTrigger _right;

		/// <summary>
		///   Determines the type of the trigger.
		/// </summary>
		private readonly BinaryInputTriggerType _triggerType;

		/// <summary>
		///   Indicates whether a ChordOnce trigger should trigger again.
		/// </summary>
		private bool _shouldTrigger = true;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="triggerType">Determines the type of the trigger.</param>
		/// <param name="left">The left sub-trigger.</param>
		/// <param name="right">The right sub-trigger.</param>
		internal BinaryInputTrigger(BinaryInputTriggerType triggerType, InputTrigger left, InputTrigger right)
		{
			Assert.ArgumentInRange(triggerType);
			Assert.ArgumentNotNull(left);
			Assert.ArgumentNotNull(right);

			_triggerType = triggerType;
			_left = left;
			_right = right;
		}

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		///   true if the current object is equal to other; otherwise, false.
		/// </returns>
		public override bool Equals(InputTrigger other)
		{
			var trigger = other as BinaryInputTrigger;
			if (trigger == null)
				return false;

			return _left.Equals(trigger._left) && _right.Equals(trigger._right) && _triggerType == trigger._triggerType;
		}

		/// <summary>
		///   Evaluates the trigger, returning true to indicate that the trigger has fired.
		/// </summary>
		/// <param name="device">The logical input device that should be used to evaluate the trigger.</param>
		internal override bool Evaluate(LogicalInputDevice device)
		{
			Assert.ArgumentNotNull(device);

			switch (_triggerType)
			{
				case BinaryInputTriggerType.ChordOnce:
					var isTriggered = _left.Evaluate(device) && _right.Evaluate(device);
					if (isTriggered && _shouldTrigger)
					{
						_shouldTrigger = false;
						return true;
					}

					if (!isTriggered)
						_shouldTrigger = true;

					return false;

				case BinaryInputTriggerType.Chord:
					return _left.Evaluate(device) && _right.Evaluate(device);

				case BinaryInputTriggerType.Alias:
					return _left.Evaluate(device) || _right.Evaluate(device);

				default:
					return false;
			}
		}

		/// <summary>
		///   Sets the logical input device the logical input is currently registered on.
		/// </summary>
		/// <param name="device">
		///   The logical input device the logical input is currently registered on. Null should be passed to
		///   indicate that the logical input is currently not registered on any device.
		/// </param>
		internal override void IsRegisteredOn(LogicalInputDevice device)
		{
			_left.IsRegisteredOn(device);
			_right.IsRegisteredOn(device);
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("({0} {2} {1})", _left, _right, _triggerType.ToExpressionString());
		}
	}
}