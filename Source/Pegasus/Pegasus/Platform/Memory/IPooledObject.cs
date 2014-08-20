namespace Pegasus.Platform.Memory
{
	using System;

	/// <summary>
	///     Represents an object that is allocated from an object pool.
	/// </summary>
	public interface IPooledObject
	{
		/// <summary>
		///     Gets a value indicating whether the instance is currently available, that is, waiting in the pool to be reused.
		/// </summary>
		bool IsAvailable { get; }

		/// <summary>
		///     Marks the instance as allocated from the given pool.
		/// </summary>
		/// <param name="objectPool">The object pool the instance is allocated from.</param>
		void AllocatedFrom(ObjectPool objectPool);
	}
}