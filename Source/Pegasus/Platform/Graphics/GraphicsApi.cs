namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///   Determines the type of the graphics API.
	/// </summary>
	public enum GraphicsApi
	{
		/// <summary>
		///   Indicates that OpenGL 3.3 is used for rendering.
		/// </summary>
		OpenGL3 = 701,

		/// <summary>
		///   Indicates that Direct3D 11 is used for rendering.
		/// </summary>
		Direct3D11 = 702
	}
}