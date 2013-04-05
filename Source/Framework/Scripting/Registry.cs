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
		///   Initializes a new instance.
		/// </summary>
		protected Registry()
		{
			Initialize(null);
		}

		/// <summary>
		///   Gets all registered instances.
		/// </summary>
		internal IEnumerable<T> AllInstances
		{
			get { return _instances.Values; }
		}

		/// <summary>
		///   Provides access to the actual instances managed by the registry.
		/// </summary>
		public InstanceList Instances { get; private set; }

		/// <summary>
		///   Initializes the registry.
		/// </summary>
		/// <param name="instances">The instances that are registered on the registry.</param>
		protected virtual void Initialize(object instances)
		{
			Instances = (InstanceList)instances;
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

		/// <summary>
		///   Stores the instances of the registry.
		/// </summary>
		public abstract class InstanceList
		{
		}
	}
}