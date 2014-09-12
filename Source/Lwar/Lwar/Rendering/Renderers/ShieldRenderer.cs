namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Actors;
	using Pegasus.Assets;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;

	/// <summary>
	///     Renders shield effects into a 3D scene.
	/// </summary>
	public class ShieldRenderer : Renderer<Shield>
	{
		/// <summary>
		///     The effect that is used to draw the shields.
		/// </summary>
		private SphereEffect _effect;

		/// <summary>
		///     The model that is used to draw the shields.
		/// </summary>
		private Model _model;

		private CubeMap _texture;

		/// <summary>
		///     Loads the required assets of the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Load(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			_effect = new SphereEffect(graphicsDevice, assets);
			_texture = assets.Load(Textures.ShieldsCubemap);
		}

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		public override void Initialize(GraphicsDevice graphicsDevice)
		{
			_model = Model.CreateSphere(graphicsDevice, 1.0f, 16);
			_effect.SphereTexture = new CubeMapView(_texture, SamplerState.TrilinearClamp);
		}

		/// <summary>
		///     Draws all registered 3D elements.
		/// </summary>
		/// <param name="output">The output that the elements should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			foreach (var shield in Elements)
			{
				_effect.World = Matrix.CreateScale(shield.Ship.Template.Radius) * shield.Transform.Matrix;
				_effect.ImpactPosition = new Vector3(shield.ImpactPosition.X, 0, shield.ImpactPosition.Y);
				_effect.TimeToLive = shield.TimeToLive;
				_model.Draw(output, _effect.Shield);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			_effect.SafeDispose();
			_model.SafeDispose();
		}
	}
}