namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Components;
	using Pegasus.Entities;

	/// <summary>
	///     Handles relative transforms.
	/// </summary>
	public class RelativeTransformBehavior : EntityBehavior<RelativeTransform, Transform, Transform>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public RelativeTransformBehavior()
			: base(ComponentDependency.Default, ComponentDependency.Default, ComponentDependency.Required)
		{
		}

		/// <summary>
		///     Gets the components for the given entity. By default, the components are retrieved from the given entity itself. On the
		///     other hand, overriding methods could retrieve components from related entities, if they so desire.
		/// </summary>
		/// <param name="entity">The entity the components should be retrieved for.</param>
		/// <param name="relativeTransform">The relative transform component of the entity.</param>
		/// <param name="transform">The transform component of the entity.</param>
		/// <param name="parentTransform">The transform component of the parent entity.</param>
		protected override void GetComponents(Entity entity, out RelativeTransform relativeTransform, out Transform transform,
											  out Transform parentTransform)
		{
			relativeTransform = entity.GetComponent<RelativeTransform>();
			transform = entity.GetComponent<Transform>();
			parentTransform = relativeTransform != null ? relativeTransform.ParentEntity.GetComponent<Transform>() : null;
		}

		/// <summary>
		///     Updates the positions and velocities.
		/// </summary>
		public void Update()
		{
			Process();
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="relativeTransforms">The relative transform components of the affected entities.</param>
		/// <param name="transforms">The transform components of the affected entities.</param>
		/// <param name="parentTransforms">The transform components of the parent entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected override void Process(Entity[] entities, RelativeTransform[] relativeTransforms, Transform[] transforms,
										Transform[] parentTransforms, int count)
		{
			for (var i = 0; i < count; ++i)
			{
				if (relativeTransforms[i].ParentEntity.IsDead)
					entities[i].Remove();
				else
				{
					transforms[i].Position = relativeTransforms[i].Position + parentTransforms[i].Position;
					transforms[i].Orientation = relativeTransforms[i].Orientation + parentTransforms[i].Orientation;
				}
			}
		}
	}
}