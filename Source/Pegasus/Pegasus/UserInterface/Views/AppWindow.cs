﻿namespace Pegasus.UserInterface.Views
{
	using System;
	using Controls;
	using Input;
	using Math;
	using Platform.Memory;
	using Scripting;

	/// <summary>
	///     Represents the default window of an application.
	/// </summary>
	public partial class AppWindow
	{
		/// <summary>
		///     Manages the input bindings registered for this window.
		/// </summary>
		private readonly BindingCollection _bindings;

		/// <summary>
		///     The logical input device that is used to handle all of the global user input of this window.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="dataContext">The data context that should be set on the window.</param>
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="mode">Indicates the window mode.</param>
		internal AppWindow(object dataContext, string title, Vector2 position, Size size, WindowMode mode)
			: base(title, position, size, mode)
		{
			_inputDevice = new LogicalInputDevice(this, usePreviewEvents: true);
			_bindings = new BindingCollection(_inputDevice);

			InputBindings.Add(new ScanCodeBinding(ScanCode.Grave, "ToggleConsole", triggerOnRepeat: false) { Preview = true });
			DataContext = dataContext;

			LoadContent();
		}

		/// <summary>
		///     Gets the layout root of the application window.
		/// </summary>
		// ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
		public AreaPanel LayoutRoot
		{
			get { return _layoutRoot; }
		}

		/// <summary>
		///     Invoked when the window is being closed.
		/// </summary>
		protected override void OnClosing()
		{
			_bindings.SafeDispose();
			_inputDevice.SafeDispose();

			base.OnClosing();
		}

		/// <summary>
		///     Processes all pending window events and handles the window's user input.
		/// </summary>
		internal override void HandleInput()
		{
			base.HandleInput();

			_inputDevice.Update();
			_bindings.Update();
		}
	}
}