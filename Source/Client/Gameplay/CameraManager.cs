using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Manages the game and debug cameras.
	/// </summary>
	public class CameraManager : DisposableObject
	{
		/// <summary>
		///   The debug camera that can be used to freely navigate the scene.
		/// </summary>
		private readonly DebugCamera _debugCamera;

		/// <summary>
		///   The logical input device that provides the user input for the cameras.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///   The window that outputs the scene rendered from the point of view of the active camera.
		/// </summary>
		private readonly Window _window;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that displays the scene rendered from the point of view of the active camera.</param>
		/// <param name="graphicsDevice">The graphics device the cameras are created for.</param>
		/// <param name="inputDevice">The logical input device that provides the user input for the cameras.</param>
		public CameraManager(Window window, GraphicsDevice graphicsDevice, LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(window, () => window);
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(inputDevice, () => inputDevice);

			_window = window;
			_inputDevice = inputDevice;

			GameCamera = new Camera3D(graphicsDevice) { FieldOfView = MathUtils.DegToRad(20), Up = new Vector3(0, 0, 1) };
			_debugCamera = new DebugCamera(graphicsDevice, inputDevice);

			ActiveCamera = GameCamera;
			_inputDevice.Modes = InputModes.Game;

			LwarCommands.ToggleDebugCamera.Invoked += ToggleDebugCamera;
			_window.Resized += WindowResized;
		}

		/// <summary>
		///   Gets the game camera that provides a top-down view of the scene and follows the local ship.
		/// </summary>
		public Camera3D GameCamera { get; private set; }

		/// <summary>
		///   Gets the active camera that should be used to draw the scene.
		/// </summary>
		public Camera ActiveCamera { get; private set; }

		/// <summary>
		///   Updates the viewports of the cameras.
		/// </summary>
		/// <param name="windowSize">The new size of the window.</param>
		private void WindowResized(Size windowSize)
		{
			var viewport = new Rectangle(0, 0, windowSize.Width, windowSize.Height);

			GameCamera.Viewport = viewport;
			_debugCamera.Viewport = viewport;
		}

		/// <summary>
		///   Toggles between the game and the debug camera.
		/// </summary>
		private void ToggleDebugCamera()
		{
			if (ActiveCamera == _debugCamera)
			{
				ActiveCamera = GameCamera;

				_inputDevice.Modes = InputModes.Game;
				_window.MouseCaptured = false;
			}
			else
			{
				ActiveCamera = _debugCamera;
				_debugCamera.Reset();

				_inputDevice.Modes = InputModes.Debug;
				_window.MouseCaptured = true;
			}
		}

		/// <summary>
		///   Updates the camera manager.
		/// </summary>
		public void Update()
		{
			_debugCamera.Update();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_debugCamera.SafeDispose();
			GameCamera.SafeDispose();

			LwarCommands.ToggleDebugCamera.Invoked -= ToggleDebugCamera;
			_window.Resized -= WindowResized;
		}
	}
}