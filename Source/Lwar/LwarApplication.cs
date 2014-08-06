namespace Lwar
{
	using System;
	using Assets;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface.ViewModels;
	using Pegasus.Framework.UserInterface.Input;
	using Pegasus.Platform.Memory;
	using Scripting;
	using UserInterface;

	/// <summary>
	///     Represents the Lwar application.
	/// </summary>
	public sealed partial class LwarApplication
	{
		/// <summary>
		///     The local game server that can be used to hosts game sessions locally.
		/// </summary>
		private readonly LocalServer _localServer = new LocalServer();

		/// <summary>
		///     The root view model of the view model stacked used by the application.
		/// </summary>
		private readonly StackedViewModel _viewModelRoot = StackedViewModel.CreateRoot();

		/// <summary>
		///     Invoked when the application is initializing.
		/// </summary>
		protected override void Initialize()
		{
			RegisterFontLoader(new FontLoader(Assets));
			Commands.Resolve();
			Cvars.Resolve();

			Window.InputDevice.ActivateLayer(InputLayers.Game);
			Window.Closing += Exit;

			Commands.Bind(Key.F1.WentDown(), "start_server");
			Commands.Bind(Key.F2.WentDown(), "stop_server");
			Commands.Bind(Key.F3.WentDown(), "connect 127.0.0.1");
			Commands.Bind(Key.F4.WentDown(), "disconnect");
			Commands.Bind(Key.F5.WentDown(), "reload_assets");

			Commands.Bind(Key.C.WentDown(), "toggle_debug_camera");
			Commands.Bind(Key.Escape.WentDown(), "exit");
			Commands.Bind(Key.F10.WentDown(), "toggle show_debug_overlay");

			//Window.LayoutRoot.Children.Add(new UserControl1());

			_viewModelRoot.Child = new MainMenuViewModel();
			_viewModelRoot.Activate();
		}

		/// <summary>
		///     Invoked when the application should update the its state.
		/// </summary>
		protected override void Update()
		{
			_localServer.Update();
			_viewModelRoot.Update();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void Dispose()
		{
			_viewModelRoot.SafeDispose();
			_localServer.SafeDispose();

			base.Dispose();
		}

		/// <summary>
		///     The entry point of the application.
		/// </summary>
		/// <param name="args">The command line arguments passed to the application.</param>
		public static void Main(string[] args)
		{
			Commands.Initialize();
			Cvars.Initialize();

			Bootstrapper<LwarApplication>.Run(args, "Lwar");
		}
	}
}