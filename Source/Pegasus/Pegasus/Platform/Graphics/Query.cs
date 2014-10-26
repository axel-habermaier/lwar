namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Security;
	using Utilities;

	/// <summary>
	///     Represents a query that can be used to retrieve information from the GPU.
	/// </summary>
	public abstract class Query : GraphicsObject
	{
		/// <summary>
		///     The native query instance.
		/// </summary>
		private readonly IntPtr _query;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="type">The type of the query.</param>
		protected Query(GraphicsDevice graphicsDevice, QueryType type)
			: base(graphicsDevice)
		{
			Assert.ArgumentInRange(type);
			_query = NativeMethods.CreateQuery(graphicsDevice.NativePtr, type);
		}

		/// <summary>
		///     Gets a value indicating whether the query has completed and whether the result data (if any) is available.
		/// </summary>
		public bool Completed
		{
			get { return NativeMethods.IsQueryDataAvailable(_query); }
		}

		/// <summary>
		///     Waits for the completion of the query by stalling the CPU until the query has completed and the result data (if any)
		///     is available.
		/// </summary>
		public void WaitForCompletion()
		{
			while (!Completed)
			{
				// Just check the query's completion status until it has been completed
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyQuery(_query);
		}

		/// <summary>
		///     Begins the query.
		/// </summary>
		protected void BeginQuery()
		{
			NativeMethods.BeginQuery(_query);
		}

		/// <summary>
		///     Ends the query.
		/// </summary>
		protected void EndQuery()
		{
			NativeMethods.EndQuery(_query);
		}

		/// <summary>
		///     Gets the result of the query.
		/// </summary>
		/// <param name="data">The address of the memory the result should be written to.</param>
		/// <param name="size">The size of the data that should be retrieved.</param>
		protected unsafe void GetQueryData(void* data, int size)
		{
			NativeMethods.GetQueryData(_query, data, size);
		}

#if DEBUG
		/// <summary>
		///   Invoked after the name of the graphics object has changed. This method is only available in debug builds.
		/// </summary>
		protected override void OnRenamed()
		{
			if (_query != IntPtr.Zero)
				NativeMethods.SetName(_query, Name);
		}
#endif

		/// <summary>
		///     Provides access to the native query functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
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

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetQueryName")]
			[Conditional("DEBUG")]
			public static extern void SetName(IntPtr query, string name);
		}
	}
}