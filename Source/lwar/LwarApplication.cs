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
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Scripting;

	internal class HelloWorldViewModel : ViewModel
	{
		private int _frameCount;
		private HelloWorldViewModel _model;
		private string _name;
		private string _name2;
		private string _name3;

		public string Name
		{
			get { return _name; }
			set
			{
				ChangePropertyValue(ref _name, value);
				Name2 = Name + Name;
				Name3 = Name2 + Name;
			}
		}

		public string Name2
		{
			get { return _name2; }
			set { ChangePropertyValue(ref _name2, value); }
		}

		public string Name3
		{
			get { return _name3; }
			set { ChangePropertyValue(ref _name3, value); }
		}

		public int FrameCount
		{
			get { return _frameCount; }
			set { ChangePropertyValue(ref _frameCount, value); }
		}

		public HelloWorldViewModel Model
		{
			get { return _model; }
			set { ChangePropertyValue(ref _model, value); }
		}

		public void Update()
		{
			++_frameCount;

			if (Model != null)
				Model.FrameCount++;

			//if (_frameCount % 1000 == 0)
			//Model = Model == null ? new HelloWorldViewModel() : null;

			if (_model == null)
				_model = new HelloWorldViewModel();
		}
	}

	internal class HelloWorldView : UserControl
	{
		public Button button1;
		//public Canvas canvas;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load the assets required by the control.</param>
		/// <param name="viewModel">The view model that should be bound to the control.</param>
		public HelloWorldView(AssetsManager assets, ViewModel viewModel)
			: base(assets, viewModel)
		{
			var baseStyle = new Style();
			baseStyle.Setters.Add(new Setter<Color>(ForegroundProperty, Color.FromRgba(0, 255, 0, 255)));

			var baseTrigger = new Trigger<bool>(IsMouseOverProperty, true);
			baseTrigger.Setters.Add(new Setter<Color>(ForegroundProperty, Color.FromRgba(0, 0, 255, 255)));
			baseStyle.Triggers.Add(baseTrigger);

			var style = new Style(baseStyle);
			style.Setters.Add(new Setter<Color>(ForegroundProperty, Color.FromRgba(255, 0, 0, 255)));

			var trigger = new Trigger<bool>(IsMouseOverProperty, true);
			trigger.Setters.Add(new Setter<Color>(ForegroundProperty, Color.FromRgba(0, 255, 123, 255)));
			style.Triggers.Add(trigger);

			Resources["MyStyle"] = style;

			var binding1 = new Binding<object>(v => ((HelloWorldViewModel)v).Name);
			var binding2 = new Binding<object>(v => ((HelloWorldViewModel)v).Name2);
			var binding3 = new Binding<object>(v => ((HelloWorldViewModel)v).Name3);

			button1 = new Button() { Width = 300, Height = 100 };
			var button2 = new Button()
			{
				Width = 100,
				Height = 300,
				Margin = new Thickness(15, 5, 21, 15),
				//HorizontalAlignment = HorizontalAlignment.Left,
				//VerticalAlignment = VerticalAlignment.Top
			};
			var button3 = new Button()
			{
				Width = 100,
				Height = 300,
				Margin = new Thickness(0, 100, 0, 0),
				//HorizontalAlignment = HorizontalAlignment.Center,
				//VerticalAlignment = VerticalAlignment.Top
			};

			//button1.SetValue(Canvas.LeftProperty, 200);
			//button1.SetValue(Canvas.TopProperty, 200);
			//button2.SetValue(Canvas.RightProperty, 5);
			//button2.SetValue(Canvas.BottomProperty, 5);

			//canvas = new Canvas();

			var panel = new StackPanel();
			//panel.Orientation = Orientation.Horizontal;

			panel.Children.Add(button1);
			panel.Children.Add(button2);
			panel.Children.Add(button3);
			Content = panel;

			var resourceBinding = new ResourceBinding<Style>("MyStyle");
			button1.SetResourceBinding(StyleProperty, resourceBinding);

			button1.SetBinding(ContentProperty, binding1);
			button2.SetBinding(ContentProperty, binding2);
			button3.SetBinding(ContentProperty, binding3);
		}

		//public void AddButton()
		//{
		//	canvas.Children.Add(button1);
		//}

		//public void RemoveButton()
		//{
		//	canvas.Children.Remove(button1);
		//}
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

		private HelloWorldView _view;
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
			_view = new HelloWorldView(uiContext.SharedAssets, _viewModel);
			uiContext.Add(_view);
		}

		/// <summary>
		///   Invoked when the application should update the its state.
		/// </summary>
		protected override void Update()
		{
			_localServer.Update();
			//_stateManager.Update();
			_viewModel.Update();

			if (_viewModel.FrameCount % 300 == 0)
				_view.button1.IsMouseOver = !_view.button1.IsMouseOver;

			if (_viewModel.FrameCount % 3000 == 0)
			{
				//_view.ViewModel = _view.ViewModel == null ? _viewModel : null;
				if (_viewModel.FrameCount % 6000 == 0)
				{
					Log.Info("Adding button: {0}", DateTime.Now.ToLongTimeString());
					//_view.AddButton();
				}

				else
				{
					Log.Info("Removing button: {0}", DateTime.Now.ToLongTimeString());
					//_view.RemoveButton();
				}
			}
			if ((_viewModel.FrameCount - 1500) % 3000 == 0)
			{
				Log.Info("Changing VM: {0}", DateTime.Now.ToLongTimeString());
				_view.ViewModel = new HelloWorldViewModel() { Name = DateTime.Now.ToLongTimeString() };
			}
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