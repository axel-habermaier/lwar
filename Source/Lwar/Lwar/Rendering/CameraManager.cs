﻿namespace Lwar.Rendering
{
	using System;
	using Gameplay;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface.Controls;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Scripting;

	/// <summary>
	///     Manages the game and debug cameras.
	/// </summary>
	public class CameraManager : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The debug camera that can be used to freely navigate the scene.
		/// </summary>
		private readonly DebugCamera _debugCamera;

		/// <summary>
		///     The window that outputs the scene rendered from the point of view of the active camera.
		/// </summary>
		private readonly Window _window;

		/// <summary>
		///     The active camera that is used to draw the scene.
		/// </summary>
		private Camera _activeCamera;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="localPlayer">The local player of the game session.</param>
		public CameraManager(Player localPlayer)
		{
			Assert.ArgumentNotNull(localPlayer);

			var graphicsDevice = Application.Current.GraphicsDevice;
			var inputDevice = Application.Current.Window.InputDevice;

			GameCamera = new GameCamera(graphicsDevice, inputDevice, localPlayer);
			_debugCamera = new DebugCamera(graphicsDevice, inputDevice);
			_window = Application.Current.Window;

			ActiveCamera = GameCamera;
			Commands.OnToggleDebugCamera += ToggleDebugCamera;
		}

		/// <summary>
		///     Gets the game camera that provides a top-down view of the scene and follows the local ship.
		/// </summary>
		public GameCamera GameCamera { get; private set; }

		/// <summary>
		///     Gets the active camera that is used to draw the scene.
		/// </summary>
		public Camera ActiveCamera
		{
			get { return _activeCamera; }
			private set
			{
				Assert.ArgumentNotNull(value);

				_debugCamera.IsActive = false;
				GameCamera.IsActive = false;

				ChangePropertyValue(ref _activeCamera, value);

				if (_activeCamera is GameCamera)
					GameCamera.IsActive = true;
				else
					_debugCamera.IsActive = true;
			}
		}

		/// <summary>
		///     Toggles between the game and the debug camera.
		/// </summary>
		private void ToggleDebugCamera()
		{
			if (ActiveCamera == _debugCamera)
			{
				ActiveCamera = GameCamera;
				_window.MouseCaptured = false;
			}
			else
			{
				ActiveCamera = _debugCamera;
				_debugCamera.Reset();
				_window.MouseCaptured = true;
			}
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
				_window.MouseCaptured = false;

			_debugCamera.SafeDispose();
			GameCamera.SafeDispose();
		}
	}
}