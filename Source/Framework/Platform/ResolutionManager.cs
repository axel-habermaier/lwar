using System;

namespace Pegasus.Framework.Platform
{
	using Graphics;
	using Input;
	using Logging;
	using Math;
	using Memory;
	using Scripting;

	/// <summary>
	///   Manages resolution and fullscreen mode changes.
	/// </summary>
	internal class ResolutionManager : DisposableObject
	{
		/// <summary>
		///   The logical input device that is used to toggle between fullscreen and windowed mode.
		/// </summary>
		private readonly LogicalInputDevice _device;

		/// <summary>
		///   The swap chain that is affected by resolution changes.
		/// </summary>
		private readonly SwapChain _swapChain;

		/// <summary>
		///   The logical input that toggles between fullscreen and windowed mode (Alt+Enter).
		/// </summary>
		private readonly LogicalInput _toggleMode = new LogicalInput(Key.LeftAlt.IsPressed() + Key.Return.IsPressed(), InputLayer.All);

		/// <summary>
		///   The window that is affected by resolution changes.
		/// </summary>
		private readonly Window _window;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that is affected by resolution changes.</param>
		/// <param name="swapChain">The swap chain that should be affected by resolution changes.</param>
		/// <param name="device">The logical input device that should be used to toggle between fullscreen and windowed mode.</param>
		public ResolutionManager(Window window, SwapChain swapChain, LogicalInputDevice device)
		{
			Assert.ArgumentNotNull(window);
			Assert.ArgumentNotNull(swapChain);
			Assert.ArgumentNotNull(device);

			_window = window;
			_swapChain = swapChain;
			_device = device;

			Cvars.WindowWidthChanged += UpdateWindowSize;
			Cvars.WindowHeightChanged += UpdateWindowSize;

			UpdateGraphicsState();
			_device.Add(_toggleMode);
		}

		/// <summary>
		///   Updates the resolution manager, ensuring that the application state remains consistent and that Alt+Enter mode toggle
		///   requests are handled.
		/// </summary>
		public void Update()
		{
			// Check if the user wants to toggle between fullscreen and windowed mode
			if (_toggleMode.IsTriggered)
			{
				Cvars.Fullscreen = !Cvars.Fullscreen;
				UpdateGraphicsState();
			}

			// We do not care about the window size in fullscreen mode; and if we're currently toggling the mode, ignore
			// the window size as well, as it might be outdated for the current frame
			if (Cvars.Fullscreen || _toggleMode.IsTriggered)
				return;

			// If we set the window width and height cvars only to the values sent by the window's resize event, we would
			// miss some values when we request the window size to change, but it is in fact not changed (that happens,
			// for instance, when the requested size exceeds the screen's resolution). Therefore, we always set the
			// cvars to the windows actual size each frame
			var size = _window.Size;

			if (Cvars.WindowWidth != size.Width)
				Cvars.WindowWidth = size.Width;

			if (Cvars.WindowHeight != size.Height)
				Cvars.WindowHeight = size.Height;
		}

		/// <summary>
		///   Updates the state of the graphics subsystem, handling transitions in and out of fullscreen mode.
		/// </summary>
		public void UpdateGraphicsState()
		{
			// Execute all deferred cvar updates
			CvarRegistry.ExecuteDeferredUpdates(UpdateMode.OnGraphicsRestart);

			// Resize and update the window and the swap chain depending on whether we're in fullscreen or windowed mode
			if (Cvars.Fullscreen)
			{
				Log.Info("Switching to fullscreen mode, resolution {0}x{1}.", Cvars.ResolutionWidth, Cvars.ResolutionHeight);
				if (!_swapChain.UpdateState(Cvars.ResolutionWidth, Cvars.ResolutionHeight, true))
				{
					Cvars.Fullscreen = false;
					UpdateGraphicsState();
				}
			}
			else
			{
				Log.Info("Switching to windowed mode, resolution {0}x{1}.", Cvars.WindowWidth, Cvars.WindowHeight);
				_swapChain.UpdateState(Cvars.WindowWidth, Cvars.WindowHeight, false);
			}
		}

		/// <summary>
		///   Sets the window's size to the values stored in the window size cvars.
		/// </summary>
		private void UpdateWindowSize(int value)
		{
			// Ignore the changes while in fullscreen mode
			if (!Cvars.Fullscreen)
				_window.Size = new Size(Cvars.WindowWidth, Cvars.WindowHeight);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Cvars.WindowWidthChanged -= UpdateWindowSize;
			Cvars.WindowHeightChanged -= UpdateWindowSize;
			_device.Remove(_toggleMode);
		}
	}
}