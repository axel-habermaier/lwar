using System;

namespace Lwar.Client.Rendering
{
	using Assets.Effects;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders a skybox.
	/// </summary>
	public class SkyboxRenderer : DisposableObject
	{
		/// <summary>
		///   The skybox model.
		/// </summary>
		private readonly Model _model;

		/// <summary>
		///   The effect that is used to draw the skybox.
		/// </summary>
		private readonly SkyboxEffect  _effect;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public SkyboxRenderer(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			_model = Model.CreateSkybox(graphicsDevice);
			_effect = new SkyboxEffect(graphicsDevice, assets)
			{
				Skybox = new CubeMapView(assets.LoadCubeMap("Textures/Space"), SamplerState.BilinearClampNoMipmaps)
			};
		}

		/// <summary>
		///   Draws the skybox.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			_model.Draw(output, _effect.Default);
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