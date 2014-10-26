namespace Pegasus.Entities
{
	using System;
	using Utilities;

	/// <summary>
	///     A base class for entity behaviors that accesses two components.
	/// </summary>
	/// <typeparam name="T1">The type of the first component.</typeparam>
	/// <typeparam name="T2">The type of the second component.</typeparam>
	public abstract class EntityBehavior<T1, T2> : EntityBehavior, IEventHandler<EntityAddedEvent>, IEventHandler<EntityRemovedEvent>
		where T1 : Component
		where T2 : Component
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="capacity">The initial capacity of the internal buffers.</param>
		protected EntityBehavior(int capacity = 8)
			: this(ComponentDependency.Default, ComponentDependency.Default, capacity)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="componentDependency1">The kind of the behavior's dependency on the first component.</param>
		/// <param name="componentDependency2">The kind of the behavior's dependency on the second component.</param>
		/// <param name="capacity">The initial capacity of the internal buffers.</param>
		protected EntityBehavior(ComponentDependency componentDependency1, ComponentDependency componentDependency2, int capacity = 8)
			: base(2, capacity, componentDependency1, componentDependency2)
		{
		}

		/// <summary>
		///     Handles the event for the given entity.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		/// <param name="entity">The entity the event was raised for.</param>
		void IEventHandler<EntityAddedEvent>.HandleEvent(EntityAddedEvent eventArgs, Entity entity)
		{
			T1 component1;
			T2 component2;

			GetComponents(entity, out component1, out component2);

			if (!CheckDependency(component1, 0) || !CheckDependency(component2, 1))
				return;

			var index = Allocate(entity);
			Components[0][index] = component1;
			Components[1][index] = component2;

			OnAdded(entity, component1, component2);
		}

		/// <summary>
		///     Handles the event for the given entity.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		/// <param name="entity">The entity the event was raised for.</param>
		void IEventHandler<EntityRemovedEvent>.HandleEvent(EntityRemovedEvent eventArgs, Entity entity)
		{
			var index = GetIndex(entity);
			if (index == -1)
				return;

			OnRemoved(entity, (T1)Components[0][index], (T2)Components[1][index]);
			Free(entity, index);
		}

		/// <summary>
		///     Gets the components for the given entity. By default, the components are retrieved from the given entity itself. On the
		///     other hand, overriding methods could retrieve components from related entities, if they so desire.
		/// </summary>
		/// <param name="entity">The entity the components should be retrieved for.</param>
		/// <param name="component1">The first component of the entity.</param>
		/// <param name="component2">The second component of the entity.</param>
		protected virtual void GetComponents(Entity entity, out T1 component1, out T2 component2)
		{
			component1 = entity.GetComponent<T1>();
			component2 = entity.GetComponent<T2>();
		}

		/// <summary>
		///     Invoked when the given entity is affected by the behavior.
		/// </summary>
		/// <param name="entity">The entity that is affected by the behavior.</param>
		/// <param name="component1">The first component of the entity that is affected by the behavior.</param>
		/// <param name="component2">The second component of the entity that is affected by the behavior.</param>
		protected virtual void OnAdded(Entity entity, T1 component1, T2 component2)
		{
		}

		/// <summary>
		///     Invoked when the given entity is no longer affected by the behavior.
		/// </summary>
		/// <param name="entity">The entity that is no longer affected by the behavior.</param>
		/// <param name="component1">The first component of the entity that is no longer affected by the behavior.</param>
		/// <param name="component2">The second component of the entity that is no longer affected by the behavior.</param>
		protected virtual void OnRemoved(Entity entity, T1 component1, T2 component2)
		{
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		protected void Process()
		{
			Process(Entities, (T1[])Components[0], (T2[])Components[1], AffectedEntityCount);
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="components1">The first components of the affected entities.</param>
		/// <param name="components2">The second components of the affected entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected abstract void Process(Entity[] entities, T1[] components1, T2[] components2, int count);

		/// <summary>
		///     Allocates or resizes a component array for the components stored at the given index.
		/// </summary>
		/// <param name="index">The index of the components that should be allocated.</param>
		internal override void AllocateArray(int index)
		{
			switch (index)
			{
				case 0:
					AllocateArray<T1>(index);
					break;
				case 1:
					AllocateArray<T2>(index);
					break;
				default:
					throw new InvalidOperationException("Unexpected components index.");
			}
		}
	}
}