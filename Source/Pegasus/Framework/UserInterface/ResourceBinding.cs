namespace Pegasus.Framework.UserInterface
{
	using System;
	using Platform.Logging;

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
		protected override void OnActivated()
		{
			SetResource();
		}

		/// <summary>
		///     Initializes the binding.
		/// </summary>
		protected override void Initialize()
		{
			Assert.OfType<UIElement>(_targetObject);
			TargetObject.ResourcesInvalidated += SetResource;
		}

		/// <summary>
		///     Removes the binding.
		/// </summary>
		internal override void Remove()
		{
			TargetObject.ResourcesInvalidated -= SetResource;
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