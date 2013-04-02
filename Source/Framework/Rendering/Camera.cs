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
		///   The constant buffer that holds the per-frame camera-related data that is passed to each vertex shader.
		/// </summary>
		private readonly ConstantBuffer<CameraData> _cameraBuffer;

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
			Assert.That(Marshal.SizeOf(typeof(CameraData)) == CameraData.Size, "Unexpected unmanaged size.");
			
			_cameraBuffer = new ConstantBuffer<CameraData>(graphicsDevice, (buffer, data) => buffer.Copy(&data));
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
				_cameraBuffer.Data.ViewportSize = new Vector2(_viewport.Width, _viewport.Height);
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
			_cameraBuffer.Bind(CameraConstantsSlot);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_cameraBuffer.SafeDispose();
		}

		/// <summary>
		///   Updates the projection matrix based on the current camera configuration.
		/// </summary>
		protected void UpdateProjectionMatrix()
		{
			UpdateProjectionMatrix(out _cameraBuffer.Data.Projection);
			UpdateConstantBuffer();
		}

		/// <summary>
		///   Updates the view matrix based on the current camera configuration.
		/// </summary>
		protected void UpdateViewMatrix()
		{
			UpdateViewMatrix(out _cameraBuffer.Data.View);
			UpdateConstantBuffer();
		}

		/// <summary>
		///   Updates the matrices constant buffer.
		/// </summary>
		private void UpdateConstantBuffer()
		{
			

			_cameraBuffer.Data.ViewProjection = _cameraBuffer.Data.View * _cameraBuffer.Data.Projection;
			_cameraBuffer.Update();
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
		///   Stores the camera data that is passed to the vertex shaders.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = Size)]
		private struct CameraData
		{
			/// <summary>
			///   The size of the struct in bytes.
			/// </summary>
			public const int Size = 208;

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

			/// <summary>
			///   The size of the viewport in pixels.
			/// </summary>
			public Vector2 ViewportSize;
		}
	}
}