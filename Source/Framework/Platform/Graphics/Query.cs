using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a query that can be used to retrieve information from the GPU.
	/// </summary>
	public abstract class Query : GraphicsObject
	{
		/// <summary>
		///   The native query instance.
		/// </summary>
		private readonly IntPtr _query;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="type">The type of the query.</param>
		protected Query(GraphicsDevice graphicsDevice, QueryType type)
			: base(graphicsDevice)
		{
			Assert.ArgumentInRange(type, () => type);
			_query = NativeMethods.CreateQuery(graphicsDevice.NativePtr, type);
		}

		/// <summary>
		///   Gets a value indicating whether the query data is available.
		/// </summary>
		public bool DataAvailable
		{
			get { return NativeMethods.IsQueryDataAvailable(_query); }
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyQuery(_query);
		}

		/// <summary>
		///   Begins the query.
		/// </summary>
		protected void BeginQuery()
		{
			NativeMethods.BeginQuery(_query);
		}

		/// <summary>
		///   Ends the query.
		/// </summary>
		protected void EndQuery()
		{
			NativeMethods.EndQuery(_query);
		}

		/// <summary>
		///   Gets the result of the query.
		/// </summary>
		/// <param name="data">The address of the memory the result should be written to.</param>
		/// <param name="size">The size of the data that should be retrieved.</param>
		protected unsafe void GetQueryData(void* data, int size)
		{
			NativeMethods.GetQueryData(_query, data, size);
		}

		/// <summary>
		///   Provides access to the native query functions.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateQuery")]
			public static extern IntPtr CreateQuery(IntPtr device, QueryType type);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyQuery")]
			public static extern void DestroyQuery(IntPtr query);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBeginQuery")]
			public static extern void BeginQuery(IntPtr query);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgEndQuery")]
			public static extern void EndQuery(IntPtr query);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetQueryData")]
			public static extern unsafe void GetQueryData(IntPtr query, void* data, int size);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgIsQueryDataAvailable")]
			public static extern bool IsQueryDataAvailable(IntPtr query);
		}
	}
}