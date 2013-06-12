﻿using System;

namespace Lwar.Client.Rendering
{
	using System.Linq;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;
	using Renderers;

	/// <summary>
	///   Represents the context in which rendering operations are performed.
	/// </summary>
	public class RenderContext : DisposableObject
	{
		/// <summary>
		///   The renderer that is used to draw the parallax scrolling effect.
		/// </summary>
		private readonly ParallaxRenderer _parallaxRenderer;

		/// <summary>
		///   The renderers that the context uses to render the scene.
		/// </summary>
		private readonly IRenderer[] _renderers = new IRenderer[]
		{
			new SunRenderer(),
			new PlanetRenderer(),
			new ShipRenderer(),
			new BulletRenderer(),
			new PhaserRenderer(),
			new RayRenderer(),
			new ShockwaveRenderer(),
			new RocketRenderer(),
			new ShieldRenderer()
		};

		/// <summary>
		///   The renderer that is used to draw the skybox.
		/// </summary>
		private readonly SkyboxRenderer _skyboxRenderer;

		/// <summary>
		///   The sprite batch that is used to draw 2D sprites into the scene.
		/// </summary>
		private readonly SpriteBatch _spriteBatch;

		/// <summary>
		///   The sprite effect that is used to draw 2D sprites into the scene.
		/// </summary>
		private readonly SpriteEffect _spriteEffect = new SpriteEffect();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public RenderContext(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(assets);

			_spriteBatch = new SpriteBatch(graphicsDevice, _spriteEffect);
			_skyboxRenderer = new SkyboxRenderer(graphicsDevice, assets);
			_parallaxRenderer = new ParallaxRenderer(graphicsDevice, assets);
			_spriteEffect.Initialize(graphicsDevice, assets);

			foreach (var renderer in _renderers)
				renderer.Initialize(graphicsDevice, assets);
		}

		/// <summary>
		///   Adds the given element to the appropriate renderer.
		/// </summary>
		/// <typeparam name="TElement">The type of the element that should be added.</typeparam>
		/// <param name="element">The element that should be added.</param>
		public void Add<TElement>(TElement element)
			where TElement : class
		{
			Assert.ArgumentNotNull(element);
			_renderers.OfType<Renderer<TElement>>().Single().Add(element);
		}

		/// <summary>
		///   Removes the given element from the appropriate renderer.
		/// </summary>
		/// <typeparam name="TElement">The type of the element that should be removed.</typeparam>
		/// <param name="element">The element that should be removed.</param>
		public void Remove<TElement>(TElement element)
			where TElement : class
		{
			Assert.ArgumentNotNull(element);
			_renderers.OfType<Renderer<TElement>>().Single().Remove(element);
		}

		/// <summary>
		///   Draws the current frame.
		/// </summary>
		/// <param name="output">The output that the render context should render to.</param>
		public void Draw(RenderOutput output)
		{
			Assert.ArgumentNotNull(output);

			RasterizerState.CullCounterClockwise.Bind();
			_skyboxRenderer.Draw(output);

			RasterizerState.CullCounterClockwise.Bind();
			_parallaxRenderer.Draw(output);

			foreach (var renderer in _renderers)
				renderer.Draw(output);

			_spriteBatch.Output = output;
			_spriteBatch.BlendState = BlendState.Premultiplied;
			_spriteBatch.DepthStencilState = DepthStencilState.DepthRead;
			_spriteBatch.SamplerState = SamplerState.BilinearClampNoMipmaps;
			_spriteBatch.WorldMatrix = Matrix.CreateRotationX(-MathUtils.PiOver2);

			foreach (var renderer in _renderers)
				renderer.Draw(_spriteBatch);

			_spriteBatch.DrawBatch();
		}

		/// <summary>
		///   Draws the user interface elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		/// <param name="camera">The camera that is used to draw the scene.</param>
		public void DrawUserInterface(SpriteBatch spriteBatch, GameCamera camera)
		{
			Assert.ArgumentNotNull(spriteBatch);

			foreach (var renderer in _renderers)
				renderer.DrawUserInterface(spriteBatch, camera);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_renderers.SafeDisposeAll();

			_skyboxRenderer.SafeDispose();
			_parallaxRenderer.SafeDispose();
			_spriteBatch.SafeDispose();
			_spriteEffect.SafeDispose();
		}
	}
}