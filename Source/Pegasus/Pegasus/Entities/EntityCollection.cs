namespace Pegasus.Entities
{
	using System;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents a collection of entities.
	/// </summary>
	public sealed class EntityCollection : DisposableObject
	{
		/// <summary>
		///     The list of active entities.
		/// </summary>
		private readonly IntrusiveList _activeEntities = new IntrusiveList();

		/// <summary>
		///     The list of added entities.
		/// </summary>
		private readonly IntrusiveList _addedEntities = new IntrusiveList();

		/// <summary>
		///     The allocator that is used to allocate pooled objects.
		/// </summary>
		private readonly PoolAllocator _allocator;

		/// <summary>
		///     The event dispatcher that is used to dispatch entity events to the behaviors.
		/// </summary>
		private readonly EventDispatcher _eventDispatcher;

		/// <summary>
		///     The list of removed entities.
		/// </summary>
		private readonly IntrusiveList _removedEntities = new IntrusiveList();

		/// <summary>
		///     Maps active entities to their entity info object.
		/// </summary>
		private IdentityMap<EntityInfo> _entityInfoMap;

		/// <summary>
		///     The allocator that is used to allocate entity identities.
		/// </summary>
		private IdentityAllocator _identityAllocator;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate pooled objects.</param>
		/// <param name="eventDispatcher">The event dispatcher that should be used to dispatch entity events.</param>
		/// <param name="maxEntities">The maximum number of entities that can be active at the same time.</param>
		public EntityCollection(PoolAllocator allocator, EventDispatcher eventDispatcher, ushort maxEntities = UInt16.MaxValue)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNull(eventDispatcher);

			_allocator = allocator;
			_identityAllocator = new IdentityAllocator(maxEntities);
			_entityInfoMap = new IdentityMap<EntityInfo>(maxEntities);
			_eventDispatcher = eventDispatcher;
		}

		/// <summary>
		///     Gets an enumerator that enumerates all active entities contained in the collection.
		/// </summary>
		/// <remarks>
		///     An active entity is an entity that has already been fully added to the collection, but that has not yet been fully
		///     removed.
		/// </remarks>
		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		/// <summary>
		///     Creates a new entity.
		/// </summary>
		public Entity CreateEntity()
		{
			var identity = _identityAllocator.Allocate();
			var entity = new Entity(this, identity);
			var info = EntityInfo.Create(_allocator, entity);

			_entityInfoMap.Add(identity, info);
			_addedEntities.Add(info);

			return entity;
		}

		/// <summary>
		///     Removes the given entity from the collection.
		/// </summary>
		/// <param name="entity">The entity that should be removed.</param>
		public void Remove(Entity entity)
		{
			Assert.ArgumentSatisfies(entity.OwningCollection == this, "The entity does not belong to this collection.");

			if (!IsAlive(entity))
				return;

			var info = _entityInfoMap[entity.Identity];

			Assert.ArgumentSatisfies(!_removedEntities.Contains(info), "The entity has already been removed.");
			Assert.ArgumentSatisfies(_activeEntities.Contains(info) || _addedEntities.Contains(info),
				"The entity has not been added to the collection.");

			// We might be trying to remove an entity that we've just recently added; in that case, just remove the entity
			// and return it to the pool immediately.
			if (_addedEntities.Contains(info))
			{
				_addedEntities.Remove(info);

				_identityAllocator.Free(entity.Identity);
				_entityInfoMap.Remove(entity.Identity);
				info.SafeDispose();
			}
			else
			{
				_activeEntities.Remove(info);
				_removedEntities.Add(info);
				_identityAllocator.Invalidate(entity.Identity);
			}
		}

		/// <summary>
		///     Checks whether the given entity is alive and has not yet been removed from the collection.
		/// </summary>
		/// <param name="entity">The entity that should be checked.</param>
		internal bool IsAlive(Entity entity)
		{
			Assert.ArgumentSatisfies(entity.OwningCollection == this, "The entity does not belong to this collection.");
			return _identityAllocator.IsValid(entity.Identity);
		}

		/// <summary>
		///     Gets the entity's first component of the given type. If the entity does not have any component of the given type,
		///     null is returned.
		/// </summary>
		/// <typeparam name="T">The type of the component that should be returned.</typeparam>
		/// <param name="entity">The entity the component should be returned for.</param>
		internal T GetComponent<T>(Entity entity)
			where T : Component
		{
			Assert.ArgumentSatisfies(entity.OwningCollection == this, "The entity does not belong to this collection.");
			Assert.ArgumentSatisfies(IsAlive(entity) || _entityInfoMap.Contains(entity.Identity),
				"Cannot search for components when the entity has already been removed from the collection.");

			var info = _entityInfoMap[entity.Identity];
			return info.GetComponent<T>();
		}

		/// <summary>
		///     Gets the entity's first component that can be used to iterate the list of all components of the given entity.
		/// </summary>
		/// <param name="entity">The entity the components should be returned for.</param>
		internal Component GetComponents(Entity entity)
		{
			Assert.ArgumentSatisfies(entity.OwningCollection == this, "The entity does not belong to this collection.");
			Assert.ArgumentSatisfies(IsAlive(entity) || _entityInfoMap.Contains(entity.Identity),
				"Cannot search for components when the entity has already been removed from the collection.");

			return _entityInfoMap[entity.Identity].Components;
		}

		/// <summary>
		///     Adds the given component to the given entity.
		/// </summary>
		/// <param name="entity">The entity the component should be added to.</param>
		/// <param name="component">The component that should be added.</param>
		internal void AddComponent(Entity entity, Component component)
		{
			Assert.ArgumentSatisfies(entity.OwningCollection == this, "The entity does not belong to this collection.");
			Assert.ArgumentSatisfies(IsAlive(entity), "Cannot add components when the entity is not alive.");
			Assert.ArgumentNotNull(component);

			var info = _entityInfoMap[entity.Identity];
			Assert.That(_addedEntities.Contains(info), "Cannot add components to active entities.");

			info.Add(component);
		}

		/// <summary>
		///     Dispatches the given event for the given entity.
		/// </summary>
		/// <typeparam name="T">The type of the event that should be dispatched.</typeparam>
		/// <param name="eventArgs">The event arguments that should be dispatched.</param>
		/// <param name="entity">The entity the event should be dispatched for.</param>
		public void DispatchEvent<T>(T eventArgs, Entity entity)
			where T : struct
		{
			Assert.ArgumentSatisfies(entity.OwningCollection == this, "The entity does not belong to this collection.");
			Assert.ArgumentSatisfies(IsAlive(entity), "Cannot dispatch an event for a dead entity.");
			Assert.ArgumentSatisfies(_activeEntities.Contains(_entityInfoMap[entity.Identity]), "Cannot dispatch an event for an unknown entity.");

			_eventDispatcher.DispatchEvent(eventArgs, entity);
		}

		/// <summary>
		///     Applies the changes made to the entity collection.
		/// </summary>
		public void ApplyChanges()
		{
			foreach (var info in _removedEntities)
			{
				_eventDispatcher.DispatchEvent(new EntityRemovedEvent(), info.Entity);
				_entityInfoMap.Remove(info.Entity.Identity);
				_identityAllocator.Free(info.Entity.Identity);
			}

			foreach (var info in _addedEntities)
				_eventDispatcher.DispatchEvent(new EntityAddedEvent(), info.Entity);

			_removedEntities.SafeDisposeAll();
			_activeEntities.Add(_addedEntities);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_addedEntities.SafeDisposeAll();
			_activeEntities.SafeDisposeAll();
			_removedEntities.SafeDisposeAll();
		}

		/// <summary>
		///     Enumerates all active entities of an entity collection.
		/// </summary>
		public struct Enumerator
		{
			/// <summary>
			///     The entities that have yet to be enumerated.
			/// </summary>
			private readonly EntityCollection _collection;

			/// <summary>
			///     The enumerator of the entity list that is currently enumerated.
			/// </summary>
			private IntrusiveList.Enumerator _enumerator;

			/// <summary>
			///     Indicates which list of the collection is currently enumerated.
			/// </summary>
			private int _state;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="collection">The collection of entities that should be enumerated.</param>
			public Enumerator(EntityCollection collection)
				: this()
			{
				Assert.ArgumentNotNull(collection);

				_collection = collection;
				_enumerator = _collection._activeEntities.GetEnumerator();
			}

			/// <summary>
			///     Gets the entity at the current position of the enumerator.
			/// </summary>
			public Entity Current { get; private set; }

			/// <summary>
			///     Advances the enumerator to the next item.
			/// </summary>
			public bool MoveNext()
			{
				while (!_enumerator.MoveNext() && _state < 2)
				{
					switch (_state++)
					{
						case 0:
							_enumerator = _collection._removedEntities.GetEnumerator();
							break;
						default:
							return false;
					}
				}

				if (_enumerator.Current == null)
					return false;

				Current = _enumerator.Current.Entity;
				return true;
			}
		}
	}
}