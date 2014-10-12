namespace Pegasus.Rendering.Particles
{
	using System;

	/// <summary>
	///     Represents a collection of a particle emitter's live particles.
	/// </summary>
	public struct ParticleCollection
	{
		/// <summary>
		///     Stores the age of each particle as a floating-point value in the range [0,1], starting at 1 and decreasing to 0 at the
		///     end of the particle's life.
		/// </summary>
		public readonly float[] Age;

		/// <summary>
		///     Stores the remaining life time time of each particle as a floating-point value in seconds.
		/// </summary>
		public readonly float[] Lifetime;

		/// <summary>
		///     The maximum number of particles that can be stored in the collection.
		/// </summary>
		internal readonly int Capacity;

		/// <summary>
		///     Stores the color of each particle as four-dimensional byte vector. The R, G, B, and A values of the n-th particle's
		///     color are therefore stored at indices (n * 4), (n * 4) + 1, (n * 4) + 2, and (n * 4) + 3.
		/// </summary>
		public readonly byte[] Color;

		/// <summary>
		///     Stores the position of each particle as three-dimensional floating-point vector. The X, Y, and Z values of the n-th
		///     particle's position are therefore stored at indices (n * 3), (n * 3) + 1, and (n * 3) + 2.
		/// </summary>
		public readonly float[] Position;

		/// <summary>
		///     Stores the velocity of each particle as three-dimensional floating-point vector. The X, Y, and Z values of the n-th
		///     particle's velocity are therefore stored at indices (n * 3), (n * 3) + 1, and (n * 3) + 2.
		/// </summary>
		public readonly float[] Velocity;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="capacity">The maximum number of particles that the collection should be able to contain.</param>
		internal ParticleCollection(int capacity)
		{
			Capacity = capacity;
			Color = new byte[capacity * 4];
			Age = new float[capacity];
			Lifetime = new float[capacity];
			Position = new float[capacity * 3];
			Velocity = new float[capacity * 3];
		}

		/// <summary>
		///     Copies the particle at the source index to the target index.
		/// </summary>
		/// <param name="source">The index of the source particle.</param>
		/// <param name="target">The index of the target particle.</param>
		public void Copy(int source, int target)
		{
			Assert.InRange(source, 0, Capacity - 1);
			Assert.InRange(target, 0, Capacity - 1);

			if (source == target)
				return;

			Age[target] = Age[source];
			Lifetime[target] = Lifetime[source];

			Color[(target * 4) + 0] = Color[(source * 4) + 0];
			Color[(target * 4) + 1] = Color[(source * 4) + 1];
			Color[(target * 4) + 2] = Color[(source * 4) + 2];
			Color[(target * 4) + 3] = Color[(source * 4) + 3];

			Position[(target * 3) + 0] = Position[(source * 3) + 0];
			Position[(target * 3) + 1] = Position[(source * 3) + 1];
			Position[(target * 3) + 2] = Position[(source * 3) + 2];

			Velocity[(target * 3) + 0] = Velocity[(source * 3) + 0];
			Velocity[(target * 3) + 1] = Velocity[(source * 3) + 1];
			Velocity[(target * 3) + 2] = Velocity[(source * 3) + 2];
		}

		/// <summary>
		///     Copies this particle collection to the given one. If this collection contains more particles than the given collection,
		///     the particles that do not fit are not copied. If this collection contains fewer particles than the given collection, the
		///     remaining particles are not modified.
		/// </summary>
		/// <param name="collection"></param>
		public void CopyTo(ref ParticleCollection collection)
		{
			var capacity = Math.Min(Capacity, collection.Capacity);
			if (capacity == 0)
				return;

			Array.Copy(Age, collection.Age, capacity);
			Array.Copy(Lifetime, collection.Lifetime, capacity);
			Array.Copy(Color, collection.Color, capacity * 4);
			Array.Copy(Position, collection.Position, capacity * 3);
			Array.Copy(Velocity, collection.Velocity, capacity * 3);
		}
	}
}