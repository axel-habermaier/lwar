namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Components;
	using Pegasus.Entities;

	/// <summary>
	///     Removes entities that have reached the end of their life.
	/// </summary>
	public class TimeToLiveBehavior : EntityBehavior<TimeToLive>
	{
		/// <summary>
		///     The number of seconds that have elapsed since the last update.
		/// </summary>
		private float _elapsedSeconds;

		/// <summary>
		///     Removes all entities that are at the end of their life.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public void RemoveDeadEntities(float elapsedSeconds)
		{
			_elapsedSeconds = elapsedSeconds;
			Process();
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="lifeTimes">The time to live components of the affected entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected override void Process(Entity[] entities, TimeToLive[] lifeTimes, int count)
		{
			for (var i = 0; i < count; ++i)
			{
				lifeTimes[i].RemainingTime -= _elapsedSeconds;

				if (lifeTimes[i].RemainingTime < 0)
					entities[i].Remove();
			}
		}
	}
}