namespace Pegasus.UserInterface.Input
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents an input binding that is triggered by turning the mouse wheel.
	/// </summary>
	public sealed class MouseWheelBinding : InputBinding
	{
		/// <summary>
		///     The direction the mouse wheel must be turned in to trigger the binding. If null, both directions trigger the binding.
		/// </summary>
		private MouseWheelDirection? _direction;

		/// <summary>
		///     The key modifiers that must be pressed for the binding to trigger.
		/// </summary>
		private KeyModifiers _modifiers;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public MouseWheelBinding()
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="method">The name of the method that should be invoked when the binding is triggered.</param>
		/// <param name="modifiers">The key modifiers that must be pressed for the binding to trigger.</param>
		public MouseWheelBinding(string method, KeyModifiers modifiers = KeyModifiers.None)
		{
			Assert.ArgumentNotNullOrWhitespace(method);

			Modifiers = modifiers;
			Method = method;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="direction">
		///     The direction the mouse wheel must be turned in to trigger the binding. If null, both directions
		///     trigger the binding.
		/// </param>
		/// <param name="method">The name of the method that should be invoked when the binding is triggered.</param>
		/// <param name="modifiers">The key modifiers that must be pressed for the binding to trigger.</param>
		public MouseWheelBinding(MouseWheelDirection direction, string method, KeyModifiers modifiers = KeyModifiers.None)
		{
			Assert.ArgumentInRange(direction);
			Assert.ArgumentNotNullOrWhitespace(method);

			Direction = direction;
			Modifiers = modifiers;
			Method = method;
		}

		/// <summary>
		///     Gets or sets the direction the mouse wheel must be turned in to trigger the binding. If null, both directions trigger
		///     the binding.
		/// </summary>
		public MouseWheelDirection? Direction
		{
			get { return _direction; }
			set
			{
				Assert.NotSealed(this);
				if (_direction.HasValue)
					Assert.ArgumentInRange(_direction.Value);

				_direction = value;
			}
		}

		/// <summary>
		///     Gets or sets the key modifiers that must be pressed for the binding to trigger.
		/// </summary>
		public KeyModifiers Modifiers
		{
			get { return _modifiers; }
			set
			{
				Assert.NotSealed(this);
				_modifiers = value;
			}
		}

		/// <summary>
		///     Checks whether the given event triggers the input binding.
		/// </summary>
		/// <param name="args">The arguments of the event that should be checked.</param>
		protected override bool IsTriggered(RoutedEventArgs args)
		{
			if (Preview && args.RoutedEvent != UIElement.PreviewMouseWheelEvent)
				return false;

			if (!Preview && args.RoutedEvent != UIElement.MouseWheelEvent)
				return false;

			var wheelEventArgs = args as MouseWheelEventArgs;
			if (wheelEventArgs == null)
				return false;

			switch (Direction)
			{
				case MouseWheelDirection.Up:
					return wheelEventArgs.Direction == MouseWheelDirection.Up && wheelEventArgs.Modifiers == Modifiers;
				case MouseWheelDirection.Down:
					return wheelEventArgs.Direction == MouseWheelDirection.Down && wheelEventArgs.Modifiers == Modifiers;
				case null:
					return true;
				default:
					Assert.NotReached("Unknown mouse wheel direction.");
					return false;
			}
		}
	}
}