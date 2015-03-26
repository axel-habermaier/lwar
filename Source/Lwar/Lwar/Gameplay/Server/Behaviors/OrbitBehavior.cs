namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Scene;
	using Pegasus.Utilities;

	/// <summary>
	///     Makes the scene node move in an orbit around its parent.
	/// </summary>
	internal class OrbitBehavior : Behavior<SceneNode>
	{
		/// <summary>
		///     An offset to the current position on the orbital trajectory.
		/// </summary>
		private float _orbitOffset;

		/// <summary>
		///     The radius of the orbit.
		/// </summary>
		private float _orbitRadius;

		/// <summary>
		///     The speed of the movement. The sign of the speed determines the direction of the orbital movement.
		/// </summary>
		private float _orbitSpeed;

		/// <summary>
		///     The number of seconds that have elapsed since the first update.
		/// </summary>
		private double _totalSeconds;

		/// <summary>
		///     Invoked when the behavior should execute a step.
		/// </summary>
		/// <param name="elapsedSeconds">The elapsed time in seconds since the last execution of the behavior.</param>
		public override void Execute(float elapsedSeconds)
		{
			_totalSeconds += elapsedSeconds;
			var time = _totalSeconds * _orbitSpeed + _orbitOffset;

			var x = MathUtils.Sin(time);
			var y = MathUtils.Cos(time);

			SceneNode.Position = new Vector3(x, 0, y) * _orbitRadius;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate pool objects.</param>
		/// <param name="orbitRadius">The radius of the planet's orbit.</param>
		/// <param name="orbitSpeed">
		///     The orbital speed of the planet. The sign of the speed determines the direction of the orbital movement.
		/// </param>
		/// <param name="orbitOffset">An offset to the position on the orbital trajectory.</param>
		public static OrbitBehavior Create(PoolAllocator allocator, float orbitRadius, float orbitSpeed, float orbitOffset)
		{
			Assert.ArgumentNotNull(allocator);

			var behavior = allocator.Allocate<OrbitBehavior>();
			behavior._orbitOffset = orbitOffset;
			behavior._orbitSpeed = orbitSpeed;
			behavior._orbitRadius = orbitRadius;
			behavior._totalSeconds = 0;

			return behavior;
		}
	}
}