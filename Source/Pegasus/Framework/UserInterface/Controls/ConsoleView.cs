namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Platform.Graphics;
	using Platform.Memory;
	using Rendering;
	using Console = Rendering.UserInterface.Console;

	partial class ConsoleView
	{
		internal ConsoleView(Console console)
		{
			InitializeComponents();

			_consolePanel.Console = console;
		}

		/// <summary>
		///     Invoked when the UI element is attached to a new logical parent.
		/// </summary>
		protected override void OnAttached()
		{
			_consolePanel.Camera = new Camera2D(Application.Current.GraphicsDevice);
			_consolePanel.SpriteBatch = new SpriteBatch(Application.Current.GraphicsDevice, Application.Current.Assets)
			{
				BlendState = BlendState.Premultiplied,
				DepthStencilState = DepthStencilState.DepthDisabled,
				SamplerState = SamplerState.PointClampNoMipmaps
			};
		}

		/// <summary>
		///     Invoked when the UI element has been detached from its current logical parent.
		/// </summary>
		protected override void OnDetached()
		{
			_consolePanel.Camera.SafeDispose();
			_consolePanel.SpriteBatch.SafeDispose();
		}
	}

	internal class ConsoleOutputPanel : RenderOutputPanel
	{
		public Camera2D Camera;
		public Console Console;
		public SpriteBatch SpriteBatch;

		protected override void Draw(RenderOutput renderOutput)
		{
			renderOutput.Camera = Camera;

			renderOutput.ClearColor(new Color());

			Camera.Viewport = renderOutput.Viewport;
			Console.Update(renderOutput.Viewport.Size);
			Console.Draw(SpriteBatch);

			SpriteBatch.DrawBatch(renderOutput);
		}
	}
}