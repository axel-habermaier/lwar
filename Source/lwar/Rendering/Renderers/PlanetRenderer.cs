namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

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
		///   Initializes the renderer.
		/// </summary>
		protected override void Initialize()
		{
			_effect = new SphereEffect(GraphicsDevice, Assets);
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
				_effect.SunPosition = new Vector3(0, 0.5f, 0);
				_effect.SphereTexture = new CubeMapView(planet.Template.CubeMap, SamplerState.TrilinearClamp);

				planet.Template.Model.Draw(output, _effect.Planet);
			}
		}

		/// <summary>
		///   Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			// Draw the planet's orbit
			foreach (var planet in Elements)
			{
				// The distance to the sun - assumes the sun lies at the origin
				var distance = planet.Position.Length;

				spriteBatch.DrawOutline(new CircleF(Vector2.Zero, distance), new Color(32, 32, 32, 64), 12, 200);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			_effect.SafeDispose();
		}
	}
}