using System;

namespace Lwar.Client.Rendering
{
	using Assets.Effects;
	using Gameplay;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders bullets into a 3D scene.
	/// </summary>
	public class BulletRenderer : Renderer<Bullet, BulletRenderer.BulletDrawState>
	{
		/// <summary>
		///   The effect that is used to draw the bullets.
		/// </summary>
		private readonly TexturedQuadEffect _effect;

		/// <summary>
		///   The model that is used to draw the bullets.
		/// </summary>
		private readonly Model _model;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public BulletRenderer(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			var texture = assets.LoadTexture2D("Textures/Bullet");

			_model = Model.CreateQuad(graphicsDevice, texture.Size);
			_effect = new TexturedQuadEffect(graphicsDevice, assets)
			{
				Texture = new Texture2DView(texture, SamplerState.TrilinearClamp)
			};
		}

		/// <summary>
		///   Invoked when an element has been added to the renderer.
		/// </summary>
		/// <param name="bullet">The element that should be drawn by the renderer.</param>
		protected override BulletDrawState OnAdded(Bullet bullet)
		{
			return new BulletDrawState { Transform = bullet.Transform };
		}

		/// <summary>
		///   Draws all registered elements.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			foreach (var bullet in RegisteredElements)
			{
				_effect.World = bullet.Transform.Matrix;
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
		///   The state required for drawing a bullet.
		/// </summary>
		public struct BulletDrawState
		{
			/// <summary>
			///   The transformation of the bullet.
			/// </summary>
			public Transformation Transform;
		}
	}
}