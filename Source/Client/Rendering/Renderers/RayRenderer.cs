using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders rays into a 3D scene.
	/// </summary>
	public class RayRenderer : Renderer<Ray>
	{
		/// <summary>
		///   The effect that is used to draw the rays.
		/// </summary>
		private TexturedQuadEffect _effect;

		/// <summary>
		///   The model that is used to draw the rays.
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

			var texture = assets.LoadTexture2D("Textures/Phaser");

			_model = Model.CreateQuad(graphicsDevice, texture.Size, new Vector2(texture.Size.Width / 2.0f, 0));
			_effect = new TexturedQuadEffect(graphicsDevice, assets) { Texture = new Texture2DView(texture, SamplerState.BilinearClampNoMipmaps) };
		}

		/// <summary>
		///   Draws all rays.
		/// </summary>
		/// <param name="output">The output that the phasers should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			foreach (var ray in Elements)
			{
				var matrix =  Matrix.CreateScale(ray.Length, 1, 1);

				_effect.World = matrix * ray.Transform.Matrix;
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
	}
}