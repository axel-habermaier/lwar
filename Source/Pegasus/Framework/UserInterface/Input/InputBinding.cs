namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using System.Linq;
	using Platform.Logging;

	/// <summary>
	///     Represents an input binding that invokes a method on the associated UI element's data context whenever it is triggered.
	/// </summary>
	public abstract class InputBinding : ISealable
	{
		/// <summary>
		///     The name of the data context method that is invoked when the binding is triggered.
		/// </summary>
		private string _method;

		/// <summary>
		///     The target method on the associated UI element that is invoked whenever the binding is triggered.
		/// </summary>
		private Action _targetMethod;

		/// <summary>
		///     Gets or sets the name of the data context method that is invoked when the binding is triggered.
		/// </summary>
		public string Method
		{
			get { return _method; }
			set
			{
				Assert.ArgumentNotNullOrWhitespace(value);
				Assert.NotSealed(this);

				_method = value;
			}
		}

		/// <summary>
		///     Gets a value indicating whether the object is sealed and can no longer be modified.
		/// </summary>
		public bool IsSealed { get; private set; }

		/// <summary>
		///     Seals the object such that it can no longer be modified.
		/// </summary>
		public void Seal()
		{
			IsSealed = true;
		}

		/// <summary>
		///     Updates the bound target method of the input binding.
		/// </summary>
		/// <param name="dataContext">The data context the target method should be invoked on.</param>
		internal void BindToDataContext(object dataContext)
		{
			Assert.That(IsSealed, "The input binding has not yet been sealed.");
			Assert.That(_method != null, "No method name has been set on the input binding.");

			_targetMethod = null;
			if (dataContext == null)
				return;

			var dataContextType = dataContext.GetType();
			var method = dataContextType.GetMethods().SingleOrDefault(m => m.Name == _method && m.ReturnType == typeof(void)
																		   && m.GetParameters().Length == 0);

			Log.DebugIf(method == null, "Input binding failure: Unable to find method 'void {0}.{1}()'.", dataContextType.FullName, _method);
			if (method == null)
				return;

			_targetMethod = (Action)Delegate.CreateDelegate(typeof(Action), dataContext, method);
		}

		/// <summary>
		///     Handles the given event, invoking the target method if the input binding is triggered.
		/// </summary>
		/// <param name="args">The arguments of the event that should be handled.</param>
		internal void HandleEvent(RoutedEventArgs args)
		{
			Assert.ArgumentNotNull(args);

			if (_targetMethod == null || !IsTriggered(args))
				return;

			_targetMethod();
			args.Handled = true;
		}

		/// <summary>
		///     Checks whether the given event triggers the input binding.
		/// </summary>
		/// <param name="args">The arguments of the event that should be checked.</param>
		protected abstract bool IsTriggered(RoutedEventArgs args);

		/// <summary>
		///     Checks whether the given event triggers the given key input.
		/// </summary>
		/// <param name="args">The arguments of the event that should be checked.</param>
		/// <param name="key">The expected key the event should have been raised for.</param>
		/// <param name="modifiers">The expected state of the modifier keys.</param>
		/// <param name="trigger">The type of the key trigger.</param>
		protected static bool IsTriggered(RoutedEventArgs args, Key key, KeyModifiers modifiers, KeyTriggerType trigger)
		{
			var keyEventArgs = args as KeyEventArgs;
			if (keyEventArgs == null || keyEventArgs.Key != key || keyEventArgs.Modifiers != modifiers)
				return false;

			switch (trigger)
			{
				case KeyTriggerType.Released:
					return !keyEventArgs.IsPressed;
				case KeyTriggerType.Pressed:
					return keyEventArgs.IsPressed;
				case KeyTriggerType.Repeated:
					return keyEventArgs.IsRepeated;
				case KeyTriggerType.WentDown:
					return keyEventArgs.WentDown;
				case KeyTriggerType.WentUp:
					return keyEventArgs.WentUp;
				default:
					return false;
			}
		}
	}
}