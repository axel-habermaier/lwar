using System;

namespace Pegasus.Framework
{
	using System.Collections.Generic;

	/// <summary>
	///   Pools objects of type T in order to reduce the pressure on the garbage collector. Instead of new'ing up a new object
	///   of type T whenever one is needed, the pool's Get() method should be used to retrieve a previously allocated instance.
	///   Once the object is no longer being used, it must be returned to the pool so that it can be reused later on. If the
	///   pool runs out of instances, it batch-creates several new ones.
	/// </summary>
	/// <typeparam name="T">The type of the pooled objects.</typeparam>
	public class ObjectPool<T> : Pool<T>
		where T : class, IDisposable, new()
	{
		/// <summary>
		///   Allocates new objects.
		/// </summary>
		/// <param name="items">The list in which the newly allocated items should be stored.</param>
		/// <param name="count">The number of items that should be allocated.</param>
		protected override void AllocateObjects(List<T> items, int count)
		{
			for (var i = 0; i < count; ++i)
				items.Add(new T());
		}
	}
}