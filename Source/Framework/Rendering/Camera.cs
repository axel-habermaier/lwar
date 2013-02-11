using System;

namespace Pegasus.Framework.Rendering
{
	using System.Runtime.InteropServices;
	using Math;
	using Platform;
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
		private readonly ConstantBuffer _matricesBuffer;

		/// <summary>
		///   The camera matrices.
		/// </summary>
		private Matrices _matrices;

		/// <summary>
		///   The camera's viewport.
		/// </summary>
		private Rectangle _viewport;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		protected Camera(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);

			_matrices = new Matrices
			{
				View = Matrix.Identity,
				Projection = Matrix.Identity,
				ViewProjection = Matrix.Identity
			};
			_matricesBuffer = ConstantBuffer.Create(graphicsDevice, _matrices);
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
		///   Activates this camera instance, causing all subsequent drawing operations to be relative to this camera instance.
		/// </summary>
		public void Bind()
		{
			_matricesBuffer.Bind(CameraConstantsSlot);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_matricesBuffer.SafeDispose();
		}

		/// <summary>
		///   Updates the projection matrix based on the current camera configuration.
		/// </summary>
		protected void UpdateProjectionMatrix()
		{
			UpdateProjectionMatrix(out _matrices.Projection);
			UpdateConstantBuffer();
		}

		/// <summary>
		///   Updates the view matrix based on the current camera configuration.
		/// </summary>
		protected void UpdateViewMatrix()
		{
			UpdateViewMatrix(out _matrices.View);
			UpdateConstantBuffer();
		}

		/// <summary>
		///   Updates the matrices constant buffer.
		/// </summary>
		private void UpdateConstantBuffer()
		{
			Assert.That(Marshal.SizeOf(typeof(Matrices)) == Matrices.Size, "Unexpected unmanaged size.");

			_matrices.ViewProjection = _matrices.View * _matrices.Projection;
			_matrices.UsePointer(p => _matricesBuffer.SetData(p, Matrices.Size));
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
		protected abstract void UpdateViewMatrix(out Matrix matrix);

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