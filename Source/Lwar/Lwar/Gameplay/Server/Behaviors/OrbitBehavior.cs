namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Components;
	using Pegasus.Entities;
	using Pegasus.Math;

	/// <summary>
	///     Updates the positions of entities that orbit around another entity.
	/// </summary>
	public class OrbitBehavior : EntityBehavior<Orbit, RelativeTransform>
	{
		/// <summary>
		///     The number of seconds that have elapsed since the first update.
		/// </summary>
		private double _totalSeconds;

		/// <summary>
		///     Removes all entities that crossed the boundaries of the galaxy.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public void UpdateOrbits(float elapsedSeconds)
		{
			_totalSeconds += elapsedSeconds;
			Process();
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="orbits">The orbit components of the affected entities.</param>
		/// <param name="transforms">The relative transform components of the affected entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected override void Process(Entity[] entities, Orbit[] orbits, RelativeTransform[] transforms, int count)
		{
			for (var i = 0; i < count; ++i)
			{
				var time = _totalSeconds * orbits[i].Speed + orbits[i].Offset;
				var x = Math.Sin(time);
				var y = Math.Cos(time);
				transforms[i].Position = new Vector2((float)x, (float)y) * orbits[i].Radius;
			}
		}
	}
}