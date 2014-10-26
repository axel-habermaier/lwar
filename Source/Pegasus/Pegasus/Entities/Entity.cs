namespace Pegasus.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Utilities;

	/// <summary>
	///     Represents an entity.
	/// </summary>
	public struct Entity : IEquatable<Entity>
	{
		/// <summary>
		///     Represents a missing entity.
		/// </summary>
		public static readonly Entity None = new Entity();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="owningCollection">The collection that owns the entity.</param>
		/// <param name="identity">The unique identity of the entity.</param>
		internal Entity(EntityCollection owningCollection, Identity identity)
			: this()
		{
			Assert.ArgumentNotNull(owningCollection);

			Identity = identity;
			OwningCollection = owningCollection;
		}

		/// <summary>
		///     Gets a list of all components of the entity. This property should only be used for debugging purposes by the debugger.
		/// </summary>
		[UsedImplicitly]
		private List<Component> Components
		{
			get
			{
				if (!IsValid)
					return null;

				var components = new List<Component>();
				for (var component = OwningCollection.GetComponents(this); component != null; component = component.Next)
					components.Add(component);

				return components;
			}
		}

		/// <summary>
		///     Gets the collection the entity belongs to.
		/// </summary>
		internal EntityCollection OwningCollection { get; private set; }

		/// <summary>
		///     Gets the unique identity of the entity.
		/// </summary>
		internal Identity Identity { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the entity represents a valid instance.
		/// </summary>
		public bool IsValid
		{
			get { return OwningCollection != null; }
		}

		/// <summary>
		///     Gets a value indicating whether the entity has been removed from the game world.
		/// </summary>
		public bool IsDead
		{
			get { return !IsValid || !OwningCollection.IsAlive(this); }
		}

		/// <summary>
		///     Gets a value indicating whether the entity is currently active in the game world.
		/// </summary>
		public bool IsAlive
		{
			get { return IsValid && OwningCollection.IsAlive(this); }
		}

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Entity other)
		{
			return Equals(OwningCollection, other.OwningCollection) && Identity.Equals(other.Identity);
		}

		/// <summary>
		///     Removes the entity from the entity collection it belongs to, if any.
		/// </summary>
		public void Remove()
		{
			if (IsValid)
				OwningCollection.Remove(this);
		}

		/// <summary>
		///     Adds the given component to the entity.
		/// </summary>
		/// <param name="component">The component that should be added.</param>
		public void Add(Component component)
		{
			CheckIsValid();
			Assert.ArgumentNotNull(component);

			OwningCollection.AddComponent(this, component);
		}

		/// <summary>
		///     Gets the first component of the given type. If the entity does not have any component of the given type,
		///     null is returned.
		/// </summary>
		/// <typeparam name="T">The type of the component that should be returned.</typeparam>
		public T GetComponent<T>()
			where T : Component
		{
			CheckIsValid();
			return OwningCollection.GetComponent<T>(this);
		}

		/// <summary>
		///     Gets the first component of the given type. If the entity does not have any component of the given type,
		///     an assertion is raised in debug builds.
		/// </summary>
		/// <typeparam name="T">The type of the component that should be returned.</typeparam>
		public T GetRequiredComponent<T>()
			where T : Component
		{
			CheckIsValid();

			var component = GetComponent<T>();
			Assert.NotNull(component, "The entity does not have the required component of type '{0}'.", typeof(T).FullName);

			return component;
		}

		/// <summary>
		///     Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is Entity && Equals((Entity)obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((OwningCollection != null ? OwningCollection.GetHashCode() : 0) * 397) ^ Identity.GetHashCode();
			}
		}

		/// <summary>
		///     Checks whether the two given entities are equal.
		/// </summary>
		/// <param name="left">The first entity that should be checked.</param>
		/// <param name="right">The second entity that should be checked.</param>
		public static bool operator ==(Entity left, Entity right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Checks whether the two given entities are not equal.
		/// </summary>
		/// <param name="left">The first entity that should be checked.</param>
		/// <param name="right">The second entity that should be checked.</param>
		public static bool operator !=(Entity left, Entity right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		///     In debug builds, checks whether the entity is valid.
		/// </summary>
		[Conditional("DEBUG"), DebuggerHidden]
		private void CheckIsValid()
		{
			Assert.That(IsValid, "The entity is invalid as it has not been created by an entity collection.");
		}

		/// <summary>
		///     Returns a string representation of the entity.
		/// </summary>
		public override string ToString()
		{
			return String.Format("Entity {0}, IsAlive={1}", Identity, IsAlive);
		}
	}
}