namespace Pegasus.Entities
{
	using System;
	using System.Collections.Generic;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents an intrusive doubly-linked list of entity infos.
	/// </summary>
	internal class EntityInfoList
	{
		/// <summary>
		///     The first entity of the list.
		/// </summary>
		private EntityInfo _first;

		/// <summary>
		///     The last entity of the list.
		/// </summary>
		private EntityInfo _last;

		/// <summary>
		///     Gets a list of all entities. This property should only be used for debugging purposes by the debugger.
		/// </summary>
		[UsedImplicitly]
		private List<Entity> Entities
		{
			get
			{
				var entities = new List<Entity>();
				for (var entityInfo = _first; entityInfo != null; entityInfo = entityInfo.Next)
					entities.Add(entityInfo.Entity);

				return entities;
			}
		}

		/// <summary>
		///     Gets an enumerator that can be used to enumerate all active entities.
		/// </summary>
		public Enumerator GetEnumerator()
		{
			return new Enumerator(_first);
		}

		/// <summary>
		///     Adds the given entity to the end of the list.
		/// </summary>
		/// <param name="entity">The entity that should be added to the list.</param>
		public void Add(EntityInfo entity)
		{
			Assert.ArgumentNotNull(entity);
			Assert.ArgumentSatisfies(!Contains(entity), "The entity is already contained in the list.");
			Assert.ArgumentSatisfies(entity.Next == null && entity.Previous == null, "The entity is already contained in a list.");

			if (_first == null)
			{
				_first = entity;
				_last = entity;
			}
			else
			{
				entity.Previous = _last;
				_last.Next = entity;
				_last = entity;
			}
		}

		/// <summary>
		///     Adds the given entity to the end of the list.
		/// </summary>
		/// <param name="entities">The entities that should be added to the list.</param>
		public void Add(EntityInfoList entities)
		{
			if (entities._first == null)
				return;

			if (_first == null)
			{
				_first = entities._first;
				_last = entities._last;
			}
			else
			{
				entities._first.Previous = _last;
				_last.Next = entities._first;
				_last = entities._last;
			}

			entities._first = null;
			entities._last = null;
		}

		/// <summary>
		///     Removes the given entity from the list.
		/// </summary>
		/// <param name="entity">The entity that should be removed.</param>
		public void Remove(EntityInfo entity)
		{
			Assert.ArgumentNotNull(entity);
			Assert.ArgumentSatisfies(Contains(entity), "The entity is not contained in the list.");

			if (entity.Next != null)
				entity.Next.Previous = entity.Previous;

			if (entity.Previous != null)
				entity.Previous.Next = entity.Next;

			if (_first == entity)
				_first = entity.Next;

			if (_last == entity)
				_last = entity.Previous;

			entity.Next = null;
			entity.Previous = null;
		}

		/// <summary>
		///     Checks whether the given entity is contained in the list.
		/// </summary>
		/// <param name="entity">The entity that should be checked.</param>
		public bool Contains(EntityInfo entity)
		{
			Assert.ArgumentNotNull(entity);

			var current = _first;
			while (current != null)
			{
				if (current == entity)
					return true;

				current = current.Next;
			}

			return false;
		}

		/// <summary>
		///     Disposes all entities contained in the list.
		/// </summary>
		public void SafeDisposeAll()
		{
			foreach (var entity in this)
			{
				entity.Next = null;
				entity.Previous = null;
				entity.SafeDispose();
			}

			_first = null;
			_last = null;
		}

		/// <summary>
		///     Enumerates a list of entities.
		/// </summary>
		internal struct Enumerator
		{
			/// <summary>
			///     The remaining entities that have yet to be enumerated.
			/// </summary>
			private EntityInfo _list;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="list">The list of entities that should be enumerated.</param>
			public Enumerator(EntityInfo list)
				: this()
			{
				_list = list;
			}

			/// <summary>
			///     Gets the entity at the current position of the enumerator.
			/// </summary>
			public EntityInfo Current { get; private set; }

			/// <summary>
			///     Advances the enumerator to the next item.
			/// </summary>
			public bool MoveNext()
			{
				Current = _list;
				if (Current == null)
					return false;

				_list = _list.Next;
				return true;
			}
		}
	}
}