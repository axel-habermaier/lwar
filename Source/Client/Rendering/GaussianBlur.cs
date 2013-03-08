using System;

namespace Lwar.Client.Rendering
{
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;

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
		///   The temporary textures that are required to blur the input texture.
		/// </summary>
		private readonly Texture2D[] _textures;

		/// <summary>
		///   The fragment shader that applies the horizontal blur.
		/// </summary>
		private FragmentShader _horizontalBlurShader;

		/// <summary>
		///   The fragment shader that applies the vertical blur.
		/// </summary>
		private FragmentShader _verticalBlurShader, _combineShader;

		/// <summary>
		/// The graphics device that is used to apply the blur effect.
		/// </summary>
		private GraphicsDevice _graphicsDevice;

		[StructLayout(LayoutKind.Sequential, Size =16)]
		struct ShaderData
		{
			public int Size;
			public int Mipmap;
		}

		private ConstantBuffer<ShaderData> _data;

		private Size[] _sizes;

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
			_verticalBlurShader = assets.LoadFragmentShader("Shaders/VerticalBlurFS");
			_horizontalBlurShader = assets.LoadFragmentShader("Shaders/HorizontalBlurFS");
			_combineShader = assets.LoadFragmentShader("Shaders/CombineBlurFS");
			_fullscreenQuad = new FullscreenQuad(graphicsDevice, assets);
			_data = new ConstantBuffer<ShaderData>(graphicsDevice, (buffer, data) => buffer.Copy(&data));

			var width = texture.Width;
			var height = texture.Height;
			var count = 0;
			var sizes = new List<Size>(){texture.Size};
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
				_renderTargets[i] = new RenderTarget(graphicsDevice, new[] { _textures[i] }, null);
		}

		/// <summary>
		///   Blurs the input texture.
		/// </summary>
		public void Blur(RenderTarget rt)
		{
			_texture.GenerateMipmaps();

			var viewport = _graphicsDevice.Viewport;

			_texture.Bind(0);
			DepthStencilState.DepthDisabled.Bind();
			
			SamplerState.BilinearClamp.Bind(0);
			SamplerState.BilinearClamp.Bind(1);
			BlendState.Opaque.Bind();
			_data.Bind(3);

			int i = 0;
			foreach (var size in _sizes)
			{
				_data.Data.Mipmap = 0;
				_data.Data.Size = size.Height;
				_data.Update();
				_graphicsDevice.Viewport = new Rectangle(Vector2i.Zero, size);
				_renderTargets[2 * i].Bind();
				_renderTargets[2 * i].Clear(new Color(0, 0, 0, 0));
				_verticalBlurShader.Bind();

				_fullscreenQuad.Draw();

				//rt.Bind();
				_data.Data.Mipmap = 0;
				_data.Data.Size = size.Width;
				_data.Update();
				_renderTargets[2 * i + 1].Bind();
				_renderTargets[2*i + 1].Clear(new Color(0, 0, 0, 0));
				_textures[2 * i].Bind(0);
				_horizontalBlurShader.Bind();
				_fullscreenQuad.Draw();

				++i;
			}

			--i;
			--i;
	

			//BlendState.Additive.Bind();
			while (i >= 0)
			{
				_graphicsDevice.Viewport = new Rectangle(Vector2i.Zero, _sizes[i]);
				_renderTargets[2 * i].Bind();
				_renderTargets[2 * i].Clear(new Color(0, 0, 0, 0));
				_textures[2 * (i + 1)].Bind(0);
				_textures[2 * i + 1].Bind(1);

				_combineShader.Bind();
				_fullscreenQuad.Draw();

				--i;
			}


			rt.Bind();
			_graphicsDevice.Viewport = viewport;
			_textures[0].Bind(0);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_fullscreenQuad.SafeDispose();
			_renderTargets.SafeDisposeAll();
			_textures.SafeDisposeAll();
			_data.SafeDispose();
		}
	}
}