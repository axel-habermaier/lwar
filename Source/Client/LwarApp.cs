using System;

namespace Client
{
	using System.Collections.Generic;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;

	internal sealed class LwarApp : App
	{
		/// <summary>
		///   The current game session.
		/// </summary>
		private readonly GameSession _session = new GameSession();

		/// <summary>
		///   Invoked when the application should update the game state.
		/// </summary>
		protected override void Update()
		{
			_session.Update();
		}

		/// <summary>
		///   Invoked when the application should draw a frame.
		/// </summary>
		protected override void Draw()
		{
			SwapChain.BackBuffer.Bind();
			RasterizerState.CullNone.Bind();
			SwapChain.BackBuffer.Clear(ClearTargets.Color, Color.Black);

			_session.Draw();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_session.SafeDispose();

			base.OnDisposing();
		}

		/// <summary>
		///   Invoked when the application is initializing.
		/// </summary>
		protected override void Initialize()
		{
			UpdatesPerSecond = 60;

			Window.Closing += Exit;
			Window.Resized += s => GraphicsDevice.SetViewport(new Rectangle(0, 0, s.Width, s.Height));
			Window.Title = "lwar";
			Window.Size = new Size(1280, 720);

			AssetsLoader.Load(Assets);

			_session.Window = Window;
			_session.GraphicsDevice = GraphicsDevice;
			_session.InputDevice = LogicalInputDevice;
			_session.Players = new List<Player> { new Player() };
			_session.Initialize();
		}

		/// <summary>
		///   Entry point of the application.
		/// </summary>
		/// <param name="args">The arguments the application was started with.</param>
		private static void Main(string[] args)
		{
			using (var app = new LwarApp())
				app.Run();
		}
	}
}