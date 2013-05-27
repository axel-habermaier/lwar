using System;

namespace Pegasus.Framework.Platform.Input
{
	using System.Collections.Generic;
	using Memory;

	/// <summary>
	///   Manages logical inputs that are triggered by input triggers.
	/// </summary>
	public class LogicalInputDevice : DisposableObject
	{
		/// <summary>
		///   The logical inputs that are currently registered on the device.
		/// </summary>
		private readonly List<LogicalInput> _inputs = new List<LogicalInput>(256);

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="keyboard">The keyboard that should be associated with this logical device.</param>
		/// <param name="mouse">The mouse that should be associated with this logical device.</param>
		public LogicalInputDevice(Keyboard keyboard, Mouse mouse)
		{
			Assert.ArgumentNotNull(keyboard, () => keyboard);
			Assert.ArgumentNotNull(mouse, () => mouse);

			Keyboard = keyboard;
			Mouse = mouse;
		}

		/// <summary>
		///   Gets or sets the current modes of the input device.
		/// </summary>
		public InputModes Modes { get; set; }

		/// <summary>
		///   Gets the keyboard that is associated with this logical device.
		/// </summary>
		public Keyboard Keyboard { get; private set; }

		/// <summary>
		///   Gets the mouse that is associated with this logical device.
		/// </summary>
		public Mouse Mouse { get; private set; }

		/// <summary>
		///   Registers a logical input on the logical input device. Subsequently, the logical input's IsTriggered
		///   property can be used to determine whether the logical input is currently triggered.
		/// </summary>
		/// <param name="input">The logical input that should be registered on the device.</param>
		public void Register(LogicalInput input)
		{
			Assert.ArgumentNotNull(input, () => input);
			Assert.ArgumentSatisfies(!input.IsRegistered, () => input, "The input is already registered on a device.");

			_inputs.Add(input);
			input.IsRegisteredOn(this);

			Log.DebugInfo("A logical input with trigger '{0}' has been registered.", input.Trigger);
		}

		/// <summary>
		///   Removes the logical input from the logical input device.
		/// </summary>
		/// <param name="input">The logical input that should be removed.</param>
		public void Remove(LogicalInput input)
		{
			Assert.ArgumentNotNull(input, () => input);
			Assert.ArgumentSatisfies(input.IsRegistered, () => input, "The input trigger is not registered.");

			if (_inputs.Remove(input))
				input.IsRegisteredOn(null);
		}

		/// <summary>
		///   Updates the device state.
		/// </summary>
		internal void Update()
		{
			Assert.That(Modes != InputModes.None, "No input mode has been set.");

			foreach (var input in _inputs)
				input.Update(this);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			// Nothing to do here
		}
	}
}