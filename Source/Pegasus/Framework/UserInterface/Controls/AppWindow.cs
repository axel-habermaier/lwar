namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Input;
	using Math;
	using Platform.Memory;
	using Rendering;
	using Scripting;
	using ViewModels;

	/// <summary>
	///     Represents the default window of an application.
	/// </summary>
	public partial class AppWindow
	{
		/// <summary>
		///     Manages the input bindings registered for this window.
		/// </summary>
		private readonly Bindings _bindings;

		/// <summary>
		///     The camera that is used to draw the console and the debug overlay.
		/// </summary>
		private readonly Camera _camera;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="mode">Indicates the window mode.</param>
		internal AppWindow(string title, Vector2i position, Size size, WindowMode mode)
			: base(title, position, size, mode)
		{
			InitializeComponents();

			InputDevice = new LogicalInputDevice(Keyboard, Mouse);
			_bindings = new Bindings(InputDevice);
			_camera = new Camera2D(Application.Current.GraphicsDevice);
			Console = new ConsoleViewModel(this, InputDevice);
		}

		/// <summary>
		///     Gets the logical input device that is used to handle all of the user input of this window.
		/// </summary>
		public LogicalInputDevice InputDevice { get; private set; }

		/// <summary>
		///     Gets the layout root of the application window.
		/// </summary>
		public AreaPanel LayoutRoot
		{
			get { return _layoutRoot; }
		}

		/// <summary>
		///     Gets the console that should be drawn on top of the window's contents.
		/// </summary>
		internal ConsoleViewModel Console { get; private set; }

		/// <summary>
		///     Invoked when the window is being closed.
		/// </summary>
		protected override void OnClosing()
		{
			_bindings.SafeDispose();
			InputDevice.SafeDispose();

			_camera.SafeDispose();
			Console.SafeDispose();

			base.OnClosing();
		}

		/// <summary>
		///     Processes all pending window events and handles the window's user input.
		/// </summary>
		internal override void HandleInput()
		{
			base.HandleInput();

			InputDevice.Update();
			_bindings.Update();

			InputDevice.TextInputEnabled = Keyboard.FocusedElement is ITextInputControl;
		}
	}
}