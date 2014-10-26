namespace Pegasus.UserInterface
{
	using System;
	using Platform.Logging;
	using Utilities;

	/// <summary>
	///     Binds a target UI element/dependency property pair to a resource and updates the target dependency property's
	///     value every time the resource changes.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	internal sealed class ResourceBinding<T> : Binding<T>
	{
		/// <summary>
		///     The key of the resource that is bound to the dependency property.
		/// </summary>
		private readonly object _key;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="key">The key of the resource that should be bound to the dependency property.</param>
		internal ResourceBinding(object key)
		{
			Assert.ArgumentNotNull(key);
			_key = key;
		}

		/// <summary>
		///     Gets the target object as an instance of UIElement.
		/// </summary>
		private UIElement TargetObject
		{
			get { return _targetObject as UIElement; }
		}

		/// <summary>
		///     Invoked when the binding has been activated.
		/// </summary>
		protected override void Activate()
		{
			Assert.OfType<UIElement>(_targetObject);

			TargetObject.ResourcesInvalidated += SetResource;
			SetResource();
		}

		/// <summary>
		///     Removes the binding. The binding can decide to ignore the removal if it would be overwritten by a local value. True is
		///     returned to indicate that the binding was removed.
		/// </summary>
		/// <param name="overwrittenByLocalValue">Indicates whether the binding is removed because it was overriden by a local value.</param>
		internal override bool Deactivate(bool overwrittenByLocalValue = false)
		{
			TargetObject.ResourcesInvalidated -= SetResource;
			return true;
		}

		/// <summary>
		///     Sets the bound resource to the target dependency property.
		/// </summary>
		private void SetResource()
		{
			if (!Active)
				return;

			object resource;
			if (!TargetObject.TryFindResource(_key, out resource))
			{
				Log.Debug("Resource binding failure: Unable to find resource '{0}'.", _key);
				_targetObject.SetBoundValue(_targetProperty, _targetProperty.DefaultValue);
			}
			else
				_targetObject.SetBoundValue(_targetProperty, (T)resource);
		}
	}
}