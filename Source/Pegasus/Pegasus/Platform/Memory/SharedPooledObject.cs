namespace Pegasus.Platform.Memory
{
	using System;
	using Utilities;

	/// <summary>
	///     An abstract base class for reference counted pooled objects that have shared ownership semantics. All owners have to
	///     invoke the AcquireOwnership method when they start using the object, which increases the reference count of the object.
	///     The object is only returned to the pool when AcquireOwnership and Dispose have been called equally often.
	/// </summary>
	public abstract class SharedPooledObject : PooledObject
	{
		/// <summary>
		///     The number of times Dispose must be called before the object is returned to the pool.
		/// </summary>
		private int _referenceCount;

		/// <summary>
		///     Invoked when an owner of the pooled object release its ownership. Returns true to indicate that
		///     the object should be returned to the pool.
		/// </summary>
		protected override sealed bool OnOwnershipReleased()
		{
			Assert.That(_referenceCount > 0, "Ownership is released without having been acquired before.");

			_referenceCount = Math.Max(_referenceCount - 1, 0);
			return _referenceCount <= 0;
		}

		/// <summary>
		///     Allows the caller to acquire shared ownership of the object. The object will not be returned to the pool before the
		///     caller called its Dispose method.
		/// </summary>
		/// <remarks>Unless, of course, some malicious caller invokes Dispose multiple times...</remarks>
		public IDisposable AcquireOwnership()
		{
			++_referenceCount;
			return this;
		}
	}
}