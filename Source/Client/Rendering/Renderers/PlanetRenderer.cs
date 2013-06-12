using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Framework;
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
				_effect.SphereTexture = new CubeMapView(planet.Template.CubeMap, SamplerState.TrilinearClamp);

				planet.Template.Model.Draw(output, _effect.Default);
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