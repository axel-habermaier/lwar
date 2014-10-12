namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Assets;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders planets into a 3D scene.
	/// </summary>
	public class PlanetRenderer : Renderer<Planet>
	{
		/// <summary>
		///     The width of the planet trajectories.
		/// </summary>
		private const int TrajectoryWidth = 10;

		/// <summary>
		///     The precision of the planet trajectories.
		/// </summary>
		private const int TrajectoryPrecision = 250;

		/// <summary>
		///     The color of the planet trajectories.
		/// </summary>
		private static readonly Vector4 TrajectoryColor = new Vector4(0.07f, 0.07f, 0.07f, 0.07f);

		/// <summary>
		///     The effect that is used to draw the planets.
		/// </summary>
		private SphereEffect _planetEffect;

		/// <summary>
		///     The model that stores the planet trajectories.
		/// </summary>
		private Model _trajectories;

		/// <summary>
		///     The effect that is used to draw the planet trajectories.
		/// </summary>
		private SimpleVertexEffect _trajectoryEffect;

		/// <summary>
		///     Loads the required assets of the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Load(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			_planetEffect = new SphereEffect(graphicsDevice, assets);
			_trajectoryEffect = new SimpleVertexEffect(graphicsDevice, assets);
		}

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		public override void Initialize(GraphicsDevice graphicsDevice)
		{
			var outline = new CircleOutline();
			foreach (var planet in Elements)
				outline.Add(planet.Position.Length, TrajectoryPrecision, TrajectoryWidth);

			_trajectories = outline.ToModel(graphicsDevice);
		}

		/// <summary>
		///     Draws all planets.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			BlendState.Premultiplied.Bind();
			DepthStencilState.DepthEnabled.Bind();

			foreach (var planet in Elements)
			{
				_planetEffect.World = planet.Transform.Matrix;
				_planetEffect.SunPosition = new Vector3(0, 0.5f, 0);
				_planetEffect.SphereTexture = new CubeMapView(planet.Template.CubeMap, SamplerState.TrilinearClamp);

				planet.Template.Model.Draw(output, _planetEffect.Planet);
			}

			_trajectoryEffect.Color = TrajectoryColor;
			_trajectories.Draw(output, _trajectoryEffect.Default);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			_planetEffect.SafeDispose();
			_trajectoryEffect.SafeDispose();
			_trajectories.SafeDispose();
		}
	}
}