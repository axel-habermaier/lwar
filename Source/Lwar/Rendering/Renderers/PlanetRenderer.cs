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
		///     Initializes the renderer.
		/// </summary>
		protected override void Initialize()
		{
			_planetEffect = new SphereEffect(GraphicsDevice, Assets);
			_trajectoryEffect = new SimpleVertexEffect(GraphicsDevice, Assets);
		}

		/// <summary>
		///     Draws all planets.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			if (_trajectories == null)
				GenerateTrajectories();

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
		///     Generates the trajectories of the planets.
		/// </summary>
		private void GenerateTrajectories()
		{
			var outline = new CircleOutline();
			foreach (var planet in Elements)
				outline.Add(planet.Position.Length, TrajectoryPrecision, TrajectoryWidth);

			_trajectories = outline.ToModel(GraphicsDevice);
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