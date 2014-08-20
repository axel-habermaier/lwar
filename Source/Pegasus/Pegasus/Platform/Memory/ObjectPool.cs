﻿namespace Pegasus.Platform.Memory
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	///     A base class for object pools.
	/// </summary>
	public abstract class ObjectPool : DisposableObject
	{
		/// <summary>
		///     The object pools with global lifetime that should be disposed automatically during application shutdown.
		/// </summary>
		private static readonly List<DisposableObject> GlobalPools = new List<DisposableObject>();

		/// <summary>
		///     Adds the given pool to the list of global pools that are disposed automatically during application shutdown.
		/// </summary>
		/// <param name="objectPool">The object pool that should be added.</param>
		protected void AddGlobalPool(ObjectPool objectPool)
		{
			Assert.ArgumentNotNull(objectPool);
			Assert.That(!GlobalPools.Contains(objectPool), "The object pool has already been added.");

			GlobalPools.Add(objectPool);
		}

		/// <summary>
		///     Disposes all pools with global lifetime.
		/// </summary>
		internal static void DisposeGlobalPools()
		{
			GlobalPools.SafeDisposeAll();
		}

		/// <summary>
		///     Returns an object to the pool so that it can be reused later.
		/// </summary>
		/// <param name="obj">The object that should be returned to the pool.</param>
		public abstract void Free(object obj);
	}
}