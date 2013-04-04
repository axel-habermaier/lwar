using System;

namespace Lwar.Client.Rendering
{
	using Assets.Effects;
	using Gameplay;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders ships into a 3D scene.
	/// </summary>
	public class ShipRenderer : Renderer<Ship, ShipRenderer.ShipDrawState>
	{
		/// <summary>
		///   The effect that is used to draw the ships.
		/// </summary>
		private readonly TexturedQuadEffect _effect;

		/// <summary>
		///   The ship model.
		/// </summary>
		private readonly Model _model;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public ShipRenderer(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			var texture = assets.LoadTexture2D("Textures/Ship");
			_model = Model.CreateQuad(graphicsDevice, texture.Size);
			_effect = new TexturedQuadEffect(graphicsDevice, assets)
			{
				Texture = new Texture2DView(texture, SamplerState.TrilinearClamp)
			};
		}

		/// <summary>
		///   Invoked when an element has been added to the renderer.
		/// </summary>
		/// <param name="ship">The element that should be drawn by the renderer.</param>
		protected override ShipDrawState OnAdded(Ship ship)
		{
			return new ShipDrawState { Transform = ship.Transform };
		}

		/// <summary>
		///   Draws all registered elements.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			foreach (var ship in RegisteredElements)
			{
				_effect.World = ship.Transform.Matrix;
				_model.Draw(output, _effect.Default);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_effect.SafeDispose();
			_model.SafeDispose();
		}

		/// <summary>
		///   The state required for drawing a ship.
		/// </summary>
		public struct ShipDrawState
		{
			/// <summary>
			///   The transformation of the bullet.
			/// </summary>
			public Transformation Transform;
		}
	}
}