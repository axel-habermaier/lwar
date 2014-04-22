namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Pegasus;
	using Pegasus.Platform.Assets;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders a skybox.
	/// </summary>
	public class SkyboxRenderer : DisposableObject
	{
		/// <summary>
		///     The effect that is used to draw the skybox.
		/// </summary>
		private readonly SkyboxEffect _effect;

		/// <summary>
		///     The skybox model.
		/// </summary>
		private readonly Model _model;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public SkyboxRenderer(AssetsManager assets)
		{
			Assert.ArgumentNotNull(assets);

			var cubemap = assets.Load(Textures.SpaceCubemap);

			_model = Model.CreateSkybox();
			_effect = new SkyboxEffect( assets) { Skybox = new CubeMapView(cubemap, SamplerState.BilinearClampNoMipmaps) };
		}

		/// <summary>
		///     Draws the skybox.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			_model.Draw(output, _effect.Default);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_effect.SafeDispose();
			_model.SafeDispose();
		}
	}
}