namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Scripting;

	/// <summary>
	///     Represents a trigger with the actual trigger evaluation being performed by a trigger held by a cvar. The cvar's value
	///     can be changed at any time, changing the behavior of the configurable trigger accordingly.
	/// </summary>
	internal class ConfigurableTrigger : InputTrigger
	{
		/// <summary>
		///     The cvar that holds the actual configurable input.
		/// </summary>
		private readonly Cvar<ConfigurableInput> _configurableInput;

		/// <summary>
		///     Determines the type of a key input trigger.
		/// </summary>
		private readonly KeyTriggerType _keyTriggerType;

		/// <summary>
		///     Determines the type of a mouse input trigger.
		/// </summary>
		private readonly MouseTriggerType _mouseTriggerType;

		/// <summary>
		///     The logical input device that is currently used to evaluate the trigger.
		/// </summary>
		private LogicalInputDevice _device;

		/// <summary>
		///     The input trigger generated from the configurable input.
		/// </summary>
		private InputTrigger _inputTrigger;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="configurableInput">The configurable input that triggers the logical input.</param>
		/// <param name="keyTriggerType">Determines the type of a key input trigger.</param>
		/// <param name="mouseTriggerType">Determines the type of a mouse input trigger.</param>
		internal ConfigurableTrigger(Cvar<ConfigurableInput> configurableInput, KeyTriggerType keyTriggerType, MouseTriggerType mouseTriggerType)
		{
			Assert.ArgumentNotNull(configurableInput);
			Assert.ArgumentInRange(keyTriggerType);
			Assert.ArgumentInRange(mouseTriggerType);

			_configurableInput = configurableInput;
			_keyTriggerType = keyTriggerType;
			_mouseTriggerType = mouseTriggerType;

			CreateInputTrigger();
		}

		/// <summary>
		///     Creates the actual input trigger from the configurable input.
		/// </summary>
		private void CreateInputTrigger()
		{
			var input = _configurableInput.Value;
			Assert.That(input.Key != null || input.MouseButton != null, "Invalid configurable input: Neither key nor mouse button required.");
			Assert.That(input.Key == null || input.MouseButton == null, "Invalid configurable input: Either key or mouse button must be null.");

			if (input.Key != null)
				_inputTrigger = new KeyTrigger(_keyTriggerType, input.Key.Value);

			if (input.MouseButton != null)
				_inputTrigger = new MouseTrigger(_mouseTriggerType, input.MouseButton.Value);

			if ((input.Modifiers & KeyModifiers.Alt) == KeyModifiers.Alt)
				_inputTrigger &= Key.LeftAlt.IsPressed() | Key.RightAlt.IsPressed();

			if ((input.Modifiers & KeyModifiers.Shift) == KeyModifiers.Shift)
				_inputTrigger &= Key.LeftShift.IsPressed() | Key.RightShift.IsPressed();

			if ((input.Modifiers & KeyModifiers.Control) == KeyModifiers.Control)
				_inputTrigger &= Key.LeftControl.IsPressed() | Key.RightControl.IsPressed();
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

			if (device != null && _device == null)
				_configurableInput.Changed += OnConfigurableInputChanged;

			if (device == null && _device != null)
				_configurableInput.Changed -= OnConfigurableInputChanged;

			_device = device;
		}

		/// <summary>
		///     Recreates the input trigger created from the configurable input.
		/// </summary>
		/// <param name="input">The updated configurable input.</param>
		private void OnConfigurableInputChanged(ConfigurableInput input)
		{
			CreateInputTrigger();
		}

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		///     true if the current object is equal to other; otherwise, false.
		/// </returns>
		public override bool Equals(InputTrigger other)
		{
			var trigger = other as ConfigurableTrigger;
			if (trigger == null)
				return false;

			return _configurableInput == trigger._configurableInput;
		}

		/// <summary>
		///     Evaluates the trigger, returning true to indicate that the trigger has fired.
		/// </summary>
		/// <param name="device">The logical input device that should be used to evaluate the trigger.</param>
		internal override bool Evaluate(LogicalInputDevice device)
		{
			Assert.NotNull(_inputTrigger, "Input trigger not created.");
			return _inputTrigger.Evaluate(device);
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("Cvar({0}: {1})", _configurableInput.Name, _inputTrigger);
		}
	}
}