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
		///     The trigger mode of the input binding.
		/// </summary>
		private TriggerMode _triggerMode = TriggerMode.Activated;

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
		///     Gets or sets the trigger mode of the input binding.
		/// </summary>
		public TriggerMode TriggerMode
		{
			get { return _triggerMode; }
			set
			{
				Assert.ArgumentInRange(value);
				Assert.NotSealed(this);

				_triggerMode = value;
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
	}
}