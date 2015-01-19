namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Bindings;
	using Interface;
	using Utilities;

	/// <summary>
	///     Represents an OpenGL3-based query that checks whether the GPU has reached a CPU/GPU synchronization marker.
	/// </summary>
	internal unsafe class SyncedQueryGL3 : GraphicsObjectGL3, IQuery
	{
		/// <summary>
		///     The sync query handle.
		/// </summary>
		private void* _sync;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		public SyncedQueryGL3(GraphicsDeviceGL3 graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///     Gets a value indicating whether the query has completed and whether the result data (if any) is available.
		/// </summary>
		public bool Completed
		{
			get
			{
				Assert.That(_sync != null, "Sync failed or no sync point has been marked.");

				var result = _gl.ClientWaitSync(_sync, GL.SyncFlushCommandsBit, 0);
				return result == GL.ConditionSatisfied || result == GL.AlreadySignaled;
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
			_sync = _gl.FenceSync(GL.SyncGpuCommandsComplete, 0);
		}

		/// <summary>
		///     Gets the result of the query.
		/// </summary>
		/// <param name="data">The address of the memory the result should be written to.</param>
		public void GetResult(void* data)
		{
			throw new NotImplementedException();
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
			_gl.DeleteSync(_sync);
		}

		/// <summary>
		///     Gets the result of the query.
		/// </summary>
		/// <param name="data">The address of the memory the result should be written to.</param>
		protected void GetQueryData(void* data)
		{
			throw new NotSupportedException();
		}
	}
}