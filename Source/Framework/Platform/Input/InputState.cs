using System;

namespace Pegasus.Framework.Platform.Input
{
	using Logging;

	/// <summary>
	///   Represents the state of an input key.
	/// </summary>
	internal struct InputState : IEquatable<InputState>
	{
		/// <summary>
		///   Gets a value indicating whether the key is currently being pressed down.
		/// </summary>
		public bool IsPressed { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the key was pressed during the current frame. WentDown is
		///   only true during the single frame when IsPressed changed from false to true.
		/// </summary>
		public bool WentDown { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the key was released during the current frame. WentUp is
		///   only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		public bool WentUp { get; private set; }

		/// <summary>
		///   Gets a value indicating whether a system key repeat event occurred. IsRepeated is also true
		///   when the key is pressed, i.e., when WentDown is true.
		/// </summary>
		public bool IsRepeated { get; private set; }

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		///   true if the current object is equal to other; otherwise, false.
		/// </returns>
		public bool Equals(InputState other)
		{
			return IsPressed == other.IsPressed && IsRepeated == other.IsRepeated && WentDown == other.WentDown &&
				   WentUp == other.WentUp;
		}

		/// <summary>
		///   Updates the input state when the key has been pressed.
		/// </summary>
		internal void KeyPressed()
		{
			WentDown = !IsPressed;
			IsPressed = true;
			WentUp = false;
			IsRepeated = true;
		}

		/// <summary>
		///   Updates the input state when the key has been released.
		/// </summary>
		internal void KeyReleased()
		{
			WentUp = IsPressed;
			IsPressed = false;
			IsRepeated = false;
		}

		/// <summary>
		///   Ensures that WentDown, WentUp, and IsRepeated only remain true for one single frame, even if the actual
		///   key state has not changed.
		/// </summary>
		internal void Update()
		{
			WentDown = false;
			WentUp = false;
			IsRepeated = false;
		}
	}
}