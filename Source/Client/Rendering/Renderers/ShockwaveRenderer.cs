using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders shockwaves into a 3D scene.
	/// </summary>
	public class ShockwaveRenderer : Renderer<Shockwave>
	{
		/// <summary>
		///   The effect that is used to draw the shockwaves.
		/// </summary>
		private SphereEffect _effect;

		/// <summary>
		///   The model that is used to draw the shockwaves.
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

			var texture = assets.LoadCubeMap("Textures/SunHeat");

			_model = Model.CreateSphere(graphicsDevice, 1, 10);
			_effect = new SphereEffect(graphicsDevice, assets) { SphereTexture = new CubeMapView(texture, SamplerState.BilinearClampNoMipmaps) };
		}

		/// <summary>
		///   Draws all shockwaves.
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
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_effect.SafeDispose();
			_model.SafeDispose();
		}
	}
}