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
		///   The cvar registry that is used to persist resolution and window size changes.
		/// </summary>
		private readonly CvarRegistry _cvars;

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
		private readonly LogicalInput _toggleMode = new LogicalInput(Key.LeftAlt.IsPressed() + Key.Return.IsPressed(), InputModes.All);

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
		/// <param name="cvars">The cvar registry that should be used to persist resolution and window size changes.</param>
		public ResolutionManager(Window window, SwapChain swapChain, LogicalInputDevice device, CvarRegistry cvars)
		{
			Assert.ArgumentNotNull(window);
			Assert.ArgumentNotNull(swapChain);
			Assert.ArgumentNotNull(device);
			Assert.ArgumentNotNull(cvars);

			_window = window;
			_swapChain = swapChain;
			_device = device;
			_cvars = cvars;

			_cvars.Instances.WindowWidth.Changed += UpdateWindowSize;
			_cvars.Instances.WindowHeight.Changed += UpdateWindowSize;

			UpdateGraphicsState();
			_device.Register(_toggleMode);
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
				_cvars.Instances.Fullscreen.SetImmediate(!_cvars.Fullscreen);
				UpdateGraphicsState();
			}

			// We do not care about the window size in fullscreen mode
			if (_cvars.Fullscreen)
				return;

			// If we set the window width and height cvars only to the values sent by the window's resize event, we would
			// miss some values when we request the window size to change, but it is in fact not changed (that happens,
			// for instance, when the requested size exceeds the screen's resolution). Therefore, we always set the
			// cvars to the windows actual size each frame
			var size = _window.Size;

			if (_cvars.WindowWidth != size.Width)
				_cvars.WindowWidth = size.Width;

			if (_cvars.WindowHeight != size.Height)
				_cvars.WindowHeight = size.Height;
		}

		/// <summary>
		///   Updates the state of the graphics subsystem, handling transitions in and out of fullscreen mode.
		/// </summary>
		public void UpdateGraphicsState()
		{
			// Execute all deferred cvar updates
			_cvars.ExecuteDeferredUpdates(UpdateMode.OnGraphicsRestart);

			// Resize and update the window and the swap chain depending on whether we're in fullscreen or windowed mode
			if (_cvars.Fullscreen)
			{
				Log.Info("Switching to fullscreen resolution {0}x{1}.", _cvars.ResolutionWidth, _cvars.ResolutionHeight);
				if (!_swapChain.UpdateState(_cvars.ResolutionWidth, _cvars.ResolutionHeight, _cvars.Fullscreen))
				{
					_cvars.Instances.Fullscreen.SetImmediate(false);
					UpdateGraphicsState();
				}
			}
			else
			{
				Log.Info("Switching to windowed resolution {0}x{1}.", _cvars.WindowWidth, _cvars.WindowHeight);

				// It's important to set the window's size first and then to use the actual size of the window
				// (which might be smaller than the requested one) to update the swap chain
				_window.Size = new Size(_cvars.WindowWidth, _cvars.WindowHeight);
				_swapChain.UpdateState(_window.Size.Width, _window.Size.Height, _cvars.Fullscreen);
			}
		}

		/// <summary>
		///   Sets the window's size to the values stored in the window size cvars.
		/// </summary>
		private void UpdateWindowSize(int value)
		{
			// Ignore the changes while in fullscreen mode
			if (!_cvars.Fullscreen)
				_window.Size = new Size(_cvars.WindowWidth, _cvars.WindowHeight);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_cvars.Instances.WindowWidth.Changed -= UpdateWindowSize;
			_cvars.Instances.WindowHeight.Changed -= UpdateWindowSize;
			_device.Remove(_toggleMode);
		}
	}
}