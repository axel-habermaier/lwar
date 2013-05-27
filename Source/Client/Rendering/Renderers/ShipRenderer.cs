using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders ships into a 3D scene.
	/// </summary>
	public class ShipRenderer : Renderer<Ship>
	{
		/// <summary>
		///   The effect that is used to draw the ships.
		/// </summary>
		private TexturedQuadEffect _effect;

		/// <summary>
		///   The ship model.
		/// </summary>
		private Model _model;

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			var texture = assets.LoadTexture2D("Textures/Ship");

			_model = Model.CreateQuad(graphicsDevice, texture.Size);
			_effect = new TexturedQuadEffect(graphicsDevice, assets) { Texture = new Texture2DView(texture, SamplerState.TrilinearClamp) };
		}

		/// <summary>
		///   Draws all ships.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			foreach (var ship in Elements)
			{
				_effect.World = ship.Transform.Matrix;
				_model.Draw(output, _effect.TexturedQuad);
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
	}
}