using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using Platform;
	using Platform.Input;
	using Platform.Memory;

	/// <summary>
	///   Manages the input functions for the console.
	/// </summary>
	internal class ConsoleInput : DisposableObject
	{
		/// <summary>
		///   The logical input device that provides the user input.
		/// </summary>
		private readonly LogicalInputDevice _device;

		/// <summary>
		///   The input mode the input device should be reset to once the console is deativated.
		/// </summary>
		private InputModes _previousInputModes;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="device">The logical input device that provides the user input.</param>
		public ConsoleInput(LogicalInputDevice device)
		{
			_device = device;

			// We don't care which of the two control buttons has been pressed
			var controlPressed = Key.LeftControl.IsPressed() | Key.RightControl.IsPressed();
			var controlReleased = Key.LeftControl.IsReleased() | Key.RightControl.IsReleased();

			Toggle = new LogicalInput(new ScanCodeKeyTrigger(KeyTriggerType.WentDown, PlatformInfo.ConsoleKey), InputModes.All);
			Submit = new LogicalInput(Key.Return.WentDown() | Key.NumpadEnter.WentDown(), InputModes.Console);
			Clear = new LogicalInput(controlPressed + Key.L.IsPressed(), InputModes.Console);
			ClearPrompt = new LogicalInput(Key.Escape.WentDown(), InputModes.Console);
			ShowOlderHistory = new LogicalInput(Key.Up.WentDown(), InputModes.Console);
			ShowNewerHistory = new LogicalInput(Key.Down.WentDown(), InputModes.Console);
			ScrollUp = new LogicalInput(Key.PageUp.IsRepeated() & controlReleased, InputModes.Console);
			ScrollDown = new LogicalInput(Key.PageDown.IsRepeated() & controlReleased, InputModes.Console);
			ScrollToTop = new LogicalInput(controlPressed + Key.PageUp.IsPressed(), InputModes.Console);
			ScrollToBottom = new LogicalInput(controlPressed + Key.PageDown.IsPressed(), InputModes.Console);
			AutoComplete = new LogicalInput(Key.Tab.WentDown(), InputModes.Console);

			device.Register(Toggle);
			device.Register(Submit);
			device.Register(Clear);
			device.Register(ClearPrompt);
			device.Register(ShowOlderHistory);
			device.Register(ShowNewerHistory);
			device.Register(ScrollUp);
			device.Register(ScrollDown);
			device.Register(ScrollToTop);
			device.Register(ScrollToBottom);
			device.Register(AutoComplete);
		}

		/// <summary>
		///   Gets the logical input for the console's toggle action.
		/// </summary>
		public LogicalInput Toggle { get; private set; }

		/// <summary>
		///   Gets the logical input for the console's clear action.
		/// </summary>
		public LogicalInput Clear { get; private set; }

		/// <summary>
		///   Gets the logical input for the console's clear prompt action.
		/// </summary>
		public LogicalInput ClearPrompt { get; private set; }

		/// <summary>
		///   Gets the logical input for the console's scroll down action.
		/// </summary>
		public LogicalInput ScrollDown { get; private set; }

		/// <summary>
		///   Gets the logical input for the console's scroll to bottom action.
		/// </summary>
		public LogicalInput ScrollToBottom { get; private set; }

		/// <summary>
		///   Gets the logical input for the console's scroll to top action.
		/// </summary>
		public LogicalInput ScrollToTop { get; private set; }

		/// <summary>
		///   Gets the logical input for the console's scroll up action.
		/// </summary>
		public LogicalInput ScrollUp { get; private set; }

		/// <summary>
		///   Gets the logical input for the console's show new history action.
		/// </summary>
		public LogicalInput ShowNewerHistory { get; private set; }

		/// <summary>
		///   Gets the logical input for the console's show older history action.
		/// </summary>
		public LogicalInput ShowOlderHistory { get; private set; }

		/// <summary>
		///   Gets the logical input for the console's submit action.
		/// </summary>
		public LogicalInput Submit { get; private set; }

		/// <summary>
		/// Gets the logical input for the console's auto-completion action.
		/// </summary>
		public LogicalInput AutoComplete { get; private set; }

		/// <summary>
		///   Invoked when the activation state of the console has been changed.
		/// </summary>
		public void OnActivationChanged(bool activated)
		{
			if (activated)
			{
				_previousInputModes = _device.Modes;
				_device.Modes = InputModes.Console;
			}
			else
				_device.Modes = _previousInputModes;
		}

		/// <summary>
		///   Raised when a text character was entered.
		/// </summary>
		public event Action<char> CharEntered
		{
			add { _device.Keyboard.CharEntered += value; }
			remove { _device.Keyboard.CharEntered -= value; }
		}

		/// <summary>
		///   Raised when a key was pressed.
		/// </summary>
		public event Action<KeyEventArgs> KeyPressed
		{
			add { _device.Keyboard.KeyPressed += value; }
			remove { _device.Keyboard.KeyPressed -= value; }
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_device.Remove(Toggle);
			_device.Remove(Submit);
			_device.Remove(Clear);
			_device.Remove(ClearPrompt);
			_device.Remove(ShowOlderHistory);
			_device.Remove(ShowNewerHistory);
			_device.Remove(ScrollUp);
			_device.Remove(ScrollDown);
			_device.Remove(ScrollToTop);
			_device.Remove(ScrollToBottom);
			_device.Remove(AutoComplete);
		}
	}
}