using System;

namespace Lwar.Client.Rendering
{
	using System.Collections.Generic;
	using Assets.Effects;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Represents a GPU-based gaussian blur filter than can be applied to a texture.
	/// </summary>
	public class GaussianBlur : DisposableObject
	{
		/// <summary>
		///   The minimum size of the temporary textures.
		/// </summary>
		private const uint MinimumSize = 16;

		private readonly BlurEffect _blurEffect;

		/// <summary>
		///   The full-screen quad that is used to blur the textures.
		/// </summary>
		private readonly FullscreenQuad _fullscreenQuad;

		private readonly RenderOutput _output;
		private readonly TexturedQuadEffect _quadEffect;

		/// <summary>
		///   The temporary render targets that are required to blur the input texture.
		/// </summary>
		private readonly RenderTarget[] _renderTargets;

		private readonly Size[] _sizes;

		/// <summary>
		///   The input texture that is blurred.
		/// </summary>
		private readonly Texture2D _texture;

		private readonly Texture2D[] _textures;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to apply the blur effect.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		/// <param name="texture">The texture that should be blurred.</param>
		public GaussianBlur(GraphicsDevice graphicsDevice, AssetsManager assets, Texture2D texture)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);
			Assert.ArgumentNotNull(texture, () => texture);
			Assert.InRange(texture.Width, MinimumSize, 2048u);

			_texture = texture;
			_fullscreenQuad = new FullscreenQuad(graphicsDevice, assets);

			var width = texture.Width;
			var height = texture.Height;
			var count = 0;
			var sizes = new List<Size> { texture.Size };
			while (width > MinimumSize && height > MinimumSize)
			{
				++count;
				width /= 2;
				height /= 2;
				sizes.Add(new Size((int)width, (int)height));
			}
			_sizes = sizes.ToArray();

			++count;
			_textures = new Texture2D[count];
			_renderTargets = new RenderTarget[count];

			width = texture.Width;
			height = texture.Height;
			for (var i = 0; i < count; ++i)
			{
				_textures[i] = new Texture2D(graphicsDevice, width, height, texture.Format, TextureFlags.RenderTarget);

				width /= 2;
				height /= 2;
			}

			for (var i = 0; i < _renderTargets.Length; ++i)
				_renderTargets[i] = new RenderTarget(graphicsDevice, null, _textures[i]);

			_output = new RenderOutput(graphicsDevice);
			_blurEffect = new BlurEffect(graphicsDevice, assets);
			_quadEffect = new TexturedQuadEffect(graphicsDevice, assets);
		}

		/// <summary>
		///   Blurs the input texture.
		/// </summary>
		public Texture2D Blur(RenderOutput output)
		{
			DepthStencilState.DepthDisabled.Bind();
			BlendState.Opaque.Bind();
			_blurEffect.Texture = new Texture2DView(_texture, SamplerState.BilinearClampNoMipmaps);
			_output.Camera = output.Camera;

			var i = 0;
			foreach (var size in _sizes)
			{
				_output.RenderTarget = _renderTargets[i];
				_output.Viewport = new Rectangle(Vector2i.Zero, size);

				_fullscreenQuad.Draw(_output, _blurEffect.Gaussian);

				_blurEffect.Texture = new Texture2DView(_textures[i], SamplerState.BilinearClampNoMipmaps);
				++i;
			}

			--i;
			--i;

			_output.Viewport = new Rectangle(Vector2i.Zero, _sizes[0]);
			_output.RenderTarget = _renderTargets[0];
			_output.ClearColor(new Color(0, 0, 0, 0));
			BlendState.Premultiplied.Bind();
			while (i >= 0)
			{
				_quadEffect.Texture = new Texture2DView(_textures[i + 1], SamplerState.BilinearClampNoMipmaps);
				_fullscreenQuad.Draw(_output, _quadEffect.FullScreen);

				--i;
			}

			return _textures[0];
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_fullscreenQuad.SafeDispose();
			_renderTargets.SafeDisposeAll();
			_textures.SafeDisposeAll();
			_blurEffect.SafeDispose();
			_quadEffect.SafeDispose();
			_output.SafeDispose();
		}
	}
}