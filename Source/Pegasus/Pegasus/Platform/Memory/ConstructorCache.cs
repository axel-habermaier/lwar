namespace Pegasus.Platform.Memory
{
	using System;
	using Utilities;

	/// <summary>
	///     Caches constructor delegates.
	/// </summary>
	public static class ConstructorCache
	{
		/// <summary>
		///     Gets a value indicating whether a constructor has already been cached.
		/// </summary>
		public static bool IsCached<T>()
			where T : class
		{
			return Cache<T>.Constructor != null;
		}

		/// <summary>
		///     Registers a constructor for the given type.
		/// </summary>
		/// <typeparam name="T">The type the constructor should be cached for.</typeparam>
		/// <param name="constructor">The constructor that should be registered.</param>
		public static void Set<T>(Func<T> constructor)
			where T : class
		{
			Assert.ArgumentNotNull(constructor);
			Assert.That(!IsCached<T>(), "A constructor has already been registered for this type.");

			Cache<T>.Constructor = constructor;
		}

		/// <summary>
		///     Gets the registered constructor for the given type, or null if no constructor has been registered.
		/// </summary>
		/// <typeparam name="T">The type the constructor should be returned for.</typeparam>
		public static Func<T> Get<T>()
			where T : class
		{
			return Cache<T>.Constructor;
		}

		/// <summary>
		///     Caches a constructor delegate for the given type.
		/// </summary>
		/// <typeparam name="T">The type that is constructed.</typeparam>
		private static class Cache<T>
			where T : class
		{
			/// <summary>
			///     The constructor for the given type.
			/// </summary>
			public static Func<T> Constructor;
		}
	}
}