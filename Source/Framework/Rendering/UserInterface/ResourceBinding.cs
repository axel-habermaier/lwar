using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Binds a target UI element/dependency property pair to a resource and updates the target dependency property's
	///   value every time the resource changes.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	internal class ResourceBinding<T>
	{
		/// <summary>
		///   The key of the resource that is bound to the dependency property.
		/// </summary>
		private readonly string _key;

		/// <summary>
		///   The target UI element that defines the target dependency property.
		/// </summary>
		private readonly UIElement _targetObject;

		/// <summary>
		///   The target dependency property whose value is bound.
		/// </summary>
		private readonly DependencyProperty<T> _targetProperty;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="targetObject">The target UI element that defines the target dependency property.</param>
		/// <param name="targetProperty">The target dependency property whose value should be bound.</param>
		/// <param name="key">The key of the resource that should be bound to the dependency property.</param>
		internal ResourceBinding(UIElement targetObject, DependencyProperty<T> targetProperty, string key)
		{
			Assert.ArgumentNotNull(targetObject);
			Assert.ArgumentNotNull(targetProperty);
			Assert.ArgumentNotNullOrWhitespace(key);

			_targetObject = targetObject;
			_targetProperty = targetProperty;
			_key = key;

			_targetObject.ResourcesInvalidated += SetResource;
			SetResource();
		}

		/// <summary>
		///   Sets the bound resource to the target dependency property.
		/// </summary>
		private void SetResource()
		{
			object resource;
			if (!_targetObject.TryFindResource(_key, out resource))
				Assert.That(false, "Unable to find resource '{0}'.", _key);

			_targetObject.SetValue(_targetProperty, (T)resource);
		}
	}
}