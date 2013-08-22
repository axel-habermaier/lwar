using System;

namespace Pegasus.Framework.UserInterface
{
	using Platform.Logging;

	/// <summary>
	///   Binds a target UI element/dependency property pair to a resource and updates the target dependency property's
	///   value every time the resource changes.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	public class ResourceBinding<T>
	{
		/// <summary>
		///   The key of the resource that is bound to the dependency property.
		/// </summary>
		private readonly string _key;

		/// <summary>
		///   The target UI element that defines the target dependency property.
		/// </summary>
		private UIElement _targetObject;

		/// <summary>
		///   The target dependency property whose value is bound.
		/// </summary>
		private DependencyProperty<T> _targetProperty;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="key">The key of the resource that should be bound to the dependency property.</param>
		public ResourceBinding(string key)
		{
			Assert.ArgumentNotNullOrWhitespace(key);
			_key = key;
		}

		/// <summary>
		///   Gets a value indicating whether the resource binding has already been bound to a dependency property.
		/// </summary>
		public bool IsBound { get; private set; }

		/// <summary>
		///   Initializes the binding.
		/// </summary>
		/// <param name="targetObject">The target UI element that defines the target dependency property.</param>
		/// <param name="targetProperty">The target dependency property whose value should be bound.</param>
		internal void Initialize(UIElement targetObject, DependencyProperty<T> targetProperty)
		{
			Assert.ArgumentNotNull(targetObject);
			Assert.ArgumentNotNull(targetProperty);
			Assert.That(!IsBound, "The binding has already been bound to a dependency property.");

			_targetObject = targetObject;
			_targetProperty = targetProperty;

			_targetObject.ResourcesInvalidated += SetResource;
			SetResource();

			IsBound = true;
		}

		/// <summary>
		///   Sets the bound resource to the target dependency property.
		/// </summary>
		private void SetResource()
		{
			object resource;
			if (!_targetObject.TryFindResource(_key, out resource))
			{
				Log.Warn("Unable to find resource '{0}'.", _key);
				_targetObject.SetValue(_targetProperty, _targetProperty.DefaultValue);
			}
			else
				_targetObject.SetValue(_targetProperty, (T)resource);
		}
	}
}