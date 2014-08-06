namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Scripting;

	/// <summary>
	///     Provides extension methods for the input system.
	/// </summary>
	public static class InputExtensions
	{
		/// <summary>
		///     Creates a trigger that triggers when the key is released.
		/// </summary>
		public static InputTrigger IsReleased(this Key key)
		{
			Assert.ArgumentInRange(key);
			return new KeyTrigger(KeyTriggerType.Released, key);
		}

		/// <summary>
		///     Creates a trigger that triggers when the key is pressed.
		/// </summary>
		public static InputTrigger IsPressed(this Key key)
		{
			Assert.ArgumentInRange(key);
			return new KeyTrigger(KeyTriggerType.Pressed, key);
		}

		/// <summary>
		///     Creates a trigger that triggers when the key is repeated.
		/// </summary>
		public static InputTrigger IsRepeated(this Key key)
		{
			Assert.ArgumentInRange(key);
			return new KeyTrigger(KeyTriggerType.Repeated, key);
		}

		/// <summary>
		///     Creates a trigger that triggers when the key went down.
		/// </summary>
		public static InputTrigger WentDown(this Key key)
		{
			Assert.ArgumentInRange(key);
			return new KeyTrigger(KeyTriggerType.WentDown, key);
		}

		/// <summary>
		///     Creates a trigger that triggers when the key went up.
		/// </summary>
		public static InputTrigger WentUp(this Key key)
		{
			Assert.ArgumentInRange(key);
			return new KeyTrigger(KeyTriggerType.WentUp, key);
		}

		/// <summary>
		///     Creates a trigger that triggers when the mouse button is released.
		/// </summary>
		public static InputTrigger IsReleased(this MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return new MouseTrigger(MouseTriggerType.Released, button);
		}

		/// <summary>
		///     Creates a trigger that triggers when the mouse button is pressed.
		/// </summary>
		public static InputTrigger IsPressed(this MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return new MouseTrigger(MouseTriggerType.Pressed, button);
		}

		/// <summary>
		///     Creates a trigger that triggers when the mouse button went down.
		/// </summary>
		public static InputTrigger WentDown(this MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return new MouseTrigger(MouseTriggerType.WentDown, button);
		}

		/// <summary>
		///     Creates trigger that triggers when the mouse button went up.
		/// </summary>
		public static InputTrigger WentUp(this MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return new MouseTrigger(MouseTriggerType.WentUp, button);
		}

		/// <summary>
		///     Creates trigger that triggers when the mouse has been double-clicked.
		/// </summary>
		public static InputTrigger DoubleClicked(this MouseButton button)
		{
			Assert.ArgumentInRange(button);
			return new MouseTrigger(MouseTriggerType.DoubleClicked, button);
		}

		/// <summary>
		///     Creates a configurable trigger.
		/// </summary>
		public static InputTrigger ToTrigger(this Cvar<InputTrigger> cvar)
		{
			Assert.ArgumentNotNull(cvar);
			return new ConfigurableTrigger(cvar);
		}

		/// <summary>
		///     Converts the given binary input trigger type into its corresponding expression string.
		/// </summary>
		/// <param name="triggerType">The binary input trigger type that should be converted.</param>
		internal static string ToExpressionString(this BinaryInputTriggerType triggerType)
		{
			Assert.ArgumentInRange(triggerType);
			switch (triggerType)
			{
				case BinaryInputTriggerType.ChordOnce:
					return "+";
				case BinaryInputTriggerType.Chord:
					return "&";
				case BinaryInputTriggerType.Alias:
					return "|";
				default:
					return "<unknown>";
			}
		}
	}
}