namespace Lwar.Rendering
{
	using System;
	using System.Collections.Generic;
	using Assets;
	using Assets.Effects;
	using Pegasus.Assets;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a GPU-based gaussian blur filter than can be applied to a texture.
	/// </summary>
	internal class GaussianBlur : DisposableObject
	{
		/// <summary>
		///     The minimum size of the temporary textures.
		/// </summary>
		private const int MinimumSize = 16;

		private readonly BlurEffect _blurEffect;

		/// <summary>
		///     The full-screen quad that is used to blur the textures.
		/// </summary>
		private readonly FullscreenQuad _fullscreenQuad;

		private readonly RenderOutput _output;
		private readonly TexturedQuadEffect _quadEffect;

		/// <summary>
		///     The temporary render targets that are required to blur the input texture.
		/// </summary>
		private readonly RenderTarget[] _renderTargets;

		private readonly Size[] _sizes;

		/// <summary>
		///     The input texture that is blurred.
		/// </summary>
		private readonly Texture2D _texture;

		private readonly Texture2D[] _textures;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="renderContext">The graphics device that should be used to apply the blur effect.</param>
		/// <param name="texture">The texture that should be blurred.</param>
		public GaussianBlur(RenderContext renderContext, Texture2D texture)
		{
			Assert.ArgumentNotNull(renderContext);
			Assert.ArgumentNotNull(texture);
			Assert.InRange(texture.Width, MinimumSize, 2048);

			_texture = texture;
			_fullscreenQuad = new FullscreenQuad(renderContext.GraphicsDevice);

			var width = texture.Width;
			var height = texture.Height;
			var count = 0;
			var sizes = new List<Size> { texture.Size };
			while (width > MinimumSize && height > MinimumSize)
			{
				++count;
				width /= 2;
				height /= 2;
				sizes.Add(new Size(width, height));
			}
			_sizes = sizes.ToArray();

			++count;
			_textures = new Texture2D[count];
			_renderTargets = new RenderTarget[count];

			width = texture.Width;
			height = texture.Height;
			for (var i = 0; i < count; ++i)
			{
				_textures[i] = new Texture2D(renderContext.GraphicsDevice, width, height, texture.Format, TextureFlags.RenderTarget);

				width /= 2;
				height /= 2;
			}

			for (var i = 0; i < _renderTargets.Length; ++i)
				_renderTargets[i] = new RenderTarget(renderContext.GraphicsDevice, null, _textures[i]);

			var assetBundle = renderContext.GetAssetBundle<GameBundle>();

			_output = new RenderOutput(renderContext);
			_blurEffect = assetBundle.BlurEffect;
			_quadEffect = assetBundle.TexturedQuadEffect;
		}

		/// <summary>
		///     Blurs the input texture.
		/// </summary>
		public Texture2D Blur(RenderOutput output)
		{
			output.RenderContext.DepthStencilStates.DepthDisabled.Bind();
			output.RenderContext.BlendStates.Opaque.Bind();
			_blurEffect.Texture = new Texture2DView(_texture, output.RenderContext.SamplerStates.BilinearClampNoMipmaps);
			_output.Camera = output.Camera;

			var i = 0;
			foreach (var size in _sizes)
			{
				_output.RenderTarget = _renderTargets[i];
				_output.Viewport = new Rectangle(Vector2.Zero, size);
				_blurEffect.ViewportSize = size;

				_fullscreenQuad.Draw(_output, _blurEffect.Gaussian);

				_blurEffect.Texture = new Texture2DView(_textures[i], output.RenderContext.SamplerStates.BilinearClampNoMipmaps);
				++i;
			}

			--i;
			--i;

			_output.Viewport = new Rectangle(Vector2.Zero, _sizes[0]);
			_output.RenderTarget = _renderTargets[0];
			_output.ClearColor(new Color(0, 0, 0, 0));
			output.RenderContext.BlendStates.Premultiplied.Bind();
			while (i >= 0)
			{
				_quadEffect.Texture = new Texture2DView(_textures[i + 1], output.RenderContext.SamplerStates.BilinearClampNoMipmaps);
				_fullscreenQuad.Draw(_output, _quadEffect.FullScreen);

				--i;
			}

			return _textures[0];
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_fullscreenQuad.SafeDispose();
			_renderTargets.SafeDisposeAll();
			_textures.SafeDisposeAll();
		}
	}
}