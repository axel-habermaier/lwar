using System;

namespace Pegasus.Framework.Rendering
{
	using Math;

	/// <summary>
	///   Represents a camera that can be used to draw 3D scenes.
	/// </summary>
	public interface ICamera
	{
		/// <summary>
		///   Gets or sets the camera's projection matrix.
		/// </summary>
		Matrix Projection { get; set; }

		/// <summary>
		///   Gets or sets the camera's view matrix.
		/// </summary>
		Matrix View { get; set; }
	}
}