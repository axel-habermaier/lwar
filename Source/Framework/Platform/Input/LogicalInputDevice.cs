using System;

namespace Pegasus.Framework.Platform.Input
{
	using System.Collections.Generic;
	using Logging;
	using Memory;

	/// <summary>
	///   Manages logical inputs that are triggered by input triggers. A logical input device has support for 32 unique and
	///   prioritized input layers that determine which inputs can be triggered. Higher-numbered layers take priority over
	///   lower-numbered ones, effectively disabling all input to lower layers. Layers are activated and deactivated using the
	///   ActivateLayer and DeactivateLayer methods, where the number of calls to DeactivateLayer for a given layer n must
	///   match the number of calls to ActivateLayer for n before n is considered inactive again. This way, the order of
	///   activation and deactivation is not important. The actual layer activation and deactivation is deferred one frame.
	/// </summary>
	public class LogicalInputDevice : DisposableObject
	{
		/// <summary>
		///   The logical inputs that are currently registered on the device.
		/// </summary>
		private readonly List<LogicalInput> _inputs = new List<LogicalInput>(256);

		/// <summary>
		///   Stores the input layer activation states. The value at index n corresponds to InputLayer(n). A layer is active if its
		///   count is greater than zero. The highest non-zero index determines the currently active input layer.
		/// </summary>
		private readonly ActivationState[] _layerStates = new ActivationState[32];

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="keyboard">The keyboard that should be associated with this logical device.</param>
		/// <param name="mouse">The mouse that should be associated with this logical device.</param>
		public LogicalInputDevice(Keyboard keyboard, Mouse mouse)
		{
			Assert.ArgumentNotNull(keyboard);
			Assert.ArgumentNotNull(mouse);

			Keyboard = keyboard;
			Mouse = mouse;
		}

		/// <summary>
		///   Gets the current input layer used by the input device.
		/// </summary>
		public InputLayer InputLayer { get; private set; }

		/// <summary>
		///   Gets the keyboard that is associated with this logical device.
		/// </summary>
		public Keyboard Keyboard { get; private set; }

		/// <summary>
		///   Gets the mouse that is associated with this logical device.
		/// </summary>
		public Mouse Mouse { get; private set; }

		/// <summary>
		/// A value indicating whether the logical input device provides text input.
		/// </summary>
		private ActivationState _textInput = new ActivationState();

		/// <summary>
		///   Gets or sets a value indicating whether the logical input device provides text input. The actual update is deferred
		///   until the next frame. Text input is only disabled if the deactivation count matches the activation count.
		/// </summary>
		public bool TextInputEnabled
		{
			get { return _textInput.Count != 0; }
			set
			{
				if (value)
					_textInput.Activate();
				else
					_textInput.Deactivate();
			}
		}

		/// <summary>
		///   Registers a logical input on the logical input device. Subsequently, the logical input's IsTriggered
		///   property can be used to determine whether the logical input is currently triggered.
		/// </summary>
		/// <param name="input">The logical input that should be registered on the device.</param>
		public void Add(LogicalInput input)
		{
			Assert.ArgumentNotNull(input);
			Assert.ArgumentSatisfies(!input.IsRegistered, "The input is already registered on a device.");

			_inputs.Add(input);
			input.IsRegisteredOn(this);

			Log.DebugInfo(LogCategory.Platform, "A logical input with trigger '{0}' has been registered.", input.Trigger);
		}

		/// <summary>
		///   Removes the logical input from the logical input device.
		/// </summary>
		/// <param name="input">The logical input that should be removed.</param>
		public void Remove(LogicalInput input)
		{
			Assert.ArgumentNotNull(input);
			Assert.ArgumentSatisfies(input.IsRegistered, "The input trigger is not registered.");

			if (_inputs.Remove(input))
				input.IsRegisteredOn(null);
		}

		/// <summary>
		///   Activates the given input layer on the device, disabling all lower input layers starting with the next frame.
		/// </summary>
		/// <param name="layer">The layer that should be activated.</param>
		public void ActivateLayer(InputLayer layer)
		{
			Assert.ArgumentSatisfies(layer.IsPrimitive, "Invalid input layer.");
			_layerStates[layer.Priority - 1].Activate();
		}

		/// <summary>
		///   Deactivates the given input layer on the device, only enabling the next lower active input layer if the number of
		///   deactivation requests for the given layer matches the number of activation requests. The actual deactivation is
		///   deferred until the next frame.
		/// </summary>
		/// <param name="layer">The layer that should be activated.</param>
		public void DeactivateLayer(InputLayer layer)
		{
			Assert.ArgumentSatisfies(layer.IsPrimitive, "Invalid input layer.");
			_layerStates[layer.Priority - 1].Deactivate();
		}

		/// <summary>
		///   Updates the device state.
		/// </summary>
		internal void Update()
		{
			// Perform all deferred layer activations and deactivations
			for (var i = 0; i < _layerStates.Length; ++i)
				_layerStates[i].Update();

			// Determine the currently active input layout with the highest priority
			for (var i = _layerStates.Length; i > 0; --i)
			{
				if (_layerStates[i - 1].Count > 0)
				{
					if (InputLayer.Priority != i)
						Log.DebugInfo(LogCategory.Platform, "Input layer has been switched to {0}.", i);

					InputLayer = InputLayer.Create(i);
					break;
				}
			}

			Assert.That(InputLayer.Priority != 0, "No active input layer.");

			// Update the text input state
			_textInput.Update();

			// Update all inputs
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

		/// <summary>
		///   Represents an activation state with deferred updates. The state is considered active for as long as Count != 0.
		/// </summary>
		private struct ActivationState
		{
			/// <summary>
			///   The current activation count.
			/// </summary>
			public ushort Count;

			/// <summary>
			///   The pending activation and deactivation requests.
			/// </summary>
			public short Pending;

			/// <summary>
			/// Executes all deferred updates to the state.
			/// </summary>
			public void Update()
			{
				Count = (ushort)(Count + Pending);
				Pending = 0;
			}

			/// <summary>
			/// Handles a deferred activation request.
			/// </summary>
			public void Activate()
			{
				Assert.InRange(Pending, Int16.MinValue, Int16.MaxValue);
				Assert.That(Count + Pending + 1 < UInt16.MaxValue, "Too many activations.");

				++Pending;
			}
			/// <summary>
			/// Handles a deferred deactivation request.
			/// </summary>
			public void Deactivate()
			{
				Assert.InRange(Pending, Int16.MinValue, Int16.MaxValue);
				Assert.That(Count + Pending > 0, "Imbalanced call to deactivate.");

				--Pending;
			}}
	}
}