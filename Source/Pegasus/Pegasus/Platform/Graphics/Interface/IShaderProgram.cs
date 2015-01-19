namespace Pegasus.Platform.Graphics.Interface
{
	using System;

	/// <summary>
	///     Represents a combination of different shader programs that control the various pipeline stages of the GPU.
	/// </summary>
	internal interface IShaderProgram : IDisposable
	{
		/// <summary>
		///     Binds the shaders of the shader program to the various pipeline stages.
		/// </summary>
		void Bind();
	}
}