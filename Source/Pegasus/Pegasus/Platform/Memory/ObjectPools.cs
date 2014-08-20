namespace Pegasus.Platform.Memory
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	///     Provides access to some default object pools.
	/// </summary>
	public static class ObjectPools
	{
		/// <summary>
		///     The default pool for string builder instances.
		/// </summary>
		public static readonly ObjectPool<StringBuilder> StringBuilders =
			new ObjectPool<StringBuilder>(() => new StringBuilder(), s => { }, s => s.Clear(), hasGlobalLifetime: true);

		/// <summary>
		///     The default pool for buffer reader instances.
		/// </summary>
		public static readonly ObjectPool<BufferReader> BufferReaders =
			new ObjectPool<BufferReader>(() => new BufferReader(), b => { }, b => b.Free(), hasGlobalLifetime: true);

		/// <summary>
		///     The default pool for buffer writer instances.
		/// </summary>
		public static readonly ObjectPool<BufferWriter> BufferWriters =
			new ObjectPool<BufferWriter>(() => new BufferWriter(), b => { }, b => b.Free(), hasGlobalLifetime: true);

		/// <summary>
		///     The object pools with global lifetime that should be disposed automatically during application shutdown.
		/// </summary>
		private static List<DisposableObject> _globalPools;

		/// <summary>
		///     Adds the given pool to the list of global pools that are disposed automatically during application shutdown.
		/// </summary>
		/// <typeparam name="T">The type of the pooled objects.</typeparam>
		/// <param name="objectPool">The object pool that should be added.</param>
		internal static void AddGlobalPool<T>(ObjectPool<T> objectPool)
			where T : class
		{
			// Lazy initialization of the global pools list to avoid problems with the order of static member initialization.
			if (_globalPools == null)
				_globalPools = new List<DisposableObject>();

			Assert.ArgumentNotNull(objectPool);
			Assert.That(!_globalPools.Contains(objectPool), "The object pool has already been added.");

			_globalPools.Add(objectPool);
		}

		/// <summary>
		///     Disposes all default pools.
		/// </summary>
		internal static void Dispose()
		{
			_globalPools.SafeDisposeAll();
		}
	}
}