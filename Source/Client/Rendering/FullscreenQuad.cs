using System;

namespace Lwar.Client.Rendering
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;

	/// <summary>
	///   Represents a textured full-screen quad.
	/// </summary>
	public class FullscreenQuad : DisposableObject
	{
		/// <summary>
		///   The fragment shader that is used to draw the quad.
		/// </summary>
		private readonly FragmentShader _fragmentShader;

		/// <summary>
		///   The full screen quad model.
		/// </summary>
		private readonly Model _model;

		/// <summary>
		///   The vertex shader that is used to draw the quad.
		/// </summary>
		private readonly VertexShader _vertexShader;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the quad.</param>
		/// <param name="assets">The assets manager that should be used to load the quad resources.</param>
		public FullscreenQuad(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			_model = Model.CreateQuad(graphicsDevice, 2, 2);
			_vertexShader = assets.LoadVertexShader("Shaders/FullscreenQuadVS");
			_fragmentShader = assets.LoadFragmentShader("Shaders/QuadFS");
		}

		/// <summary>
		///   Draws the full-screen quad with the given texture.
		/// </summary>
		/// <param name="texture">The texture the full-screen quad should be drawn with.</param>
		public void Draw(Texture2D texture)
		{
			SamplerState.BilinearClamp.Bind(0);
			texture.Bind(0);

			_vertexShader.Bind();
			_fragmentShader.Bind();

			_model.Draw();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_model.SafeDispose();
		}
	}
}