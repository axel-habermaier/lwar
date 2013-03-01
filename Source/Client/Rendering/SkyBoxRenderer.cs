using System;

namespace Lwar.Client.Rendering
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;

	/// <summary>
	///   Renders a skybox.
	/// </summary>
	public class SkyBoxRenderer : DisposableObject
	{
		/// <summary>
		///   The skybox cube map.
		/// </summary>
		private readonly CubeMap _cubeMap;

		/// <summary>
		///   The fragment shader that is used to draw the ships.
		/// </summary>
		private readonly FragmentShader _fragmentShader;

		/// <summary>
		///   The skybox model.
		/// </summary>
		private readonly Model _model;

		/// <summary>
		///   The bilinear, no-mipmaps texture sampler for the skybox texture.
		/// </summary>
		private readonly SamplerState _sampler;

		/// <summary>
		///   The vertex shader that is used to draw the ships.
		/// </summary>
		private readonly VertexShader _vertexShader;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public SkyBoxRenderer(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			_vertexShader = assets.LoadVertexShader("Shaders/SkyboxVS");
			_fragmentShader = assets.LoadFragmentShader("Shaders/SkyboxFS");
			_model = Model.CreateSkyBox(graphicsDevice);
			_cubeMap = assets.LoadCubeMap("Textures/Space");

			_sampler = new SamplerState(graphicsDevice) { Filter = TextureFilter.BilinearNoMipmaps };
		}

		/// <summary>
		///   Draws the skybox.
		/// </summary>
		public void Draw()
		{
			_vertexShader.Bind();
			_fragmentShader.Bind();
			_sampler.Bind(0);
			_cubeMap.Bind(0);

			_model.Draw();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_sampler.SafeDispose();
			_model.SafeDispose();
		}
	}
}