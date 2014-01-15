namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	///     Provides resources used by UI elements.
	/// </summary>
	public struct ResourceDictionary
	{
		/// <summary>
		///     The underlying dictionary instance that stores the resources.
		/// </summary>
		private Dictionary<object, object> _dictionary;

		/// <summary>
		///     Gets the number of resources contained in the dictionary.
		/// </summary>
		public int Count
		{
			get { return _dictionary.Count; }
		}

		/// <summary>
		///     Gets the resource with the specified key.
		/// </summary>
		/// <param name="key">The key of the resource to get or set.</param>
		public object this[object key]
		{
			get { return _dictionary[key]; }
		}

		/// <summary>
		///     Gets a value indicating whether the resource dictionary has already been initialized.
		/// </summary>
		internal bool IsInitialized
		{
			get { return _dictionary != null; }
		}

		/// <summary>
		///     Initializes the resource dictionary.
		/// </summary>
		internal void Initialize()
		{
			Assert.That(!IsInitialized, "The resource dictionary has already been initialized.");
			_dictionary = new Dictionary<object, object>();
		}

		/// <summary>
		///     Raised when a key of the resource dictionary has changed.
		/// </summary>
		internal event ResourceKeyChangedHandler ResourceChanged;

		/// <summary>
		///     Adds a resource with the provided key to the dictionary.
		/// </summary>
		/// <param name="key">The key of the resource that should be add.</param>
		/// <param name="resource">The resource that should be added.</param>
		public void Add(object key, object resource)
		{
			Assert.ArgumentNotNull(key);
			Assert.That(!_dictionary.ContainsKey(key), "A resource with key '{0}' already exists.", key);
			Assert.ArgumentNotNull(resource);

			_dictionary.Add(key, resource);
			SealResource(resource);
			RaiseChangeEvent(key);
		}

		/// <summary>
		///     Adds a resource with the provided key to the dictionary. If a resource with the given key already exists, it is
		///     replaced by the new resource.
		/// </summary>
		/// <param name="key">The key of the resource that should be add.</param>
		/// <param name="resource">The resource that should be added.</param>
		public void AddOrReplace(object key, object resource)
		{
			Assert.ArgumentNotNull(key);
			Assert.ArgumentNotNull(resource);

			_dictionary[key] = resource;
			SealResource(resource);
			RaiseChangeEvent(key);
		}

		/// <summary>
		///     Removes the resource with the specified key from the dictionary.
		/// </summary>
		/// <param name="key">The key of the resource that should be removed.</param>
		public bool Remove(object key)
		{
			Assert.ArgumentNotNull(key);

			if (!_dictionary.Remove(key))
				return false;

			RaiseChangeEvent(key);
			return true;
		}

		/// <summary>
		///     Removes all resources from the dictionary.
		/// </summary>
		public void Clear()
		{
			var keys = _dictionary.Keys.ToArray();
			foreach (var key in keys)
				Remove(key);
		}

		/// <summary>
		///     Determines whether the dictionary contains a resource with the specified key.
		/// </summary>
		public bool ContainsKey(object key)
		{
			return _dictionary.ContainsKey(key);
		}

		/// <summary>
		///     Gets the resource associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the resource that should be returned.</param>
		/// <param name="resource">Returns the resource with the specified key, if it is found.</param>
		public bool TryGetValue(object key, out object resource)
		{
			Assert.ArgumentNotNull(key);
			return _dictionary.TryGetValue(key, out resource);
		}

		/// <summary>
		///     Seals the given resource, if necessary.
		/// </summary>
		/// <param name="resource">The resource that should be sealed.</param>
		private static void SealResource(object resource)
		{
			var sealable = resource as ISealable;
			if (sealable != null && !sealable.IsSealed)
				sealable.Seal();
		}

		/// <summary>
		///     Raises the resource changed event.
		/// </summary>
		/// <param name="key">The key of the resource that has been changed.</param>
		private void RaiseChangeEvent(object key)
		{
			Assert.ArgumentNotNull(key);

			if (ResourceChanged != null)
				ResourceChanged(this, key);
		}
	}
}