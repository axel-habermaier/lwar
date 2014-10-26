namespace Pegasus.Entities
{
	using System;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Holds information about an entity.
	/// </summary>
	internal sealed class EntityInfo : UniquePooledObject
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static EntityInfo()
		{
			ConstructorCache.Set(() => new EntityInfo());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private EntityInfo()
		{
		}

		/// <summary>
		///     Gets or sets the next entity in an intrusive entity list.
		/// </summary>
		internal EntityInfo Next { get; set; }

		/// <summary>
		///     Gets or sets the previous entity in an intrusive entity list.
		/// </summary>
		internal EntityInfo Previous { get; set; }

		/// <summary>
		///     Gets the entity that the information is provided for.
		/// </summary>
		internal Entity Entity { get; private set; }

		/// <summary>
		///     Gets the entity's first component that can be used to iterate through all components of this entity.
		/// </summary>
		internal Component Components { get; private set; }

		/// <summary>
		///     Adds the given component to the entity.
		/// </summary>
		/// <param name="component">The component that should be added.</param>
		internal void Add(Component component)
		{
			Assert.ArgumentNotNull(component);
			Assert.ArgumentSatisfies(component.Next == null, "The component is already in use elsewhere.");

			component.Next = Components;
			Components = component;
		}

		/// <summary>
		///     Gets the first component of the given type. If the entity does not have any component of the given type,
		///     null is returned.
		/// </summary>
		/// <typeparam name="T">The type of the component that should be returned.</typeparam>
		internal T GetComponent<T>()
			where T : Component
		{
			Assert.NotPooled(this);

			for (var c = Components; c != null; c = c.Next)
			{
				var component = c as T;
				if (component != null)
					return component;
			}

			return null;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the object.</param>
		/// <param name="entity">The entity that the information is provided for.</param>
		public static EntityInfo Create(PoolAllocator allocator, Entity entity)
		{
			Assert.ArgumentNotNull(allocator);

			var info = allocator.Allocate<EntityInfo>();
			info.Entity = entity;
			return info;
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			Assert.IsNull(Next, "The entity is still is some list.");
			Assert.IsNull(Previous, "The entity is still is some list.");

			for (var component = Components; component != null;)
			{
				var tmp = component;
				component = component.Next;
				tmp.SafeDispose();
			}

			Components = null;
		}
	}
}