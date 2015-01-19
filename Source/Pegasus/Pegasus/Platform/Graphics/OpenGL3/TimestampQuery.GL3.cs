namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Represents an OpenGL3-based query that records the current GPU timestamp. A timestamp query can only be issued when a
	///     timestamp disjoint query is currently active.
	/// </summary>
	internal unsafe class TimestampQueryGL3 : GraphicsObjectGL3, IQuery
	{
		/// <summary>
		///     The query handle.
		/// </summary>
		private readonly uint _handle;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		public TimestampQueryGL3(GraphicsDeviceGL3 graphicsDevice)
			: base(graphicsDevice)
		{
			_handle = GLContext.Allocate(_gl.GenQueries, "TimestampQuery");
		}

		/// <summary>
		///     Gets a value indicating whether the query has completed and whether the result data (if any) is available.
		/// </summary>
		public bool Completed
		{
			get
			{
				int available;
				_gl.GetQueryObjectiv(_handle, GL.QueryResultAvailable, &available);

				return available == GL.True;
			}
		}

		/// <summary>
		///     Begins the query.
		/// </summary>
		public void Begin()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Ends the query.
		/// </summary>
		public void End()
		{
			_gl.QueryCounter(_handle, GL.Timestamp);
		}

		/// <summary>
		///     Gets the result of the query.
		/// </summary>
		/// <param name="data">The address of the memory the result should be written to.</param>
		public void GetResult(void* data)
		{
			ulong timestamp;
			_gl.GetQueryObjectui64v(_handle, GL.QueryResult, &timestamp);
			*(ulong*)data = timestamp;
		}

		/// <summary>
		///     Sets the debug name of the query.
		/// </summary>
		/// <param name="name">The debug name of the query.</param>
		public void SetName(string name)
		{
			// Not supported by OpenGL
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			GLContext.Deallocate(_gl.DeleteQueries, _handle);
		}
	}
}