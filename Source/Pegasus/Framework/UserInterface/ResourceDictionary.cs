using System;

namespace Pegasus.Framework.UserInterface
{
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	///   Provides resources used by UI elements.
	/// </summary>
	public class ResourceDictionary
	{
		/// <summary>
		///   The underlying dictionary instance.
		/// </summary>
		private readonly Dictionary<object, object> _dictionary = new Dictionary<object, object>();

		/// <summary>
		///   Gets the number of resources contained in the dictionary.
		/// </summary>
		public int Count
		{
			get { return _dictionary.Count; }
		}

		/// <summary>
		///   Gets or sets the resource with the specified key.
		/// </summary>
		/// <param name="key">The key of the resource to get or set.</param>
		public object this[object key]
		{
			get { return _dictionary[key]; }
			set
			{
				_dictionary[key] = value;

				var sealable = value as ISealable;
				if (sealable != null && !sealable.IsSealed)
					sealable.Seal();

				RaiseChangeEvent(key);
			}
		}

		/// <summary>
		///   Raised when a key of the resource dictionary has changed.
		/// </summary>
		internal event ResourceKeyChangedHandler ResourceChanged;

		/// <summary>
		///   Raises the resource changed event.
		/// </summary>
		/// <param name="key">The key of the resource that has been changed.</param>
		private void RaiseChangeEvent(object key)
		{
			Assert.ArgumentNotNull(key);

			if (ResourceChanged != null)
				ResourceChanged(this, key);
		}

		/// <summary>
		///   Adds a resource with the provided key to the dictionary.
		/// </summary>
		/// <param name="key">The key of the resource that should be add.</param>
		/// <param name="resource">The resource that should be added.</param>
		public void Add(object key, object resource)
		{
			_dictionary.Add(key, resource);

			var sealable = resource as ISealable;
			if (sealable != null && !sealable.IsSealed)
				sealable.Seal();

			RaiseChangeEvent(key);
		}

		/// <summary>
		///   Removes the resource with the specified key from the dictionary.
		/// </summary>
		/// <param name="key">The key of the resource that should be removed.</param>
		public bool Remove(object key)
		{
			if (!_dictionary.Remove(key))
				return false;

			RaiseChangeEvent(key);
			return true;
		}

		/// <summary>
		///   Removes all resources from the dictionary.
		/// </summary>
		public void Clear()
		{
			var keys = _dictionary.Keys.ToArray();
			foreach (var key in keys)
				Remove(key);
		}

		/// <summary>
		///   Determines whether the dictionary contains a resource with the specified key.
		/// </summary>
		public bool ContainsKey(object key)
		{
			return _dictionary.ContainsKey(key);
		}

		/// <summary>
		///   Gets the resource associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the resource that should be returned.</param>
		/// <param name="resource">Returns the resource with the specified key, if it is found.</param>
		public bool TryGetValue(object key, out object resource)
		{
			return _dictionary.TryGetValue(key, out resource);
		}
	}
}