namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders a skybox.
	/// </summary>
	internal class SkyboxRenderer : DisposableObject, IRenderer
	{
		/// <summary>
		///     The skybox cube map.
		/// </summary>
		private CubeMap _cubeMap;

		/// <summary>
		///     The effect that is used to draw the skybox.
		/// </summary>
		private SkyboxEffect _effect;

		/// <summary>
		///     The skybox model.
		/// </summary>
		private Model _model;

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for drawing.</param>
		/// <param name="assets">The asset bundle that provides access to Lwar assets.</param>
		public void Initialize(RenderContext renderContext, GameBundle assets)
		{
			_cubeMap = assets.Space;
			_effect = assets.SkyboxEffect;

			_model = Model.CreateSkybox(renderContext.GraphicsDevice);
			_effect.Skybox = new CubeMapView(_cubeMap, renderContext.SamplerStates.BilinearClampNoMipmaps);
		}

		/// <summary>
		///     Draws the skybox.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			output.RenderContext.BlendStates.Premultiplied.Bind();
			output.RenderContext.RasterizerStates.CullCounterClockwise.Bind();

			_effect.ViewportSize = output.Viewport.Size;
			_model.Draw(output, _effect.Default);
		}

		/// <summary>
		///     Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_model.SafeDispose();
		}
	}
}