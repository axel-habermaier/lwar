namespace Pegasus.Platform.Graphics
{
	using System;
	using Interface;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a query that can be used to retrieve information from the GPU.
	/// </summary>
	public abstract class Query : GraphicsObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="type">The type of the query.</param>
		protected Query(GraphicsDevice graphicsDevice, QueryType type)
			: base(graphicsDevice)
		{
			Assert.ArgumentInRange(type);
			QueryObject = graphicsDevice.CreateQuery(type);
		}

		/// <summary>
		///     Gets the underlying query object.
		/// </summary>
		internal IQuery QueryObject { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the query has completed and whether the result data (if any) is available.
		/// </summary>
		public bool Completed
		{
			get
			{
				Assert.NotDisposed(this);
				return QueryObject.Completed;
			}
		}

		/// <summary>
		///     Invoked after the name of the graphics object has changed. This method is only invoked in debug builds.
		/// </summary>
		/// <param name="name">The new name of the graphics object.</param>
		protected override void OnRenamed(string name)
		{
			QueryObject.SetName(name);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			QueryObject.SafeDispose();
		}

		/// <summary>
		///     Waits for the completion of the query by stalling the CPU until the query has completed and the result data (if any)
		///     is available.
		/// </summary>
		public void WaitForCompletion()
		{
			Assert.NotDisposed(this);
			while (!Completed)
			{
				// Just check the query's completion status until it has been completed
			}
		}
	}
}