namespace Pegasus.Rendering.UserInterface
{
	using System;
	using Platform;
	using Framework.UserInterface.Input;
	using Platform.Memory;

	/// <summary>
	///     Manages the input functions for the console.
	/// </summary>
	internal class ConsoleInput : DisposableObject
	{
		/// <summary>
		///     The logical input device that provides the user input.
		/// </summary>
		private readonly LogicalInputDevice _device;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="device">The logical input device that provides the user input.</param>
		public ConsoleInput(LogicalInputDevice device)
		{
			_device = device;

			// We don't care which of the two control buttons has been pressed
			var controlPressed = Key.LeftControl.IsPressed() | Key.RightControl.IsPressed();
			var controlReleased = Key.LeftControl.IsReleased() | Key.RightControl.IsReleased();

			Toggle = new LogicalInput(new ScanCodeKeyTrigger(KeyTriggerType.WentDown, PlatformInfo.ConsoleKey));
			Submit = new LogicalInput(Key.Return.WentDown() | Key.NumpadEnter.WentDown());
			Clear = new LogicalInput(controlPressed + Key.L.IsPressed());
			ClearPrompt = new LogicalInput(Key.Escape.WentDown());
			ShowOlderHistory = new LogicalInput(Key.Up.IsRepeated());
			ShowNewerHistory = new LogicalInput(Key.Down.IsRepeated());
			ScrollUp = new LogicalInput(Key.PageUp.IsRepeated() & controlReleased);
			ScrollDown = new LogicalInput(Key.PageDown.IsRepeated() & controlReleased);
			ScrollToTop = new LogicalInput(controlPressed + Key.PageUp.IsPressed());
			ScrollToBottom = new LogicalInput(controlPressed + Key.PageDown.IsPressed());
			AutoCompleteNext = new LogicalInput(Key.Tab.WentDown() & Key.LeftShift.IsReleased());
			AutoCompletePrevious = new LogicalInput(Key.Tab.IsPressed() + Key.LeftShift.IsPressed());

			device.Add(Toggle);
			device.Add(Submit);
			device.Add(Clear);
			device.Add(ClearPrompt);
			device.Add(ShowOlderHistory);
			device.Add(ShowNewerHistory);
			device.Add(ScrollUp);
			device.Add(ScrollDown);
			device.Add(ScrollToTop);
			device.Add(ScrollToBottom);
			device.Add(AutoCompleteNext);
			device.Add(AutoCompletePrevious);
		}

		/// <summary>
		///     Gets the logical input for the console's toggle action.
		/// </summary>
		public LogicalInput Toggle { get; private set; }

		/// <summary>
		///     Gets the logical input for the console's clear action.
		/// </summary>
		public LogicalInput Clear { get; private set; }

		/// <summary>
		///     Gets the logical input for the console's clear prompt action.
		/// </summary>
		public LogicalInput ClearPrompt { get; private set; }

		/// <summary>
		///     Gets the logical input for the console's scroll down action.
		/// </summary>
		public LogicalInput ScrollDown { get; private set; }

		/// <summary>
		///     Gets the logical input for the console's scroll to bottom action.
		/// </summary>
		public LogicalInput ScrollToBottom { get; private set; }

		/// <summary>
		///     Gets the logical input for the console's scroll to top action.
		/// </summary>
		public LogicalInput ScrollToTop { get; private set; }

		/// <summary>
		///     Gets the logical input for the console's scroll up action.
		/// </summary>
		public LogicalInput ScrollUp { get; private set; }

		/// <summary>
		///     Gets the logical input for the console's show new history action.
		/// </summary>
		public LogicalInput ShowNewerHistory { get; private set; }

		/// <summary>
		///     Gets the logical input for the console's show older history action.
		/// </summary>
		public LogicalInput ShowOlderHistory { get; private set; }

		/// <summary>
		///     Gets the logical input for the console's submit action.
		/// </summary>
		public LogicalInput Submit { get; private set; }

		/// <summary>
		///     Gets the logical input for the console's auto-completion action in forward direction.
		/// </summary>
		public LogicalInput AutoCompleteNext { get; private set; }

		/// <summary>
		///     Gets the logical input for the console's auto-completion action in backwards direction.
		/// </summary>
		public LogicalInput AutoCompletePrevious { get; private set; }

		/// <summary>
		///     Raised when a text character was entered.
		/// </summary>
		public event Action<char> CharEntered
		{
			add { _device.Keyboard.CharacterEntered += value; }
			remove { _device.Keyboard.CharacterEntered -= value; }
		}

		/// <summary>
		///     Raised when a key was pressed.
		/// </summary>
		public event Action<KeyEventArgs> KeyPressed
		{
			add { _device.Keyboard.KeyPressed += value; }
			remove { _device.Keyboard.KeyPressed -= value; }
		}

		/// <summary>
		///     Raised when the mouse wheel has been moved.
		/// </summary>
		public event Action<int> MouseWheel
		{
			add { _device.Mouse.Wheel += value; }
			remove { _device.Mouse.Wheel -= value; }
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
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
			_device.Remove(AutoCompleteNext);
			_device.Remove(AutoCompletePrevious);
		}
	}
}