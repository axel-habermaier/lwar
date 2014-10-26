namespace Pegasus.Platform.Memory
{
	using System;
	using System.Text;
	using Utilities;

	/// <summary>
	///     Provides access to an application-global pool of string builder instances.
	/// </summary>
	public static class StringBuilderPool
	{
		/// <summary>
		///     The default pool for string builder instances.
		/// </summary>
		private static readonly ObjectPool<StringBuilder> StringBuilders =
			new ObjectPool<StringBuilder>(() => new StringBuilder(), hasGlobalLifetime: true);

		/// <summary>
		///     Allocates a string builder instance from a application-wide pool.
		/// </summary>
		public static ScopedLifetime Allocate(out StringBuilder stringBuilder)
		{
			stringBuilder = StringBuilders.Allocate();
			return new ScopedLifetime(stringBuilder);
		}

		/// <summary>
		///     Returns the given string builder to the application-global pool.
		/// </summary>
		/// <param name="stringBuilder">The string builder that should be returned to the pool.</param>
		public static void Free(StringBuilder stringBuilder)
		{
			Assert.ArgumentNotNull(stringBuilder);

			stringBuilder.Clear();
			StringBuilders.Free(stringBuilder);
		}

		/// <summary>
		///     Holds a reference to a pooled string builder instance that will be returned to the application-global default pool once
		///     the Dispose method is called.
		/// </summary>
		public struct ScopedLifetime : IDisposable
		{
			/// <summary>
			///     The pooled string builder instance that should be returned to the pool.
			/// </summary>
			private readonly StringBuilder _stringBuilder;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="stringBuilder">The pooled string builder instance that should be returned to the pool.</param>
			public ScopedLifetime(StringBuilder stringBuilder)
				: this()
			{
				Assert.ArgumentNotNull(stringBuilder);
				_stringBuilder = stringBuilder;
			}

			/// <summary>
			///     Disposes the object, releasing all managed and unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				Free(_stringBuilder);
			}
		}
	}
}