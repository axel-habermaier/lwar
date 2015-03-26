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

		/// <summary>
		///     Allows the caller to acquire shared ownership of the object. The object will not be returned to the pool before the
		///     caller called its Dispose method.
		/// </summary>
		/// <remarks>Unless, of course, some malicious caller invokes Dispose multiple times...</remarks>
		IDisposable AcquireOwnership();
	}
}