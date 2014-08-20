﻿namespace Pegasus.Framework.UserInterface.Input
{
	using System;

	/// <summary>
	///     Represents a trigger that triggers if a key is in a certain state. It is similar to a KeyTrigger, except
	///     that it identifies the monitored key by its scan code.
	/// </summary>
	internal sealed class ScanCodeKeyTrigger : InputTrigger
	{
		/// <summary>
		///     The scan code of the key that the trigger monitors.
		/// </summary>
		private readonly int _scanCode;

		/// <summary>
		///     Determines the type of the trigger.
		/// </summary>
		private readonly KeyTriggerType _triggerType;

		/// <summary>
		///     The logical input device that is currently used to evaluate the trigger. We must store a reference
		///     to it in order to be able to unregister from the keyboard's key event.
		/// </summary>
		private LogicalInputDevice _device;

		/// <summary>
		///     The state of the monitored key.
		/// </summary>
		private InputState _state = new InputState();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="triggerType">Determines the type of the trigger.</param>
		/// <param name="scanCode">The scan code of the key that the trigger monitors.</param>
		internal ScanCodeKeyTrigger(KeyTriggerType triggerType, int scanCode)
		{
			Assert.ArgumentInRange(triggerType);

			_triggerType = triggerType;
			_scanCode = scanCode;
		}

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		public override bool Equals(InputTrigger other)
		{
			var trigger = other as ScanCodeKeyTrigger;
			if (trigger == null)
				return false;

			return _scanCode == trigger._scanCode && _triggerType == trigger._triggerType && _device == trigger._device &&
				   _state.Equals(trigger._state);
		}

		/// <summary>
		///     Evaluates the trigger, returning true to indicate that the trigger has fired.
		/// </summary>
		/// <param name="device">The logical input device that should be used to evaluate the trigger.</param>
		internal override bool Evaluate(LogicalInputDevice device)
		{
			// First check whether the key is triggered, then update the key state. Otherwise, we would miss 
			// all WentDown/WentUp/Released events.
			bool isTriggered;
			switch (_triggerType)
			{
				case KeyTriggerType.WentDown:
					isTriggered = _state.WentDown;
					break;
				case KeyTriggerType.Pressed:
					isTriggered = _state.IsPressed;
					break;
				case KeyTriggerType.WentUp:
					isTriggered = _state.WentUp;
					break;
				case KeyTriggerType.Repeated:
					isTriggered = _state.IsRepeated;
					break;
				default:
					throw new InvalidOperationException("Unsupported trigger type.");
			}

			_state.Update();
			return isTriggered;
		}

		/// <summary>
		///     Sets the logical input device the logical input is currently registered on.
		/// </summary>
		/// <param name="device">
		///     The logical input device the logical input is currently registered on. Null should be passed to
		///     indicate that the logical input is currently not registered on any device.
		/// </param>
		internal override void SetLogicalDevice(LogicalInputDevice device)
		{
			if (device == _device)
				return;

			if (_device != null)
			{
				_device.KeyPressed -= OnKeyPressed;
				_device.KeyReleased -= OnKeyReleased;
			}

			if (device != null)
			{
				device.KeyPressed += OnKeyPressed;
				device.KeyReleased += OnKeyReleased;
			}

			_device = device;
		}

		/// <summary>
		///     Invoked whenever a key is pressed.
		/// </summary>
		/// <param name="key">The key that was pressed.</param>
		private void OnKeyPressed(KeyEventArgs key)
		{
			if (key.ScanCode == _scanCode)
				_state.Pressed();
		}

		/// <summary>
		///     Invoked whenever a key is released.
		/// </summary>
		/// <param name="key">The key that was released.</param>
		private void OnKeyReleased(KeyEventArgs key)
		{
			if (key.ScanCode == _scanCode)
				_state.Released();
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("ScanCode({1}, {0})", _triggerType, _scanCode);
		}
	}
}