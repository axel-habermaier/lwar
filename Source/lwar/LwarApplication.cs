using System;

namespace Lwar
{
	using System.Net;
	using Network;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Controls;
	using Pegasus.Platform.Assets;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Input;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Scripting;

	internal class HelloWorldViewModel : ViewModel
	{
		private int _frameCount;
		private string _name;

		public string Name
		{
			get { return _name; }
			set { ChangePropertyValue(ref _name, value); }
		}

		public int FrameCount
		{
			get { return _frameCount; }
			set { ChangePropertyValue(ref _frameCount, value); }
		}

		public void Update()
		{
			++_frameCount;

			if (Model != null)
				Model.FrameCount++;

			if (_frameCount % 1000 == 0)
				Model = Model == null ? new HelloWorldViewModel() : null;
		}

		private HelloWorldViewModel _model;

		public HelloWorldViewModel Model
		{
			get { return _model; }
			set { ChangePropertyValue(ref _model, value); }
		}
	}

	internal class HelloWorldView : UserControl
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load the assets required by the control.</param>
		/// <param name="viewModel">The view model that should be bound to the control.</param>
		public HelloWorldView(AssetsManager assets, ViewModel viewModel)
			: base(assets, viewModel)
		{
			var style = new Style();
			style.Setters.Add(new Setter<Color>(ForegroundProperty, Color.FromRgba(255, 0, 0, 255)));

			var trigger = new Trigger<bool>(IsMouseOverProperty, true);
			trigger.Setters.Add(new Setter<Color>(ForegroundProperty, Color.FromRgba(0, 255, 123, 255)));
			style.Triggers.Add(trigger);

			Resources["MyStyle"] = style;

			var binding1 = new Binding<object>(v => ((HelloWorldViewModel)v).Name);
			var binding2 = new Binding<object>(v => ((HelloWorldViewModel)v).Model.FrameCount);

			var canvas = new Canvas();
			var button1 = new Button() { Width = 300, Height = 100 };
			var button2 = new Button() { Width = 100, Height = 300 };

			button1.SetBinding(ContentProperty, binding1);
			button2.SetBinding(ContentProperty, binding2);

			canvas.Children.Add(button1);
			canvas.Children.Add(button2);

			Content = canvas;

			var resourceBinding = new ResourceBinding<Style>("MyStyle");
			button1.SetResourceBinding(StyleProperty, resourceBinding);

			button2.ViewModel = viewModel;
		}
	}

	/// <summary>
	///   Represents the lwar application.
	/// </summary>
	internal sealed class LwarApplication : Application
	{
		/// <summary>
		///   The local game server that can be used to hosts game sessions locally.
		/// </summary>
		private LocalServer _localServer;

		private HelloWorldViewModel _viewModel;

		/// <summary>
		///   Invoked when the application is initializing.
		/// </summary>
		/// <param name="uiContext">The UI context that is used to draw the user interface.</param>
		protected override void Initialize(UIContext uiContext)
		{
			Commands.Resolve();
			Cvars.Resolve();

			Commands.OnConnect += Connect;
			Commands.OnDisconnect += Disconnect;

			//Context.InputDevice.ActivateLayer(InputLayers.Game);
			//Context.Window.Closing += Exit;

			_localServer = new LocalServer();
			//_stateManager = new ScreenManager(Context);
			//_stateManager.Add(new MainMenu());

			Commands.Bind(Key.F1.WentDown(), "start_server");
			Commands.Bind(Key.F2.WentDown(), "stop_server");
			Commands.Bind(Key.F3.WentDown(), "connect 127.0.0.1");
			Commands.Bind(Key.F4.WentDown(), "disconnect");
			Commands.Bind(Key.F5.WentDown(), "reload_assets");
			Commands.Bind(Key.C.WentDown(), "toggle_debug_camera");
			Commands.Bind(Key.Escape.WentDown(), "exit");
			Commands.Bind(Key.F9.WentDown(), "toggle show_platform_info");
			Commands.Bind(Key.F10.WentDown(), "toggle show_frame_stats");

			_viewModel = new HelloWorldViewModel() { Name = "Axel" };
			var view = new HelloWorldView(uiContext.SharedAssets, _viewModel);
			uiContext.Add(view);
		}

		/// <summary>
		///   Invoked when the application should update the its state.
		/// </summary>
		protected override void Update()
		{
			_localServer.Update();
			//_stateManager.Update();
			_viewModel.Update();
		}

		/// <summary>
		///   Invoked when the application should draw a frame.
		/// </summary>
		/// <param name="output">The output that the scene should be rendered to.</param>
		protected override void Draw(RenderOutput output)
		{
			output.ClearColor(new Color(0, 0, 0, 0));
			output.ClearDepth();

			//_stateManager.Draw(output);
		}

		/// <summary>
		///   Invoked when the application should draw the user interface.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		protected override void DrawUserInterface(SpriteBatch spriteBatch)
		{
			//_stateManager.DrawUserInterface(spriteBatch);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void Dispose()
		{
			Commands.OnConnect -= Connect;
			Commands.OnDisconnect -= Disconnect;

			//_stateManager.SafeDispose();
			_localServer.SafeDispose();
		}

		/// <summary>
		///   Connects to the server at the given end point and joins the game session.
		/// </summary>
		/// <param name="address">The IP address of the server.</param>
		/// <param name="port">The port of the server.</param>
		private void Connect(IPAddress address, ushort port)
		{
			Assert.ArgumentNotNull(address);

			Disconnect();
			//_stateManager.Add(new Level(new IPEndPoint(address, port)));
		}

		/// <summary>
		///   Disconnects from the game session the client is currently connected to.
		/// </summary>
		private void Disconnect()
		{
			//_stateManager.Clear();
			//_stateManager.Add(new MainMenu());
		}
	}
}