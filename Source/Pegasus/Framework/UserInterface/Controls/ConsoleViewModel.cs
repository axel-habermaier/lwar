namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Assets;
	using Platform.Input;
	using Platform.Memory;
	using Console = Rendering.UserInterface.Console;

	/// <summary>
	///     Displays the in-game console.
	/// </summary>
	internal class ConsoleViewModel : ViewModel
	{
		/// <summary>
		///     The console view.
		/// </summary>
		private readonly ConsoleView _view;

		/// <summary>
		///     The window in which the console is shown.
		/// </summary>
		private AppWindow _window;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window in which the console should be shown.</param>
		/// <param name="inputDevice">The logical input device that should be used to listen for user input.</param>
		public ConsoleViewModel(AppWindow window, LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(window);
			Assert.ArgumentNotNull(inputDevice);

			var font = Application.Current.Assets.Load(Fonts.LiberationMono11);
			Console = new Console(inputDevice, font);
			Console.Update(window.Size);

			_view = new ConsoleView(Console);
			_window = window;
			window.LayoutRoot.AddTopmost(_view);
		}

		/// <summary>
		///     Gets the in-game console.
		/// </summary>
		public Console Console { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Console.SafeDispose();
			_window.LayoutRoot.RemoveTopmost(_view);
		}
	}
}