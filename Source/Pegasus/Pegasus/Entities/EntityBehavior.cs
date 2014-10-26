namespace Pegasus.Entities
{
	using System;
	using System.Collections.Generic;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     A base class for entity behaviors.
	/// </summary>
	public abstract class EntityBehavior : DisposableObject, IEventHandler
	{
		/// <summary>
		///     The stored components of the behavior.
		/// </summary>
		internal readonly object[][] Components;

		/// <summary>
		///     The behavior's dependency kinds on the components.
		/// </summary>
		private readonly ComponentDependency[] _componentDependencies;

		/// <summary>
		///     Maps an entity to the index of the component arrays where its components are stored.
		/// </summary>
		private readonly Dictionary<Entity, int> _map;

		/// <summary>
		///     The entities associated with the stored data.
		/// </summary>
		internal Entity[] Entities;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="componentCount">The number of components that should actually be stored.</param>
		/// <param name="capacity">The initial capacity of the component arrays.</param>
		/// <param name="componentDependencies">The behavior's dependency kinds on the components.</param>
		internal EntityBehavior(int componentCount, int capacity, params ComponentDependency[] componentDependencies)
		{
			Assert.ArgumentSatisfies(componentCount > 0, "Invalid component count.");
			Assert.ArgumentSatisfies(capacity > 0, "Invalid capacity.");
			Assert.ArgumentNotNull(componentDependencies);
			Assert.ArgumentSatisfies(componentDependencies.Length == componentCount, "Invalid number of component dependencies.");

			Components = new object[componentCount][];
			Entities = new Entity[capacity];

			_componentDependencies = componentDependencies;
			_map = new Dictionary<Entity, int>();

			// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			for (var i = 0; i < Components.Length; ++i)
				AllocateArray(i);
		}

		/// <summary>
		///     Gets The capacity of the component arrays.
		/// </summary>
		private int Capacity
		{
			get { return Entities.Length; }
		}

		/// <summary>
		///     Gets the number of entities that are affected by the behavior
		/// </summary>
		internal int AffectedEntityCount { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
		}

		/// <summary>
		///     Allocates or resizes a component array for the components stored at the given index.
		/// </summary>
		/// <param name="index">The index of the components that should be allocated.</param>
		internal abstract void AllocateArray(int index);

		/// <summary>
		///     Allocates or resizes a component array for the components stored at the given index.
		/// </summary>
		/// <param name="index">The index of the components that should be allocated.</param>
		internal void AllocateArray<T>(int index)
			where T : Component
		{
			Assert.ArgumentInRange(index, Components);

			var typedArray = (T[])Components[index];
			if (Components[index] == null)
				typedArray = new T[Capacity];
			else
				Array.Resize(ref typedArray, Capacity);

			Components[index] = typedArray;
		}

		/// <summary>
		///     Checks whether the dependency on the given component is fulfilled.
		/// </summary>
		/// <param name="component">The component that should be checked.</param>
		/// <param name="index">The index of the component.</param>
		internal bool CheckDependency<T>(T component, int index)
			where T : Component
		{
			if (component != null)
				return true;

			switch (_componentDependencies[index])
			{
				case ComponentDependency.Default:
					return false;
				case ComponentDependency.Optional:
					return true;
				case ComponentDependency.Required:
					Assert.NotReached("Expected a component of type '{0}'.", typeof(T).FullName);
					return false;
				default:
					throw new InvalidOperationException("Unknown kind of component dependency.");
			}
		}

		/// <summary>
		///     Allocates storage for the given entity, returning the storage index for the entity. If storage has already been
		///     allocated for the entity, the previous index is returned.
		/// </summary>
		/// <param name="entity">The entity that should be stored.</param>
		internal int Allocate(Entity entity)
		{
			int index;
			if (_map.TryGetValue(entity, out index))
				return index;

			if (AffectedEntityCount >= Capacity)
				IncreaseCapacity();

			Entities[AffectedEntityCount] = entity;
			_map.Add(entity, AffectedEntityCount);

			return AffectedEntityCount++;
		}

		/// <summary>
		///     Frees the storage for the given entity.
		/// </summary>
		/// <param name="entity">The entity that should no longer be stored.</param>
		/// <param name="index">The entity's data index.</param>
		internal void Free(Entity entity, int index)
		{
			Assert.ArgumentSatisfies(GetIndex(entity) == index, "Invalid entity data index.");

			_map.Remove(entity);

			--AffectedEntityCount;
			if (AffectedEntityCount <= 0 || index == AffectedEntityCount)
				return;

			_map[Entities[AffectedEntityCount]] = index;
			Entities[index] = Entities[AffectedEntityCount];

			for (var i = 0; i < Components.Length; ++i)
				Components[i][index] = Components[i][AffectedEntityCount];
		}

		/// <summary>
		///     Gets the index for the given entity. If no data is stored for the given entity, -1 is returned.
		/// </summary>
		/// <param name="entity">The entity the index should be returned for.</param>
		internal int GetIndex(Entity entity)
		{
			int index;
			if (!_map.TryGetValue(entity, out index))
				return -1;

			return index;
		}

		/// <summary>
		///     Increases the capacity of the arrays.
		/// </summary>
		private void IncreaseCapacity()
		{
			var capacity = (int)(Capacity * 1.5);
			if (capacity == Capacity)
				++capacity;

			Array.Resize(ref Entities, capacity);
			for (var i = 0; i < Components.Length; ++i)
				AllocateArray(i);
		}
	}
}