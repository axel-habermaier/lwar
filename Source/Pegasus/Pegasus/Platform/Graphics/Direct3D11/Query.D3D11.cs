namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using Bindings;
	using Interface;
	using Logging;
	using Utilities;

	/// <summary>
	///     Represents an Direct3D11-based query.
	/// </summary>
	internal unsafe class QueryD3D11 : GraphicsObjectD3D11, IQuery
	{
		/// <summary>
		///     The type of the query.
		/// </summary>
		private readonly QueryType _type;

		/// <summary>
		///     The underlying Direct3D11 query.
		/// </summary>
		private D3D11Query _query;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="type">The type of the query.</param>
		public QueryD3D11(GraphicsDeviceD3D11 graphicsDevice, QueryType type)
			: base(graphicsDevice)
		{
			var desc = new D3D11QueryDescription { Flags = D3D11QueryFlags.None };

			switch (type)
			{
				case QueryType.Occlusion:
					desc.Type = D3D11QueryType.Occlusion;
					break;
				case QueryType.Synced:
					desc.Type = D3D11QueryType.Event;
					break;
				case QueryType.TimestampDisjoint:
					desc.Type = D3D11QueryType.TimestampDisjoint;
					break;
				case QueryType.Timestamp:
					desc.Type = D3D11QueryType.Timestamp;
					break;
				default:
					throw new ArgumentOutOfRangeException("type");
			}

			_type = type;
			Device.CreateQuery(ref desc, out _query).CheckSuccess("Failed to create query.");
		}

		/// <summary>
		///     Gets a value indicating whether the query has completed and whether the result data (if any) is available.
		/// </summary>
		public bool Completed
		{
			get
			{
				var result = Context.GetData(_query, null, 0, 0);
				if (!result.IsOk && !result.IsFalse)
					Log.Die("{0}", result.GetErrorMessage("Failed to check availability of query result."));

				return result.IsOk;
			}
		}

		/// <summary>
		///     Begins the query.
		/// </summary>
		public void Begin()
		{
			Context.Begin(_query);
		}

		/// <summary>
		///     Ends the query.
		/// </summary>
		public void End()
		{
			Context.End(_query);
		}

		/// <summary>
		///     Gets the result of the query.
		/// </summary>
		/// <param name="data">The address of the memory the result should be written to.</param>
		public void GetResult(void* data)
		{
			var available = new D3D11Result();

			switch (_type)
			{
				case QueryType.Synced:
					Log.Die("Not supported.");
					break;
				case QueryType.Timestamp:
					available = Context.GetData(_query, data, sizeof(ulong), D3D11QueryFlags.None);
					break;
				case QueryType.TimestampDisjoint:
					available = Context.GetData(_query, data, sizeof(TimestampDisjointQuery.Result), D3D11QueryFlags.None);
					break;
				case QueryType.Occlusion:
					Log.Die("Not implemented.");
					break;
				default:
					throw new InvalidOperationException("Unsupported query type.");
			}

			Assert.That(available.IsOk, "Tried to get data from a query that has not yet completed.");
		}

		/// <summary>
		///     Sets the debug name of the query.
		/// </summary>
		/// <param name="name">The debug name of the query.</param>
		public void SetName(string name)
		{
			_query.SetDebugName(name);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_query.Release();
		}
	}
}