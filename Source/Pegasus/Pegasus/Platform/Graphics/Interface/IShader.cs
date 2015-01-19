namespace Pegasus.Platform.Graphics.Interface
{
	using System;

	/// <summary>
	///     Represents a shader that controls a programmable stage of the graphics pipeline.
	/// </summary>
	internal interface IShader : IDisposable
	{
		/// <summary>
		///     Sets the debug name of the shader.
		/// </summary>
		/// <param name="name">The debug name of the shader.</param>
		void SetName(string name);
	}
}