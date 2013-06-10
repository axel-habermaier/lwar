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
	///   Renders planets into a 3D scene.
	/// </summary>
	public class PlanetRenderer : Renderer<Planet>
	{
		/// <summary>
		///   The effect that is used to draw the planets.
		/// </summary>
		private SphereEffect _effect;

		/// <summary>
		///   The planet model.
		/// </summary>
		private Model _model;

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(assets);

			var cubemap = assets.LoadCubeMap("Textures/Planet");

			_model = Model.CreateSphere(graphicsDevice, Planet.Radius, 15);
			_effect = new SphereEffect(graphicsDevice, assets) { SphereTexture = new CubeMapView(cubemap, SamplerState.TrilinearClamp) };
		}

		/// <summary>
		///   Draws all planets.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			BlendState.Premultiplied.Bind();
			DepthStencilState.DepthEnabled.Bind();

			foreach (var planet in Elements)
			{
				_effect.World = planet.Transform.Matrix;
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