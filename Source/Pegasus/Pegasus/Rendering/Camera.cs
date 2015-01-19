namespace Pegasus.Rendering
{
	using System;
	using System.Runtime.InteropServices;
	using Math;
	using Platform.Graphics;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents a camera that can be used to draw scenes.
	/// </summary>
	public abstract class Camera : DisposableObject
	{
		/// <summary>
		///     The constant buffer that holds the camera-related data that is passed to each vertex shader.
		/// </summary>
		private readonly ConstantBuffer _cameraBuffer;

		/// <summary>
		///     Gets the camera's projection matrix.
		/// </summary>
		protected Matrix _projection;

		/// <summary>
		///     Gets the camera's view matrix.
		/// </summary>
		protected Matrix _view;

		/// <summary>
		///     Indicates whether the contents of the camera buffer are outdated.
		/// </summary>
		private bool _bufferUpdateRequired = true;

		/// <summary>
		///     The camera's position within the world.
		/// </summary>
		private Vector3 _position;

		/// <summary>
		///     The camera's viewport.
		/// </summary>
		private Rectangle _viewport;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		protected Camera(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.That(Marshal.SizeOf(typeof(CameraBuffer)) == CameraBuffer.Size, "Unexpected unmanaged size.");

			_cameraBuffer = new ConstantBuffer(graphicsDevice, CameraBuffer.Size, CameraBuffer.Slot);
			_cameraBuffer.SetName("CameraBuffer");

			UpdateProjectionMatrix();
			UpdateViewMatrix();
		}

		/// <summary>
		///     Gets or sets the camera's position within the world.
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
		///     Gets or sets the camera's viewport.
		/// </summary>
		public Rectangle Viewport
		{
			get { return _viewport; }
			set
			{
				if (_viewport == value)
					return;

				_viewport = value;
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		///     Activates this camera instance, causing all subsequent drawing operations to be relative to this camera instance.
		/// </summary>
		internal unsafe void Bind()
		{
			if (_bufferUpdateRequired)
			{
				var bufferData = new CameraBuffer(ref _view, ref _projection, ref _position);
				_cameraBuffer.CopyData(&bufferData);

				_bufferUpdateRequired = false;
			}

			_cameraBuffer.Bind();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_cameraBuffer.SafeDispose();
		}

		/// <summary>
		///     Updates the projection matrix based on the current camera configuration.
		/// </summary>
		protected void UpdateProjectionMatrix()
		{
			UpdateProjectionMatrixCore();
			_bufferUpdateRequired = true;
		}

		/// <summary>
		///     Updates the view matrix based on the current camera configuration.
		/// </summary>
		protected void UpdateViewMatrix()
		{
			UpdateViewMatrixCore();
			_bufferUpdateRequired = true;
		}

		/// <summary>
		///     Updates the projection matrix based on the current camera configuration.
		/// </summary>
		protected abstract void UpdateProjectionMatrixCore();

		/// <summary>
		///     Updates the view matrix based on the current camera configuration.
		/// </summary>
		protected abstract void UpdateViewMatrixCore();

		// Disable annoying "private field is assigned but its value is never used" warnings on Mono
#pragma warning disable 0414

		/// <summary>
		///     Stores the camera data that is passed to the vertex shaders.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = Size)]
		private struct CameraBuffer
		{
			/// <summary>
			///     The size of the struct in bytes.
			/// </summary>
			public const int Size = 208;

			/// <summary>
			///     The slot that is used to pass the camera buffer to the vertex shaders.
			/// </summary>
			public const int Slot = 0;

			/// <summary>
			///     The view matrix of the camera.
			/// </summary>
			private readonly Matrix _view;

			/// <summary>
			///     The projection matrix of the camera.
			/// </summary>
			private readonly Matrix _projection;

			/// <summary>
			///     The product of the view and the projection matrix that is pre-calculated on the CPU.
			/// </summary>
			private readonly Matrix _viewProjection;

			/// <summary>
			///     The position of the camera in world space.
			/// </summary>
			private readonly Vector3 _cameraPosition;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="view">The view matrix of the camera.</param>
			/// <param name="projection"> The projection matrix of the camera.</param>
			/// <param name="cameraPosition">The position of the camera in world space.</param>
			public CameraBuffer(ref Matrix view, ref Matrix projection, ref Vector3 cameraPosition)
			{
				_view = view;
				_projection = projection;
				_viewProjection = view * projection;
				_cameraPosition = cameraPosition;
			}
		}

#pragma warning restore 0414
	}
}