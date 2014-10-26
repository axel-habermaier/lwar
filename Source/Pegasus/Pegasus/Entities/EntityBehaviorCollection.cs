namespace Pegasus.Entities
{
	using System;
	using System.Collections.Generic;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents a collection of entity behaviors.
	/// </summary>
	public sealed class EntityBehaviorCollection : DisposableObject
	{
		/// <summary>
		///     The entity behaviors stored in the collection.
		/// </summary>
		private readonly List<EntityBehavior> _behaviors = new List<EntityBehavior>();

		/// <summary>
		///     The event dispatcher that is used to dispatch entity events to the behaviors.
		/// </summary>
		private readonly EventDispatcher _eventDispatcher;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="eventDispatcher">The event dispatcher that should be used to dispatch entity events to the behaviors.</param>
		public EntityBehaviorCollection(EventDispatcher eventDispatcher)
		{
			Assert.ArgumentNotNull(eventDispatcher);
			_eventDispatcher = eventDispatcher;
		}

		/// <summary>
		///     Adds the given entity behavior to the collection.
		/// </summary>
		/// <param name="entityBehavior">The entity behavior that should be added.</param>
		public void Add(EntityBehavior entityBehavior)
		{
			Assert.ArgumentNotNull(entityBehavior);
			Assert.ArgumentSatisfies(!_behaviors.Contains(entityBehavior), "The entity behavior has already been added.");

			_behaviors.Add(entityBehavior);
			_eventDispatcher.AddEventHandler(entityBehavior);
		}

		/// <summary>
		///     Removes the given entity behavior from the collection.
		/// </summary>
		/// <param name="entityBehavior">The entity behavior that should be removed.</param>
		public void Remove(EntityBehavior entityBehavior)
		{
			Assert.ArgumentNotNull(entityBehavior);
			Assert.ArgumentSatisfies(_behaviors.Contains(entityBehavior), "Cannot remove unknown entity behavior.");

			_behaviors.Remove(entityBehavior);
			_eventDispatcher.RemoveEventHandler(entityBehavior);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_behaviors.SafeDisposeAll();
		}
	}
}