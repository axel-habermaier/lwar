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
		private const int CameraBufferSlot = 0;

		public Rectangle Viewport;

		/// <summary>
		///   The constant buffer that holds the per-frame camera-related data that is passed to each vertex shader.
		/// </summary>
		private readonly ConstantBuffer<CameraBuffer> _cameraBuffer;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		protected unsafe Camera(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.That(Marshal.SizeOf(typeof(CameraBuffer)) == CameraBuffer.Size, "Unexpected unmanaged size.");

			_cameraBuffer = new ConstantBuffer<CameraBuffer>(graphicsDevice, (buffer, data) => buffer.Copy(&data));
		}

		/// <summary>
		///   Activates this camera instance, causing all subsequent drawing operations to be relative to this camera instance.
		/// </summary>
		public void Bind()
		{
			_cameraBuffer.Bind(CameraBufferSlot);
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
		protected abstract void UpdateViewMatrix(out Matrix matrix);

		/// <summary>
		///   Stores the camera data that is passed to the vertex shaders.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = Size)]
		private struct CameraBuffer
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