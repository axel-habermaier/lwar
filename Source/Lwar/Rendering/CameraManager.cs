﻿namespace Lwar.Rendering
{
	using System;
	using Pegasus;
	using Pegasus.Framework.UserInterface.Controls;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Input;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Scripting;

	/// <summary>
	///     Manages the game and debug cameras.
	/// </summary>
	public class CameraManager : DisposableObject
	{
		/// <summary>
		///     The debug camera that can be used to freely navigate the scene.
		/// </summary>
		private readonly DebugCamera _debugCamera;

		/// <summary>
		///     The logical input device that provides the user input for the cameras.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///     The window that outputs the scene rendered from the point of view of the active camera.
		/// </summary>
		private readonly Window _window;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that displays the scene rendered from the point of view of the active camera.</param>
		/// <param name="graphicsDevice">The graphics device the cameras are created for.</param>
		/// <param name="inputDevice">The logical input device that provides the user input for the cameras.</param>
		public CameraManager(Window window, GraphicsDevice graphicsDevice, LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(window);
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(inputDevice);

			_window = window;
			_inputDevice = inputDevice;

			GameCamera = new GameCamera(graphicsDevice, inputDevice);
			_debugCamera = new DebugCamera(graphicsDevice, inputDevice, InputLayers.Debug);

			ActiveCamera = GameCamera;
			GameCamera.IsActive = true;

			Commands.OnToggleDebugCamera += ToggleDebugCamera;
		}

		/// <summary>
		///     Gets the game camera that provides a top-down view of the scene and follows the local ship.
		/// </summary>
		public GameCamera GameCamera { get; private set; }

		/// <summary>
		///     Gets the active camera that should be used to draw the scene.
		/// </summary>
		public Camera ActiveCamera { get; private set; }

		/// <summary>
		///     Toggles between the game and the debug camera.
		/// </summary>
		private void ToggleDebugCamera()
		{
			if (ActiveCamera == _debugCamera)
			{
				ActiveCamera = GameCamera;

				_inputDevice.DeactivateLayer(InputLayers.Debug);
				_window.MouseCaptured = false;
			}
			else
			{
				ActiveCamera = _debugCamera;
				_debugCamera.Reset();

				_inputDevice.ActivateLayer(InputLayers.Debug);
				_window.MouseCaptured = true;
			}

			GameCamera.IsActive = ActiveCamera == GameCamera;
		}

		/// <summary>
		///     Updates the camera manager.
		/// </summary>
		public void Update()
		{
			GameCamera.Update();
			_debugCamera.Update();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.OnToggleDebugCamera -= ToggleDebugCamera;

			if (ActiveCamera == _debugCamera)
			{
				_window.MouseCaptured = false;
				_inputDevice.DeactivateLayer(InputLayers.Debug);
			}

			_debugCamera.SafeDispose();
			GameCamera.SafeDispose();
		}
	}
}