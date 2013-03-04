using System;

namespace Lwar.Client.Rendering
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;

	/// <summary>
	///   Represents a GPU-based gaussian blur filter than can be applied to a texture.
	/// </summary>
	public class GaussianBlur
	{
		/// <summary>
		///   The minimum size of the temporary textures.
		/// </summary>
		private const int MinimumSize = 16;

		/// <summary>
		///   The temporary render targets that are required to blur the input texture.
		/// </summary>
		private readonly RenderTarget[] _renderTargets;

		/// <summary>
		///   The temporary textures that are required to blur the input texture.
		/// </summary>
		private readonly Texture2D[] _textures;

		/// <summary>
		///   The full-screen quad that is used to blur the textures.
		/// </summary>
		private FullscreenQuad _fullscreenQuad;

		/// <summary>
		///   The fragment shader that applies the horizontal blur.
		/// </summary>
		private FragmentShader _horizontalBlurShader;

		/// <summary>
		///   The size of the input texture that is blurred.
		/// </summary>
		private int _textureSize;

		/// <summary>
		///   The fragment shader that applies the vertical blur.
		/// </summary>
		private FragmentShader _verticalBlurShader;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to apply the blur effect.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		/// <param name="textureSize">The size of the texture that should be blurred.</param>
		public GaussianBlur(GraphicsDevice graphicsDevice, AssetsManager assets, int textureSize)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);
			Assert.ArgumentInRange(textureSize, () => textureSize, 32, 1024);

			_textureSize = textureSize;
			_verticalBlurShader = assets.LoadFragmentShader("Shaders/VerticalBlurFS");
			_horizontalBlurShader = assets.LoadFragmentShader("Shaders/HorizontalBlurFS");
			_fullscreenQuad = new FullscreenQuad(graphicsDevice, assets);

			var size = textureSize;
			var count = 0;
			while (size > 0)
			{
				++count;
				size /= 2;

				if (size == MinimumSize)
					break;
			}

			Assert.That(size == MinimumSize, "Only power-of-two textures can be blurred.");
			_textures = new Texture2D[count * 2];
			_renderTargets = new RenderTarget[count * 2];

			size = textureSize;
			for (var i = 0; i < count; ++i)
			{
				_textures[2 * i] = new Texture2D(graphicsDevice, size, size, SurfaceFormat.Rgba16F, TextureFlags.RenderTarget);
				_textures[2 * i + 1] = new Texture2D(graphicsDevice, size, size, SurfaceFormat.Rgba16F, TextureFlags.RenderTarget);
			}

			for (var i = 0; i < _renderTargets.Length; ++i)
				_renderTargets[i] = new RenderTarget(graphicsDevice, new[] { _textures[i] }, null);
		}
	}
}