using System;

namespace Pegasus.Framework.Platform.Input
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
			_window = window;
			_window.KeyPressed += OnKeyPressed;
			_window.KeyReleased += OnKeyReleased;
		}

		/// <summary>
		///   Raised when a text character was entered.
		/// </summary>
		public event Action<char> CharEntered
		{
			add { _window.CharacterEntered += value; }
			remove { _window.CharacterEntered -= value; }
		}

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

		protected override void OnDisposing()
		{
			_window.KeyPressed -= OnKeyPressed;
			_window.KeyReleased -= OnKeyReleased;
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
			Assert.ArgumentInRange(key, () => key);
			return _states[(int)key].IsPressed;
		}

		/// <summary>
		///   Gets a value indicating whether the key was pressed during the current frame. WentDown is
		///   only true during the single frame when IsPressed changed from false to true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentDown(Key key)
		{
			Assert.ArgumentInRange(key, () => key);
			return _states[(int)key].WentDown;
		}

		/// <summary>
		///   Gets a value indicating whether the key was released during the current frame. WentUp is
		///   only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool WentUp(Key key)
		{
			Assert.ArgumentInRange(key, () => key);
			return _states[(int)key].WentUp;
		}

		/// <summary>
		///   Gets a value indicating whether a system key repeat event occurred. IsRepeated is also true
		///   when the key is pressed, i.e., when WentDown is true.
		/// </summary>
		/// <param name="key">The key that should be checked.</param>
		public bool IsRepeated(Key key)
		{
			Assert.ArgumentInRange(key, () => key);
			return _states[(int)key].IsRepeated;
		}
	}
}