namespace Pegasus.Platform.Memory
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	///   Pools arrays of type T and a given length in order to reduce the pressure on the garbage collector. Instead of
	///   new'ing up a new array of type T whenever one is needed, the pool's Get() method should be used to retrieve a
	///   previously allocated instance. Once the array is no longer being used, it should be returned to the pool so that
	///   it can be reused later on. If the pool runs out of instances, it batch-creates several new ones.
	/// </summary>
	/// <typeparam name="T">The type of the pooled arrays.</typeparam>
	public class ArrayPool<T> : Pool<T[]>
		where T : new()
	{
		/// <summary>
		///   The size of the pooled arrays.
		/// </summary>
		private readonly int _size;

		/// <summary>
		///   Initializes the type.
		/// </summary>
		/// <param name="size">The size of the pooled arrays.</param>
		public ArrayPool(int size)
		{
			Assert.ArgumentInRange(size, 1, Int32.MaxValue);
			_size = size;
		}

		/// <summary>
		///   Allocates new objects.
		/// </summary>
		/// <param name="items">The list in which the newly allocated items should be stored.</param>
		/// <param name="count">The number of items that should be allocated.</param>
		protected override void AllocateObjects(List<T[]> items, int count)
		{
			for (var i = 0; i < count; ++i)
			{
				var array = new T[_size];
				for (var j = 0; j < _size; ++j)
					array[j] = new T();

				items.Add(array);
			}
		}
	}
}