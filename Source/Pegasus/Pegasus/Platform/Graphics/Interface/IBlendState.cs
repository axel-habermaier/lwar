namespace Pegasus.Platform.Graphics.Interface
{
	using System;

	/// <summary>
	///     Describes a blend state of the output merger pipeline stage.
	/// </summary>
	internal interface IBlendState : IDisposable
	{
		/// <summary>
		///     Sets the state on the pipeline.
		/// </summary>
		void Bind();

		/// <summary>
		///     Sets the debug name of the state.
		/// </summary>
		/// <param name="name">The debug name of the state.</param>
		void SetName(string name);
	}
}