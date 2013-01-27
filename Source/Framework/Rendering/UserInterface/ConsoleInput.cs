using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using Platform;
	using Platform.Input;

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
		///   Initializes a new instance.
		/// </summary>
		/// <param name="device">The logical input device that provides the user input.</param>
		public ConsoleInput(LogicalInputDevice device)
		{
			_device = device;

			// We don't care which of the two control buttons has been pressed
			var control = Key.LeftControl.IsPressed() | Key.RightControl.IsPressed();

			Toggle = new LogicalInput(new ScanCodeKeyTrigger(KeyTriggerType.WentDown, PlatformInfo.ConsoleKey));
			Submit = new LogicalInput(Key.Return.WentDown() | Key.NumpadEnter.WentDown());
			Clear = new LogicalInput(control + Key.L.IsPressed());
			ClearPrompt = new LogicalInput(Key.Escape.WentDown());
			ShowOlderHistory = new LogicalInput(Key.Up.WentDown());
			ShowNewerHistory = new LogicalInput(Key.Down.WentDown());
			ScrollUp = new LogicalInput(Key.PageUp.WentDown());
			ScrollDown = new LogicalInput(Key.PageDown.WentDown());
			ScrollToTop = new LogicalInput(control + Key.Home.IsPressed());
			ScrollToBottom = new LogicalInput(control + Key.End.IsPressed());

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
		}
	}
}