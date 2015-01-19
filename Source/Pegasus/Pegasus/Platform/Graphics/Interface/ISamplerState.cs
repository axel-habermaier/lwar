namespace Pegasus.Platform.Graphics.Interface
{
	using System;

	/// <summary>
	///     Describes a sampler state of a shader pipeline stage.
	/// </summary>
	internal interface ISamplerState : IDisposable
	{
		/// <summary>
		///     Sets the state on the pipeline.
		/// </summary>
		/// <param name="slot">The slot the sampler state should be bound to.</param>
		void Bind(int slot);

		/// <summary>
		///     Sets the debug name of the state.
		/// </summary>
		/// <param name="name">The debug name of the state.</param>
		void SetName(string name);
	}
}