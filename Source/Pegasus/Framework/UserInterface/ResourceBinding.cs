using System;

namespace Pegasus.Framework.UserInterface
{
	using Platform.Logging;

	/// <summary>
	///   Binds a target UI element/dependency property pair to a resource and updates the target dependency property's
	///   value every time the resource changes.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	internal sealed class ResourceBinding<T> : Binding<T>
	{
		/// <summary>
		///   The key of the resource that is bound to the dependency property.
		/// </summary>
		private readonly object _key;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="key">The key of the resource that should be bound to the dependency property.</param>
		public ResourceBinding(object key)
		{
			Assert.ArgumentNotNull(key);
			_key = key;
		}

		/// <summary>
		///   Gets the target object as an instance of UIElement.
		/// </summary>
		private UIElement TargetObject
		{
			get { return _targetObject as UIElement; }
		}

		/// <summary>
		///   Initializes the binding.
		/// </summary>
		protected override void Initialize()
		{
			Assert.OfType<UIElement>(_targetObject);

			TargetObject.ResourcesInvalidated += SetResource;
			SetResource();
		}

		/// <summary>
		///   Sets the bound resource to the target dependency property.
		/// </summary>
		private void SetResource()
		{
			object resource;
			if (!TargetObject.TryFindResource(_key, out resource))
			{
				Log.Warn("Unable to find resource '{0}'.", _key);
				_targetObject.SetValue(_targetProperty, _targetProperty.DefaultValue);
			}
			else
				_targetObject.SetValue(_targetProperty, (T)resource);
		}
	}
}