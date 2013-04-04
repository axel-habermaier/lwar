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
	///   Renders planets into a 3D scene.
	/// </summary>
	public class PlanetRenderer : Renderer<Planet, PlanetRenderer.PlanetDrawState>
	{
		/// <summary>
		///   The effect that is used to draw the planets.
		/// </summary>
		private readonly SphereEffect _effect;

		/// <summary>
		///   The planet model.
		/// </summary>
		private readonly Model _model;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public PlanetRenderer(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			_model = Model.CreateSphere(graphicsDevice, 100, 25);
			_effect = new SphereEffect(graphicsDevice, assets)
			{
				SphereTexture = new CubeMapView(assets.LoadCubeMap("Textures/Sun"), SamplerState.TrilinearClamp)
			};
		}

		/// <summary>
		///   Invoked when an element has been added to the renderer.
		/// </summary>
		/// <param name="planet">The element that should be drawn by the renderer.</param>
		protected override PlanetDrawState OnAdded(Planet planet)
		{
			return new PlanetDrawState { Transform = planet.Transform };
		}

		/// <summary>
		///   Draws all registered elements.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			foreach (var planet in RegisteredElements)
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

		/// <summary>
		///   The state required for drawing a planet.
		/// </summary>
		public struct PlanetDrawState
		{
			/// <summary>
			///   The transformation of the planet.
			/// </summary>
			public Transformation Transform;
		}
	}
}