namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Assets;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders shockwaves into a 3D scene.
	/// </summary>
	public class ShockwaveRenderer : Renderer<Shockwave>
	{
		/// <summary>
		///     The effect that is used to draw the shockwaves.
		/// </summary>
		private SphereEffect _effect;

		/// <summary>
		///     The model that is used to draw the shockwaves.
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
			_texture = assets.Load(Textures.SunHeatCubemap);
			_effect = new SphereEffect(graphicsDevice, assets);
		}

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		public override void Initialize(GraphicsDevice graphicsDevice)
		{
			_model = Model.CreateSphere(graphicsDevice, 1, 10);
			_effect.SphereTexture = new CubeMapView(_texture, SamplerState.BilinearClampNoMipmaps);
		}

		/// <summary>
		///     Draws all shockwaves.
		/// </summary>
		/// <param name="output">The output that the shockwaves should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			foreach (var shockwave in Elements)
			{
				_effect.World = Matrix.CreateScale(shockwave.Radius) * shockwave.Transform.Matrix;
				_model.Draw(output, _effect.Default);
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