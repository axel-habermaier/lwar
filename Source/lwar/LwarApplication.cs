using System;

namespace Lwar
{
	using System.Net;
	using Assets;
	using Network;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Controls;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Input;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Pegasus.Rendering.UserInterface;
	using Screens;
	using Scripting;

	/// <summary>
	///   Represents the lwar application.
	/// </summary>
	internal sealed class LwarApplication : Application
	{
		/// <summary>
		///   The local game server that can be used to hosts game sessions locally.
		/// </summary>
		private LocalServer _localServer;

		/// <summary>
		///   The state manager that manages the states of the application.
		/// </summary>
		private ScreenManager _stateManager;

		private Button b;

		private Button b2;
		private Layout l, l2;
		private TestViewModel vm;

		/// <summary>
		///   Invoked when the application is initializing.
		/// </summary>
		protected override void Initialize()
		{
			Commands.Resolve();
			Cvars.Resolve();

			Commands.OnConnect += Connect;
			Commands.OnDisconnect += Disconnect;

			Context.InputDevice.ActivateLayer(InputLayers.Game);
			Context.Window.Closing += Exit;

			_localServer = new LocalServer();
			_stateManager = new ScreenManager(Context);
			_stateManager.Add(new MainMenu());

			Commands.Bind(Key.F1.WentDown(), "start_server");
			Commands.Bind(Key.F2.WentDown(), "stop_server");
			Commands.Bind(Key.F3.WentDown(), "connect 127.0.0.1");
			Commands.Bind(Key.F4.WentDown(), "disconnect");
			Commands.Bind(Key.F5.WentDown(), "reload_assets");
			Commands.Bind(Key.C.WentDown(), "toggle_debug_camera");
			Commands.Bind(Key.Escape.WentDown(), "exit");
			Commands.Bind(Key.F9.WentDown(), "toggle show_platform_info");
			Commands.Bind(Key.F10.WentDown(), "toggle show_frame_stats");

			var s = new Style();
			s.Setters.Add(new Setter<Color>(UIElement.ForegroundProperty, Color.FromRgba(255, 0, 0, 255)));
			var t = new Trigger<bool>(UIElement.IsMouseOverProperty, true);
			t.Setters.Add(new Setter<Color>(UIElement.ForegroundProperty, Color.FromRgba(0, 255, 123, 255)));
			s.Triggers.Add(t);

			l = new Layout();
			l2 = new Layout();

			b = new Button { Width = 100, Height = 200 };
			l.Children.Add(b);
			l.Resources.Add("myStyle", s);
			l2.Resources.Add("myStyle", new Style());
			var rb = new ResourceBinding<Style>("myStyle");
			b.SetResourceBinding(UIElement.StyleProperty, rb);

			b2 = new Button() { ViewModel = vm = new TestViewModel(), Width = 300, Height = 100 };
			b.Content = "blub";
			l.Children.Add(b2);

			vm.Rank = 17;
			vm.A = new TestViewModel();
			vm.A.B = new TestViewModel();
			vm.A.Rank = 21;

			var rb2 = new ResourceBinding<Style>("myStyle");
			b2.SetResourceBinding(UIElement.StyleProperty, rb2);

			var binding = new Binding<object>(viewModel => ((TestViewModel)viewModel).A.Rank);
			b2.SetBinding(ContentControl.ContentProperty, binding);
		}

		/// <summary>
		///   Invoked when the application should update the its state.
		/// </summary>
		protected override void Update()
		{
			_localServer.Update();
			_stateManager.Update();

			//var vm = b.ViewModel as TestViewModel;
			vm.Rank++;
			vm.A.Rank++;

			if (vm.Rank % 500 == 0)
			{
				var s = new Style();
				s.Setters.Add(new Setter<Color>(UIElement.ForegroundProperty, Color.FromRgba(255, 52, 251, 255)));
				vm.A = new TestViewModel() { A = new TestViewModel() };
				//var r = new ResourceDictionary();
				//r.Add("myStyle", s);
				//b.Resources = r;
				b.IsMouseOver = !b.IsMouseOver;
			}
			if (vm.Rank % 1000 == 0)
			{
				l.Children.Remove(b2);
				l2.Children.Add(b2);
			}
			if (vm.Rank % 2000 == 0)
			{
				l.Resources["myStyle"] = new Style();
			}

			//if (vm.Rank % 3000 == 0)
			//b.ViewModel = new TestViewModel() { A = new TestViewModel{Rank = 9999999 }};
		}

		/// <summary>
		///   Invoked when the application should draw a frame.
		/// </summary>
		/// <param name="output">The output that the scene should be rendered to.</param>
		protected override void Draw(RenderOutput output)
		{
			output.ClearColor(new Color(0, 0, 0, 0));
			output.ClearDepth();

			_stateManager.Draw(output);
		}

		/// <summary>
		///   Invoked when the application should draw the user interface.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		protected override void DrawUserInterface(SpriteBatch spriteBatch)
		{
			_stateManager.DrawUserInterface(spriteBatch);

			spriteBatch.Layer = 17;
			b.Draw(spriteBatch, Context.Assets.LoadFont(Fonts.LiberationMono11));
			b2.Draw(spriteBatch, Context.Assets.LoadFont(Fonts.LiberationMono11));
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void Dispose()
		{
			Commands.OnConnect -= Connect;
			Commands.OnDisconnect -= Disconnect;

			_stateManager.SafeDispose();
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
			_stateManager.Add(new Level(new IPEndPoint(address, port)));
		}

		/// <summary>
		///   Disconnects from the game session the client is currently connected to.
		/// </summary>
		private void Disconnect()
		{
			_stateManager.Clear();
			_stateManager.Add(new MainMenu());
		}

		private class TestViewModel : ViewModel
		{
			//private static int _x;
			private int _rank;
			private TestViewModel a, b, c;

			public TestViewModel()
			{
				//_rank = ++_x;
			}

			public TestViewModel A
			{
				get { return a; }
				set { ChangePropertyValue(ref a, value); }
			}

			public TestViewModel B
			{
				get { return b; }
				set { ChangePropertyValue(ref b, value); }
			}

			public TestViewModel C
			{
				get { return c; }
				set { ChangePropertyValue(ref c, value); }
			}

			/// <summary>
			///   Rank of the local player
			/// </summary>
			public int Rank
			{
				get { return _rank; }
				set { ChangePropertyValue(ref _rank, value); }
			}
		}
	}
}