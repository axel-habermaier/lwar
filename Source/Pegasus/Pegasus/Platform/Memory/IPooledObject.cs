namespace Pegasus.Platform.Memory
{
	using System;

	/// <summary>
	///     Represents an object that is allocated from an object pool.
	/// </summary>
	public interface IPooledObject : IDisposable
	{
		/// <summary>
		///     Gets a value indicating whether the instance is currently available, that is, waiting in the pool to be reused.
		/// </summary>
		bool InUse { get; }
	}
}