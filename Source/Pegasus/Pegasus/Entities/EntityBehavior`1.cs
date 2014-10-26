namespace Pegasus.Entities
{
	using System;

	/// <summary>
	///     A base class for entity behaviors that accesses one component.
	/// </summary>
	/// <typeparam name="T">The type of the component.</typeparam>
	public abstract class EntityBehavior<T> : EntityBehavior, IEventHandler<EntityAddedEvent>, IEventHandler<EntityRemovedEvent>
		where T : Component
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="capacity">The initial capacity of the internal buffers.</param>
		protected EntityBehavior(int capacity = 8)
			: base(1, capacity, ComponentDependency.Default)
		{
		}

		/// <summary>
		///     Handles the event for the given entity.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		/// <param name="entity">The entity the event was raised for.</param>
		void IEventHandler<EntityAddedEvent>.HandleEvent(EntityAddedEvent eventArgs, Entity entity)
		{
			var component = entity.GetComponent<T>();
			if (component == null)
				return;

			var index = Allocate(entity);
			Components[0][index] = component;

			OnAdded(entity, component);
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

			OnRemoved(entity, (T)Components[0][index]);
			Free(entity, index);
		}

		/// <summary>
		///     Invoked when the given entity is affected by the behavior.
		/// </summary>
		/// <param name="entity">The entity that is affected by the behavior.</param>
		/// <param name="component">The component of the entity that is affected by the behavior.</param>
		protected virtual void OnAdded(Entity entity, T component)
		{
		}

		/// <summary>
		///     Invoked when the given entity is no longer affected by the behavior.
		/// </summary>
		/// <param name="entity">The entity that is no longer affected by the behavior.</param>
		/// <param name="component">The component of the entity that is no longer affected by the behavior.</param>
		protected virtual void OnRemoved(Entity entity, T component)
		{
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		protected void Process()
		{
			Process(Entities, (T[])Components[0], AffectedEntityCount);
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="components">The components of the affected entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected abstract void Process(Entity[] entities, T[] components, int count);

		/// <summary>
		///     Allocates or resizes a component array for the components stored at the given index.
		/// </summary>
		/// <param name="index">The index of the components that should be allocated.</param>
		internal override void AllocateArray(int index)
		{
			switch (index)
			{
				case 0:
					AllocateArray<T>(index);
					break;
				default:
					throw new InvalidOperationException("Unexpected components index.");
			}
		}
	}
}