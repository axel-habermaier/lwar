﻿namespace Lwar.Rendering
{
	using System;
	using Gameplay.Client;
	using Pegasus;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Pegasus.UserInterface;
	using Pegasus.UserInterface.Input;
	using Pegasus.Utilities;

	/// <summary>
	///     Manages the game and debug cameras.
	/// </summary>
	internal class CameraManager : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The debug camera that can be used to freely navigate the scene.
		/// </summary>
		private readonly DebugCamera _debugCamera;

		/// <summary>
		///     The active camera that is used to draw the scene.
		/// </summary>
		private Camera _activeCamera;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="localPlayer">The local player of the game session.</param>
		/// <param name="inputDevice">The input device that should be used to control the cameras.</param>
		public CameraManager(Player localPlayer, LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(localPlayer);
			Assert.ArgumentNotNull(inputDevice);

			var graphicsDevice = Application.Current.GraphicsDevice;

			GameCamera = new GameCamera(graphicsDevice, inputDevice, localPlayer);
			_debugCamera = new DebugCamera(graphicsDevice, inputDevice);

			ActiveCamera = GameCamera;
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
		public void ToggleDebugCamera()
		{
			if (ActiveCamera == _debugCamera)
			{
				ActiveCamera = GameCamera;
				Mouse.RelativeMouseMode = false;
			}
			else
			{
				ActiveCamera = _debugCamera;
				_debugCamera.Reset();
				Mouse.RelativeMouseMode = true;
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
			if (ActiveCamera == _debugCamera)
				Mouse.RelativeMouseMode = false;

			_debugCamera.SafeDispose();
			GameCamera.SafeDispose();
		}
	}
}