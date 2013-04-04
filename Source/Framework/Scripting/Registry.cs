using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;

	/// <summary>
	///   Provides access to named instances.
	/// </summary>
	/// <typeparam name="T">The type of the named instances.</typeparam>
	public abstract class Registry<T>
		where T : class
	{
		/// <summary>
		///   The registered instances.
		/// </summary>
		private readonly Dictionary<string, T> _instances = new Dictionary<string, T>();

		/// <summary>
		///   Gets the registered instances.
		/// </summary>
		internal IEnumerable<T> Instances
		{
			get { return _instances.Values; }
		}

		/// <summary>
		///   Registers the given instance.
		/// </summary>
		/// <param name="instance">The instance that should be registered.</param>
		/// <param name="name">The name of the instance.</param>
		protected void Register(T instance, string name)
		{
			Assert.ArgumentNotNull(instance, () => instance);
			Assert.ArgumentNotNullOrWhitespace(name, () => name);

			_instances.Add(name, instance);
		}

		/// <summary>
		///   Finds the instance with the given name. Returns false if no such instance is found.
		/// </summary>
		/// <param name="name">The name of the instance that should be returned.</param>
		/// <param name="instance">The instance with the given name, if it is found.</param>
		internal bool TryFind(string name, out T instance)
		{
			Assert.ArgumentNotNullOrWhitespace(name, () => name);
			return _instances.TryGetValue(name, out instance);
		}
	}
}