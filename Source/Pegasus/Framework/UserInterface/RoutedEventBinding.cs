﻿namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Linq.Expressions;
	using Platform.Logging;

	/// <summary>
	///     Binds a UI element/routed event pair to a method of the UI element's data context.
	/// </summary>
	/// <typeparam name="T">The type of the event arguments.</typeparam>
	internal class RoutedEventBinding<T>
		where T : RoutedEventArgs
	{
		/// <summary>
		///     The name of the method that is invoked when the routed event is raised.
		/// </summary>
		private readonly string _methodName;

		/// <summary>
		///     The routed event that is bound to.
		/// </summary>
		private readonly RoutedEvent<T> _routedEvent;

		/// <summary>
		///     The UI element that raises the routed event.
		/// </summary>
		private readonly UIElement _uiElement;

		/// <summary>
		///     The data context the event handler is invoked on.
		/// </summary>
		private object _dataContext;

		/// <summary>
		///     The handler registered on the routed event that invokes the target method.
		/// </summary>
		private Action<object, object, T> _handler;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="uiElement">The UI element that raises the routed event.</param>
		/// <param name="routedEvent">The routed event that should be bound to.</param>
		/// <param name="method">The name of the method that should be invoked when the routed event is raised.</param>
		public RoutedEventBinding(UIElement uiElement, RoutedEvent<T> routedEvent, string method)
		{
			Assert.ArgumentNotNull(uiElement);
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNullOrWhitespace(method);

			_uiElement = uiElement;
			_routedEvent = routedEvent;
			_methodName = method;

			uiElement.AddHandler(routedEvent, OnEventRaised);
			uiElement.AddChangedHandler(UIElement.DataContextProperty, OnDataContextChanged);
			InitializeBinding(uiElement.DataContext);
		}

		/// <summary>
		///     Reinitializes the event binding for the new data context.
		/// </summary>
		private void OnDataContextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<object> args)
		{
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

			var oldDataContext = _dataContext;
			_dataContext = dataContext;
			if (dataContext == null)
				return;

			// Reuse the handler expression if possible.
			if (oldDataContext != null && oldDataContext.GetType() == _dataContext.GetType())
				return;

			Expression handler = null;
			var dataContextType = dataContext.GetType();
			var target = Expression.Parameter(typeof(object));
			var sender = Expression.Parameter(typeof(object));
			var args = Expression.Parameter(typeof(T));

			// First, try to bind to a method of the appropriate name without any parameters.
			var method = dataContextType.GetMethod(_methodName, Type.EmptyTypes);
			if (method != null)
				handler = Expression.Call(Expression.Convert(target, dataContextType), method);
			else
			{
				// Otherwise, try to bind to a method of the appropriate name with a RoutedEventHandler<T> signature.
				method = dataContextType.GetMethod(_methodName, new[] { typeof(object), typeof(T) });

				if (method != null)
					handler = Expression.Call(Expression.Convert(target, dataContextType), method, sender, args);
			}

			Log.DebugIf(method == null, "Unable to find method '{0}' with the appropriate signature on '{1}'.",
				_methodName, dataContextType.FullName);

			if (handler == null)
				return;

			_handler = Expression.Lambda<Action<object, object, T>>(handler, target, sender, args).Compile();
		}

		/// <summary>
		///     Forwards the invocation of the routed event to the target method.
		/// </summary>
		private void OnEventRaised(object sender, T args)
		{
			Log.DebugIf(_dataContext == null, "Event invocation missed: Data context was a null value.");

			if (_dataContext != null && _handler != null)
				_handler(_dataContext, sender, args);
		}
	}
}