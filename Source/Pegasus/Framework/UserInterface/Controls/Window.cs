﻿namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///   Represents an operating system window that hosts UI elements.
	/// </summary>
	public abstract class Window : ContentControl, IDisposable
	{
		/// <summary>
		///   The application the window belongs to.
		/// </summary>
		private Application _application;

		/// <summary>
		///   The swap chain that is used to render the window's contents.
		/// </summary>
		private SwapChain _swapChain;

		/// <summary>
		///   The native operating system window that is used to display the UI.
		/// </summary>
		private NativeWindow _window;

		/// <summary>
		///   The window's title.
		/// </summary>
		private string _title = String.Empty;

		/// <summary>
		///   The screen position of the window's left upper corner.
		/// </summary>
		private Vector2i _position = Vector2i.Zero;

		/// <summary>
		/// Gets a value indicating whether the window is open.
		/// </summary>
		public bool IsOpen { get; private set; }

		/// <summary>
		///   The size of the window's rendering area.
		/// </summary>
		private Size _size = new Size(1024, 768);

		/// <summary>
		///   The window mode.
		/// </summary>
		private WindowMode _mode = WindowMode.Normal;

		/// <summary>
		///   Opens the window.
		/// </summary>
		/// <param name="application">The application the window belongs to.</param>
		internal void Open(Application application)
		{
			Assert.ArgumentNotNull(application);
			Assert.That(!IsOpen, "The window is already open.");

			_application = application;

			_window = new NativeWindow(Title, Position, Size, Mode);
			_swapChain = new SwapChain(application.GraphicsDevice, _window, false, _window.Size);

			IsOpen = true;
		}

		/// <summary>
		///   Sets the window's title.
		/// </summary>
		public string Title
		{
			get { return _title; }
			set
			{
				if (_title == value)
					return;

				_title = value;
				_window.Title = value;
			}
		}

		/// <summary>
		///   Gets or sets the size of the window's rendering area.
		/// </summary>
		public Size Size
		{
			get { return _size; }
			set
			{
				if (_size == value)
					return;

				_size = value;
				_window.Size = value;
			}
		}

		/// <summary>
		///   Gets or sets the screen position of the window's left upper corner.
		/// </summary>
		public Vector2i Position
		{
			get { return _position; }
			set
			{
				if (_position == value)
					return;

				_position = value;
				_window.Position = value;
			}
		}

		/// <summary>
		///   Gets or sets the window state.
		/// </summary>
		public WindowMode Mode
		{
			get { return _mode; }
			set
			{
				if (_mode == value)
					return;

				_mode = value;
				_window.Mode = value;
			}
		}

		/// <summary>
		///   Closes the window.
		/// </summary>
		public void Close()
		{
			_application.RemoveWindow(this);

			_swapChain.SafeDispose();
			_window.SafeDispose();

			IsOpen = false;
		}

		/// <summary>
		///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			Close();

#if DEBUG
			GC.SuppressFinalize(this);
#endif
		}

#if DEBUG
		/// <summary>
		///   Ensures that the instance has been disposed.
		/// </summary>
		~Window()
		{
			Log.Error("Finalizer runs for an instance of '{0}'.", GetType().FullName);
		}
#endif
	}
}