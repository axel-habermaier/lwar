using System;

namespace Lwar.Client.Rendering
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Graphics;

	/// <summary>
	///   Renders planets into a 3D scene.
	/// </summary>
	public struct PlanetRenderer
	{
		/// <summary>
		///   The vertex shader that is used to draw the planets.
		/// </summary>
		[Asset("Shaders/SphereVS")]
		public static VertexShader VertexShader;

		/// <summary>
		///   The fragment shader that is used to draw the planets.
		/// </summary>
		[Asset("Shaders/SphereFS")]
		public static FragmentShader FragmentShader;

		/// <summary>
		///   The context in which the planets are rendered.
		/// </summary>
		private readonly RenderContext _context;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="context">The context in which the planet should be rendered.</param>
		public PlanetRenderer(RenderContext context)
		{
			Assert.ArgumentNotNull(context, () => context);
			_context = context;
		}

		/// <summary>
		///   Draws the planet.
		/// </summary>
		/// <param name="planet">The planet that should be drawn.</param>
		public void Draw(Planet planet)
		{
			Assert.ArgumentNotNull(planet, () => planet);

			_context.UpdateWorldTransform(planet);
			VertexShader.Bind();
			FragmentShader.Bind();
			Planet.Texture.Bind(0);
			SamplerState.BilinearClamp.Bind(0);

			planet.Model.Draw();
		}
	}
}