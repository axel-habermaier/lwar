namespace Pegasus.Rendering.Particles
{
	using System;
	using System.Runtime.InteropServices;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents a collection of a particle emitter's live particles.
	/// </summary>
	public sealed unsafe class ParticleCollection : DisposableObject
	{
		/// <summary>
		///     The pointer to the memory allocated for the particle collection.
		/// </summary>
		private readonly void* _memory;

		/// <summary>
		///     The size of the allocated memory in bytes.
		/// </summary>
		private readonly int _size;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="capacity">The maximum number of particles that the collection should be able to contain.</param>
		internal ParticleCollection(int capacity)
		{
			Capacity = capacity;

			const int particleSize = sizeof(float) * 3 + // positions
									 sizeof(float) * 3 + // velocities
									 sizeof(byte) * 4 + // colors
									 sizeof(float) + // remaining lifetimes
									 sizeof(float) + // initial lifetimes
									 sizeof(float) + // age
									 sizeof(float); // scales

			_size = particleSize * Capacity;
			_memory = Marshal.AllocHGlobal(_size).ToPointer();

			GC.AddMemoryPressure(_size);

			var pointer = (byte*)_memory;
			Positions = (float*)pointer;
			pointer += sizeof(float) * 3 * Capacity;

			Velocities = (float*)pointer;
			pointer += sizeof(float) * 3 * Capacity;

			Colors = pointer;
			pointer += sizeof(byte) * 4 * Capacity;

			Lifetimes = (float*)pointer;
			pointer += sizeof(float) * Capacity;

			InitialLifetimes = (float*)pointer;
			pointer += sizeof(float) * Capacity;

			Age = (float*)pointer;
			pointer += sizeof(float) * Capacity;

			Scales = (float*)pointer;
		}

		/// <summary>
		///     Gets the maximum number of particles that can be stored in the collection.
		/// </summary>
		internal int Capacity { get; private set; }

		/// <summary>
		///     Stores the age of each particle as a floating-point value in the range [0,1], starting at 1 and decreasing to 0 at the
		///     end of the particle's life.
		/// </summary>
		public float* Age { get; private set; }

		/// <summary>
		///     Stores the scale of each particles as a floating-point value.
		/// </summary>
		public float* Scales { get; private set; }

		/// <summary>
		///     Stores the remaining life time time of each particle as a floating-point value in seconds.
		/// </summary>
		public float* Lifetimes { get; private set; }

		/// <summary>
		///     Stores the initial life time time of each particle as a floating-point value in seconds.
		/// </summary>
		public float* InitialLifetimes { get; private set; }

		/// <summary>
		///     Stores the color of each particle as four-dimensional byte vector. The R, G, B, and A values of the n-th particle's
		///     color are therefore stored at indices (n * 4), (n * 4) + 1, (n * 4) + 2, and (n * 4) + 3.
		/// </summary>
		public byte* Colors { get; private set; }

		/// <summary>
		///     Stores the position of each particle as three-dimensional floating-point vector. The X, Y, and Z values of the n-th
		///     particle's position are therefore stored at indices (n * 3), (n * 3) + 1, and (n * 3) + 2.
		/// </summary>
		public float* Positions { get; private set; }

		/// <summary>
		///     Stores the velocity of each particle as three-dimensional floating-point vector. The X, Y, and Z values of the n-th
		///     particle's velocity are therefore stored at indices (n * 3), (n * 3) + 1, and (n * 3) + 2.
		/// </summary>
		public float* Velocities { get; private set; }

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
			Lifetimes[target] = Lifetimes[source];
			InitialLifetimes[target] = InitialLifetimes[source];
			Scales[target] = Scales[source];

			Colors[(target * 4) + 0] = Colors[(source * 4) + 0];
			Colors[(target * 4) + 1] = Colors[(source * 4) + 1];
			Colors[(target * 4) + 2] = Colors[(source * 4) + 2];
			Colors[(target * 4) + 3] = Colors[(source * 4) + 3];

			Positions[(target * 3) + 0] = Positions[(source * 3) + 0];
			Positions[(target * 3) + 1] = Positions[(source * 3) + 1];
			Positions[(target * 3) + 2] = Positions[(source * 3) + 2];

			Velocities[(target * 3) + 0] = Velocities[(source * 3) + 0];
			Velocities[(target * 3) + 1] = Velocities[(source * 3) + 1];
			Velocities[(target * 3) + 2] = Velocities[(source * 3) + 2];
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Marshal.FreeHGlobal(new IntPtr(_memory));
			GC.RemoveMemoryPressure(_size);
		}
	}
}