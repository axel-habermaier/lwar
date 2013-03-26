using System;

namespace Pegasus.AssetsCompiler.ShaderCompilation
{
	using Library;

	/// <summary>
	///   Represents an effect that provides shaders for drawing operations.
	/// </summary>
	public class Effect
	{
		/// <summary>
		///   Gets the projection matrix that should be used to project the geometry onto the screen.
		/// </summary>
		protected Matrix Projection
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		///   Gets the view matrix of the camera that should be used to render the geometry.
		/// </summary>
		protected Matrix View
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		///   Gets the view matrix multiplied with the projection matrix.
		/// </summary>
		protected Matrix ViewProjection
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		///   Gets the world matrix that should be used to transform the geometry.
		/// </summary>
		protected Matrix World
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		///   Gets the size of the viewport.
		/// </summary>
		protected Vector2 ViewportSize
		{
			get { throw new NotSupportedException(); }
		}
	}
}