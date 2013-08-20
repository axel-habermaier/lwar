using System;

namespace Pegasus.Platform.Input
{
	using Memory;

	/// <summary>
	///   Represents the state of the keyboard.
	/// </summary>
	public class Keyboard : DisposableObject
	{
		/// <summary>
		///   The key states.
		/// </summary>
		private readonly InputState[] _states = new InputState[Enum.GetValues(typeof(Key)).Length];

		/// <summary>
		///   The window that generates the key events.
		/// </summary>
		private readonly Window _window;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that generates the key events.</param>
		public Keyboard(Window window)
		{
			Assert.ArgumentNotNull(window);

			_window = window;
			_window.KeyPressed += OnKeyPressed;
			_window.KeyReleased += OnKeyReleased;
			_window.CharacterEntered += OnCharacterEntered;
			_window.DeadCharacterEntered += OnDeadCharacterEntered;
		}

		/// <summary>
		///   Raised when a text character was entered.
		/// </summary>
		public event Action<char> CharacterEntered;

		/// <summary>
		///   Raised when a key was pressed.
		/// </summary>
		public event Action<KeyEventArgs> KeyPressed
		{
			add { _window.KeyPressed += value; }
			remove { _window.KeyPressed -= value; }
		}

		/// <summary>
		///   Raised when a key was released.
		/// </summary>
		public event Action<KeyEventArgs> KeyReleased
		{
			add { _window.KeyReleased += value; }
			remove { _window.KeyReleased -= value; }
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_window.KeyPressed -= OnKeyPressed;
			_window.KeyReleased -= OnKeyReleased;
			_window.CharacterEntered -= OnCharacterEntered;
			_window.DeadCharacterEntered -= OnDeadCharacterEntered;
		}

		/// <summary>
		///   Invoked when a character has been entered as the result of a dead key press.
		/// </summary>
		/// <param name="character">Identifies the character that has been entered.</param>
		/// <param name="cancel">
		///   If set to true, the dead character is removed such that the subsequently entered character is not
		///   influenced by the dead character.
		/// </param>
		private static void OnDeadCharacterEntered(CharacterEnteredEventArgs character, out bool cancel)
		{
			// Cancel the dead key if it is the result of the console key being pressed (happens, for instance, on German keyboard layouts)
			cancel = character.ScanCode == PlatformInfo.ConsoleKey;
		}

		/// <summary>
		///   Invoked when a character has been entered.
		/// </summary>
		/// <param name="character">Identifies the character that has been entered.</param>
		private void OnCharacterEntered(CharacterEnteredEventArgs character)
		{
			// Only raise the character entered event if the character is a printable ASCII character
			if (character.Character < 32 || character.Character > 255 || Char.IsControl(character.Character))
				return;

			// Do not raise the character entered event if the character is the result of the console key being pressed
			if (character.ScanCode == PlatformInfo.ConsoleKey)
				return;

			if (CharacterEntered != null)
				CharacterEntered(character.Character);
		}

		/// <summary>
		///   Invoked when a key has been released.
		/// </summary>
		/// <param name="key">Identifies the key that has been released.</param>
		private void OnKeyReleased(KeyEventArgs key)
		{
			_states[(int)key.Key].KeyReleased();
		}

		/// <summary>
		///   Invoked when a key has been pressed.
		/// </summary>
		/// <param name="key">Identifies the key that has been pressed.</param>
		private void OnKeyPressed(KeyEventArgs key)
		{
			_states[(int)key.Key].KeyPressed();
		}

		/// <summary>
		///   Updates the keyboard state.
		/// </summary>
		internal void Update()
		{
			for (var i = 0; i < _states.Length; ++i)
				_states[i].Update();
		}

		/// <summary>
		///   Gets a value indicating whether the key is currently being pressed down.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool IsPressed(Key key)
		{
			Assert.ArgumentInRange(key);
			return _states[(int)key].IsPressed;
		}

		/// <summary>
		///   Gets a value indicating whether the key was pressed during the current frame. WentDown is
		///   only true during the single frame when IsPressed changed from false to true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentDown(Key key)
		{
			Assert.ArgumentInRange(key);
			return _states[(int)key].WentDown;
		}

		/// <summary>
		///   Gets a value indicating whether the key was released during the current frame. WentUp is
		///   only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentUp(Key key)
		{
			Assert.ArgumentInRange(key);
			return _states[(int)key].WentUp;
		}

		/// <summary>
		///   Gets a value indicating whether a system key repeat event occurred. IsRepeated is also true
		///   when the key is pressed, i.e., when WentDown is true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool IsRepeated(Key key)
		{
			Assert.ArgumentInRange(key);
			return _states[(int)key].IsRepeated;
		}
	}
}