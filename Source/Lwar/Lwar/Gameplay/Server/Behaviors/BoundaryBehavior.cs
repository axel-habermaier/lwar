namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Components;
	using Pegasus.Entities;

	/// <summary>
	///     Removes entities that reach the boundaries of the galaxy.
	/// </summary>
	public class BoundaryBehavior : EntityBehavior<Transform>
	{
		/// <summary>
		///     Removes all entities that crossed the boundaries of the galaxy.
		/// </summary>
		public void RemoveEntitiesWithInvalidPositions()
		{
			Process();
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="transforms">The transform components of the affected entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected override void Process(Entity[] entities, Transform[] transforms, int count)
		{
			for (var i = 0; i < count; ++i)
			{
				if (transforms[i].Position.LengthSquared > Int16.MaxValue * Int16.MaxValue)
					entities[i].Remove();
			}
		}
	}
}