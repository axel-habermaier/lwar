using System;

namespace Lwar.Client.Rendering
{
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Assets.Effects;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;
	using Texture2D = Pegasus.Framework.Platform.Graphics.Texture2D;

	/// <summary>
	///   Represents a GPU-based gaussian blur filter than can be applied to a texture.
	/// </summary>
	public class GaussianBlur : DisposableObject
	{
		/// <summary>
		///   The minimum size of the temporary textures.
		/// </summary>
		private const uint MinimumSize = 16;

		/// <summary>
		///   The full-screen quad that is used to blur the textures.
		/// </summary>
		private readonly FullscreenQuad _fullscreenQuad;

		/// <summary>
		///   The temporary render targets that are required to blur the input texture.
		/// </summary>
		private readonly RenderTarget[] _renderTargets;

		/// <summary>
		///   The input texture that is blurred.
		/// </summary>
		private readonly Texture2D _texture;

		/// <summary>
		/// The graphics device that is used to apply the blur effect.
		/// </summary>
		private GraphicsDevice _graphicsDevice;

		private Texture2D[] _textures;
		private Size[] _sizes;

		private BlurEffect _blurEffect;

		private RenderOutput _output;
		private TexturedQuadEffect _quadEffect;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to apply the blur effect.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		/// <param name="texture">The texture that should be blurred.</param>
		public unsafe GaussianBlur(GraphicsDevice graphicsDevice, AssetsManager assets, Texture2D texture)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);
			Assert.ArgumentNotNull(texture, () => texture);
			Assert.InRange(texture.Width, MinimumSize, 2048u);
			//Assert.ArgumentSatisfies(texture.Width == texture.Height, () => texture, "Rectangular textures are not supported.");

			_graphicsDevice = graphicsDevice;
			_texture = texture;
			_fullscreenQuad = new FullscreenQuad(graphicsDevice, assets);

			var width = texture.Width;
			var height = texture.Height;
			var count = 0;
			var sizes = new List<Size>() { texture.Size };
			while (width > MinimumSize && height > MinimumSize)
			{
				++count;
				width /= 2;
				height /= 2;
				sizes.Add(new Size((int)width, (int)height));
			}
			_sizes = sizes.ToArray();

			++count;
			//Assert.That(size == MinimumSize, "Only power-of-two textures can be blurred.");
			_textures = new Texture2D[count * 2];
			_renderTargets = new RenderTarget[count * 2];

			width = texture.Width;
			height = texture.Height;
			for (var i = 0; i < count; ++i)
			{
				_textures[2 * i] = new Texture2D(graphicsDevice, width, height, texture.Format, TextureFlags.RenderTarget);
				_textures[2 * i + 1] = new Texture2D(graphicsDevice, width, height, texture.Format, TextureFlags.RenderTarget);

				width /= 2;
				height /= 2;
			}

			for (var i = 0; i < _renderTargets.Length; ++i)
				_renderTargets[i] = new RenderTarget(graphicsDevice,null, _textures[i]);

			_output = new RenderOutput(graphicsDevice);
			_blurEffect =new BlurEffect(graphicsDevice, assets);
			_quadEffect = new TexturedQuadEffect(graphicsDevice, assets);
		}

		/// <summary>
		///   Blurs the input texture.
		/// </summary>
		public Texture2D Blur(RenderOutput output)
		{
			//_texture.GenerateMipmaps();

			//var viewport = _graphicsDevice.Viewport;

			//_texture.Bind(0);
			DepthStencilState.DepthDisabled.Bind();

			//SamplerState.BilinearClampNoMipmaps.Bind(0);
			//SamplerState.BilinearClampNoMipmaps.Bind(1);
			BlendState.Opaque.Bind();
			//_data.Bind(3);

			_output.Camera = output.Camera;
			int i = 0;
			foreach (var size in _sizes)
			{
				//_data.Data.Mipmap = 0;
				//_data.Data.Size = size.Height;
				//_data.Update();
				_output.RenderTarget= _renderTargets[2 * i];
				_output.Viewport = new Rectangle(Vector2i.Zero, size);
				//_renderTargets[2 * i].Clear(new Color(0, 0, 0, 0));
				//_verticalBlurShader.Bind();
				_blurEffect.Texture = new Texture2DView(_texture, SamplerState.BilinearClampNoMipmaps);

				_fullscreenQuad.Draw(_output, _blurEffect.BlurHorizontally);

				//rt.Bind();
				//_data.Data.Mipmap = 0;
				//_data.Data.Size = size.Width;
				//_data.Update();
				_output.RenderTarget =  _renderTargets[2 * i + 1];
				//_renderTargets[2*i + 1].Clear(new Color(0, 0, 0, 0));
				//_graphicsDevice.Viewport = new Rectangle(Vector2i.Zero, size);
				_blurEffect.Texture=new Texture2DView(_textures[2 * i], SamplerState.BilinearClampNoMipmaps);
				//_horizontalBlurShader.Bind();
				//_fullscreenQuad.Draw();
				_fullscreenQuad.Draw(_output, _blurEffect.BlurVertically);
				++i;
			}

			--i;
			--i;

			//_renderTargets[1].Bind();
			//_graphicsDevice.Viewport = new Rectangle(Vector2i.Zero, _sizes[0]);
			_output.Viewport = new Rectangle(Vector2i.Zero, _sizes[0]);
			_output.RenderTarget = _renderTargets[1];
			BlendState.Premultiplied.Bind();
			while (i >= 0)
			{


				//_renderTargets[2 * i].Clear(new Color(0, 0, 0, 0));
				_quadEffect.Texture = new Texture2DView(_textures[2 * (i + 1) + 1], SamplerState.BilinearClampNoMipmaps);
				//_textures[2 * i + 1].Bind(1);

				//_combineShader.Bind();
				_fullscreenQuad.Draw(_output, _quadEffect.FullScreen);

				--i;
			}


			//rt.Bind();
			//_graphicsDevice.Viewport = viewport;
			//_textures[1].Bind(0);
			return _textures[1];
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