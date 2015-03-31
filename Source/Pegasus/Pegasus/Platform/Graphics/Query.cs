namespace Pegasus.Platform.Graphics
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents a query that can be used to retrieve information from the GPU.
	/// </summary>
	public abstract unsafe class Query : GraphicsObject
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
			NativeObject = DeviceInterface->InitializeQuery((int)type);
		}

		/// <summary>
		///     Gets a value indicating whether the query has completed and whether the result data (if any) is available.
		/// </summary>
		public bool Completed
		{
			get
			{
				Assert.NotDisposed(this);
				return DeviceInterface->IsQueryCompleted(NativeObject);
			}
		}

		/// <summary>
		///     Gets the function that should be used to set the debug name of the native object.
		/// </summary>
		protected override SetNameDelegate SetNameFunction
		{
			get { return DeviceInterface->SetQueryName; }
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceInterface->FreeQuery(NativeObject);
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