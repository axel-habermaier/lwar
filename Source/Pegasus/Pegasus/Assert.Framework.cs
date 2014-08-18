namespace Pegasus
{
	using System;
	using System.Diagnostics;
	using Framework;
	using Platform.Memory;

	/// <summary>
	///     Defines assertion helpers that can be used to check for errors. The checks are only performed in debug builds.
	/// </summary>
	public static partial class Assert
	{
		/// <summary>
		///     Throws a PegasusException if the given object has already been sealed.
		/// </summary>
		/// <param name="obj">The object that should be checked.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void NotSealed(ISealable obj)
		{
			ArgumentNotNull(obj);

			if (obj.IsSealed)
				throw new PegasusException("The '{0}' instance has already been sealed.", obj.GetType().FullName);
		}

		/// <summary>
		///     Throws a PegasusException if the given object is not null or has not been disposed.
		/// </summary>
		/// <param name="obj">The object that should be checked.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void NullOrDisposed<T>(T obj)
			where T : DisposableObject
		{
			if (obj != null && !obj.IsDisposed)
				throw new PegasusException("The '{0}' instance has not been disposed.", typeof(T).FullName);
		}

		/// <summary>
		///     Throws a PegasusException if the given object has already been disposed.
		/// </summary>
		/// <param name="obj">The object that should be checked.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void NotDisposed(DisposableObject obj)
		{
			ArgumentNotNull(obj);

			if (obj.IsDisposed)
				throw new PegasusException(obj.GetType().FullName);
		}

		/// <summary>
		///     Throws a PegasusException if the given object has already been returned to the pool.
		/// </summary>
		/// <param name="obj">The object that should be checked.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void NotPooled<T>(T obj)
			where T : PooledObject<T>, new()
		{
			ArgumentNotNull(obj);

			if (obj.IsAvailable)
				throw new PegasusException(obj.GetType().FullName);
		}
	}
}