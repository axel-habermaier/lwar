using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   The camera that is used during regular game sessions.
	/// </summary>
	public class Camera : ICamera
	{
		/// <summary>
		///   Gets or sets the camera's projection matrix.
		/// </summary>
		public Matrix Projection { get; set; }

		/// <summary>
		///   Gets or sets the camera's view matrix.
		/// </summary>
		public Matrix View { get; set; }
	}
}