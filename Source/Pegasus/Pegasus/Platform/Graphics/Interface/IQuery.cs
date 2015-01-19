namespace Pegasus.Platform.Graphics.Interface
{
	using System;

	/// <summary>
	///     Represents a query that can be used to retrieve information from the GPU.
	/// </summary>
	internal interface IQuery : IDisposable
	{
		/// <summary>
		///     Gets a value indicating whether the query has completed and whether the result data (if any) is available.
		/// </summary>
		bool Completed { get; }

		/// <summary>
		///     Begins the query.
		/// </summary>
		void Begin();

		/// <summary>
		///     Ends the query.
		/// </summary>
		void End();

		/// <summary>
		///     Gets the result of the query.
		/// </summary>
		/// <param name="data">The address of the memory the result should be written to.</param>
		unsafe void GetResult(void* data);

		/// <summary>
		///     Sets the debug name of the query.
		/// </summary>
		/// <param name="name">The debug name of the query.</param>
		void SetName(string name);
	}
}