using System;

namespace Pegasus.Framework.Rendering
{
	using System.Runtime.InteropServices;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   Represents a camera that can be used to draw scenes.
	/// </summary>
	public abstract class Camera : DisposableObject
	{
		/// <summary>
		///   The constant buffer slot that is used to pass the camera matrices to the vertex shader.
		/// </summary>
		private const int CameraConstantsSlot = 0;

		/// <summary>
		///   The constant buffer that holds the per-frame camera-related matrices that are passed to each vertex shader.
		/// </summary>
		private readonly ConstantBuffer<Matrices> _matrices;

		/// <summary>
		///   The camera's position within the world.
		/// </summary>
		private Vector3 _position;

		/// <summary>
		///   The target the camera looks at.
		/// </summary>
		private Vector3 _target;

		/// <summary>
		///   The up vector.
		/// </summary>
		private Vector3 _up;

		/// <summary>
		///   The camera's viewport.
		/// </summary>
		private Rectangle _viewport;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		protected unsafe Camera(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			_matrices = new ConstantBuffer<Matrices>(graphicsDevice, (buffer, matrices) => buffer.Copy(&matrices));
		}

		/// <summary>
		///   Gets or sets the camera's viewport.
		/// </summary>
		public Rectangle Viewport
		{
			get { return _viewport; }
			set
			{
				_viewport = value;
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		///   Gets or sets the camera's position within the world.
		/// </summary>
		public Vector3 Position
		{
			get { return _position; }
			set
			{
				_position = value;
				UpdateViewMatrix();
			}
		}

		/// <summary>
		///   Gets or sets the target the camera looks at.
		/// </summary>
		public Vector3 Target
		{
			get { return _target; }
			set
			{
				_target = value;
				UpdateViewMatrix();
			}
		}

		/// <summary>
		///   Gets or sets the up vector.
		/// </summary>
		public Vector3 Up
		{
			get { return _up; }
			set
			{
				_up = value;
				UpdateViewMatrix();
			}
		}

		/// <summary>
		///   Activates this camera instance, causing all subsequent drawing operations to be relative to this camera instance.
		/// </summary>
		public void Bind()
		{
			_matrices.Bind(CameraConstantsSlot);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_matrices.SafeDispose();
		}

		/// <summary>
		///   Updates the projection matrix based on the current camera configuration.
		/// </summary>
		protected void UpdateProjectionMatrix()
		{
			UpdateProjectionMatrix(out _matrices.Data.Projection);
			UpdateConstantBuffer();
		}

		/// <summary>
		///   Updates the view matrix based on the current camera configuration.
		/// </summary>
		protected void UpdateViewMatrix()
		{
			UpdateViewMatrix(out _matrices.Data.View);
			UpdateConstantBuffer();
		}

		/// <summary>
		///   Updates the matrices constant buffer.
		/// </summary>
		private void UpdateConstantBuffer()
		{
			Assert.That(Marshal.SizeOf(typeof(Matrices)) == Matrices.Size, "Unexpected unmanaged size.");

			_matrices.Data.ViewProjection = _matrices.Data.View * _matrices.Data.Projection;
			_matrices.Update();
		}

		/// <summary>
		///   Updates the projection matrix based on the current camera configuration.
		/// </summary>
		/// <param name="matrix">The matrix that should hold the projection matrix once the method returns.</param>
		protected abstract void UpdateProjectionMatrix(out Matrix matrix);

		/// <summary>
		///   Updates the view matrix based on the current camera configuration.
		/// </summary>
		/// <param name="matrix">The matrix that should hold the view matrix once the method returns.</param>
		protected virtual void UpdateViewMatrix(out Matrix matrix)
		{
			matrix = Matrix.CreateLookAt(Position, Target, Up);
		}

		/// <summary>
		///   Stores the camera matrices.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct Matrices
		{
			/// <summary>
			///   The size of the struct in bytes.
			/// </summary>
			public const int Size = 192;

			/// <summary>
			///   The view matrix, where the camera lies in the origin.
			/// </summary>
			public Matrix View;

			/// <summary>
			///   The projection matrix used by the camera.
			/// </summary>
			public Matrix Projection;

			/// <summary>
			///   The product of the view and the projection matrix that is pre-calculated on the CPU.
			/// </summary>
			public Matrix ViewProjection;
		}
	}
}