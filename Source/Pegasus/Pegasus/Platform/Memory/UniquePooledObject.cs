namespace Pegasus.Platform.Memory
{
	using System;

	/// <summary>
	///     An abstract base class for objects whose instances are pooled with non-shared ownership semantics.
	/// </summary>
	public abstract class UniquePooledObject : PooledObject
	{
		/// <summary>
		///     Invoked when an owner of the pooled object release its ownership. Returns true to indicate that
		///     the object should be returned to the pool.
		/// </summary>
		protected sealed override bool OnOwnershipReleased()
		{
			return true;
		}
	}
}