namespace Pegasus.UserInterface
{
	using System;
	using System.Linq;
	using System.Reflection;
	using Platform.Logging;
	using Utilities;

	/// <summary>
	///     Binds a UI element/routed event pair to a method of the UI element's data context.
	/// </summary>
	/// <typeparam name="T">The type of the event arguments.</typeparam>
	internal class RoutedEventBinding<T>
		where T : RoutedEventArgs
	{
		/// <summary>
		///     The UI element that is bound to.
		/// </summary>
		private readonly UIElement _element;

		/// <summary>
		///     The name of the method that is invoked when the routed event is raised.
		/// </summary>
		private readonly string _methodName;

		/// <summary>
		///     The routed event that is bound to.
		/// </summary>
		private readonly RoutedEvent<T> _routedEvent;

		/// <summary>
		///     Indicates whether the binding is currently active.
		/// </summary>
		private bool _active;

		/// <summary>
		///     The data context the event handler is invoked on.
		/// </summary>
		private object _dataContext;

		/// <summary>
		///     The handler registered on the routed event that invokes the target method.
		/// </summary>
		private Action<object, T> _handler;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="element">The UI element that raises the routed event.</param>
		/// <param name="routedEvent">The routed event that should be bound to.</param>
		/// <param name="method">The name of the method that should be invoked when the routed event is raised.</param>
		public RoutedEventBinding(UIElement element, RoutedEvent<T> routedEvent, string method)
		{
			Assert.ArgumentNotNull(element);
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNullOrWhitespace(method);

			_element = element;
			_routedEvent = routedEvent;
			_methodName = method;
		}

		/// <summary>
		///     Gets or sets a value indicating whether the binding is currently active.
		/// </summary>
		internal bool Active
		{
			get { return _active; }
			set
			{
				if (_active == value)
					return;

				_active = value;

				if (_active)
					Activate();
				else
					Deactivate();
			}
		}

		/// <summary>
		///     Invoked when the binding has been activated.
		/// </summary>
		private void Activate()
		{
			InitializeBinding(_element.DataContext);
			_element.AddHandler(_routedEvent, OnEventRaised);
			_element.AddChangedHandler(UIElement.DataContextProperty, OnDataContextChanged);
		}

		/// <summary>
		///     Invoked when the binding has been deactivated.
		/// </summary>
		private void Deactivate()
		{
			_element.RemoveHandler(_routedEvent, OnEventRaised);
			_element.RemoveChangedHandler(UIElement.DataContextProperty, OnDataContextChanged);
		}

		/// <summary>
		///     Reinitializes the event binding for the new data context.
		/// </summary>
		private void OnDataContextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<object> args)
		{
			if (Active)
				InitializeBinding(args.NewValue);
		}

		/// <summary>
		///     Initializes the event binding for the given data context.
		/// </summary>
		/// <param name="dataContext">The data context that defines the event handler.</param>
		private void InitializeBinding(object dataContext)
		{
			Log.DebugIf(dataContext == null,
						"Event binding failure: Data context is null while trying to bind method '{0}'.", _methodName);

			_dataContext = dataContext;
			_handler = null;

			if (dataContext == null)
				return;

			var dataContextType = dataContext.GetType();

			// First, try to bind to a method of the appropriate name without any parameters.
			var method = dataContextType.GetTypeInfo().GetDeclaredMethods(_methodName).FirstOrDefault(m => m.GetParameters().Length == 0);
			if (method != null)
			{
				var methodDelegate = (Action)method.CreateDelegate(typeof(Action), dataContext);
				_handler = (s, a) => methodDelegate();
			}
			else
			{
				// Otherwise, try to bind to a method of the appropriate name with a RoutedEventHandler<T> signature.
				method = dataContextType.GetTypeInfo().GetDeclaredMethods(_methodName).FirstOrDefault(m =>
				{
					var parameters = m.GetParameters();
					if (parameters.Length != 2)
						return false;

					return parameters[0].ParameterType == typeof(object) && parameters[1].ParameterType == typeof(T);
				});

				if (method != null)
					_handler = (Action<object, T>)method.CreateDelegate(typeof(Action<object, T>), dataContext);
			}

			Log.DebugIf(_handler == null, "Unable to find method '{0}' with the appropriate signature on '{1}'.",
						_methodName, dataContextType.FullName);
		}

		/// <summary>
		///     Forwards the invocation of the routed event to the target method.
		/// </summary>
		private void OnEventRaised(object sender, T args)
		{
			Log.DebugIf(_dataContext == null, "Event invocation missed: Data context was a null value.");

			if (_dataContext != null && _handler != null)
				_handler(sender, args);
		}
	}
}