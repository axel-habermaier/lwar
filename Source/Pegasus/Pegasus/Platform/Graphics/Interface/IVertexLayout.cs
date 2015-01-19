namespace Pegasus.Platform.Graphics.Interface
{
	using System;

	/// <summary>
	///     Represents a description of the memory layout and other properties of the vertex data that is fed into the
	///     input-assembler stage of the graphics pipeline.
	/// </summary>
	internal interface IVertexLayout : IDisposable
	{
		/// <summary>
		///     Binds the input layout to the input-assembler stage of the pipeline.
		/// </summary>
		void Bind();
	}
}