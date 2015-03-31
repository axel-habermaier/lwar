namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Interface;

	/// <summary>
	///     Represents an OpenGL3-based query that records the frequency of the GPU timer that can be used to interpret the result
	///     of timestamp queries. It also indicates whether a GPU event invalidates all timestamp results.
	/// </summary>
	internal class TimestampDisjointQueryGL3 : GraphicsObjectGL3, IQuery
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		public TimestampDisjointQueryGL3(GraphicsDeviceGL3 graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///     Gets a value indicating whether the query has completed and whether the result data (if any) is available.
		/// </summary>
		public bool Completed
		{
			get { return true; }
		}

		/// <summary>
		///     Begins the query.
		/// </summary>
		public void Begin()
		{
			// Not required by OpenGL
		}

		/// <summary>
		///     Ends the query.
		/// </summary>
		public void End()
		{
			// Not required by OpenGL
		}

		/// <summary>
		///     Gets the result of the query.
		/// </summary>
		/// <param name="data">The address of the memory the result should be written to.</param>
		public unsafe void GetResult(void* data)
		{
			var typedData = (TimestampDisjointQueryResult*)data;
			typedData->Frequency = 1000000000;
			typedData->Disjoint = false;
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
			// Nothing to do here
		}
	}
}