using System;

namespace Pegasus.AssetsCompiler.Effects
{
	using Math;

	/// <summary>
	///   Represents an effect that provides shaders for drawing operations.
	/// </summary>
	public abstract class Effect
	{
		/// <summary>
		///   The projection matrix that should be used to project the geometry onto the screen.
		/// </summary>
		protected readonly Matrix Projection;

		/// <summary>
		///   The view matrix of the camera that should be used to render the geometry.
		/// </summary>
		protected readonly Matrix View;

		/// <summary>
		///   The view matrix multiplied with the projection matrix.
		/// </summary>
		protected readonly Matrix ViewProjection;

		/// <summary>
		///   The size of the viewport.
		/// </summary>
		protected readonly Vector2 ViewportSize;

		/// <summary>
		///   The world matrix that should be used to transform the geometry.
		/// </summary>
		protected readonly Matrix World;
	}
}