namespace Lwar.Rendering
{
	using System;
	using Assets.Effects;
	using Pegasus;
	using Pegasus.Assets;
	using Pegasus.Framework;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Renderers;

	/// <summary>
	///     Represents the context in which rendering operations are performed.
	/// </summary>
	public class RenderContext : DisposableObject
	{
		/// <summary>
		///     The effect that is used to draw the level boundary.
		/// </summary>
		private readonly SimpleVertexEffect _boundaryEffect;

		/// <summary>
		///     The model representing the boundary of the level.
		/// </summary>
		private readonly Model _levelBoundary;

		/// <summary>
		///     The renderer that is used to draw the parallax scrolling effect.
		/// </summary>
		private readonly ParallaxRenderer _parallaxRenderer;

		/// <summary>
		///     The renderers that the context uses to render the scene.
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
			new ShieldRenderer(),
			new ExplosionRenderer()
		};

		/// <summary>
		///     The renderer that is used to draw the skybox.
		/// </summary>
		private readonly SkyboxRenderer _skyboxRenderer;

		/// <summary>
		///     The sprite batch that is used to draw 2D sprites into the scene.
		/// </summary>
		private readonly SpriteBatch _spriteBatch;

		/// <summary>
		///     The sprite effect that is used to draw 2D sprites into the scene.
		/// </summary>
		private readonly SpriteEffect _spriteEffect;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public RenderContext(AssetsManager assets)
		{
			Assert.ArgumentNotNull(assets);
			var graphicsDevice = Application.Current.GraphicsDevice;

			_spriteEffect = new SpriteEffect(graphicsDevice, assets);
			_spriteBatch = new SpriteBatch(graphicsDevice, assets);
			_skyboxRenderer = new SkyboxRenderer(graphicsDevice, assets);
			_parallaxRenderer = new ParallaxRenderer(graphicsDevice, assets);
			_boundaryEffect = new SimpleVertexEffect(graphicsDevice, assets);

			const int thickness = 64;
			var outline = new CircleOutline();
			outline.Add(Int16.MaxValue + thickness / 2, 265, thickness);
			_levelBoundary = outline.ToModel(graphicsDevice);

			foreach (var renderer in _renderers)
				renderer.Initialize(graphicsDevice, assets);
		}

		/// <summary>
		///     Adds the given element to the appropriate renderer.
		/// </summary>
		/// <typeparam name="TElement">The type of the element that should be added.</typeparam>
		/// <param name="element">The element that should be added.</param>
		public void Add<TElement>(TElement element)
			where TElement : class
		{
			Assert.ArgumentNotNull(element);

			foreach (var renderer in _renderers)
			{
				var typedRenderer = renderer as Renderer<TElement>;
				if (typedRenderer != null)
				{
					typedRenderer.Add(element);
					break;
				}
			}
		}

		/// <summary>
		///     Removes the given element from the appropriate renderer.
		/// </summary>
		/// <typeparam name="TElement">The type of the element that should be removed.</typeparam>
		/// <param name="element">The element that should be removed.</param>
		public void Remove<TElement>(TElement element)
			where TElement : class
		{
			Assert.ArgumentNotNull(element);

			foreach (var renderer in _renderers)
			{
				var typedRenderer = renderer as Renderer<TElement>;
				if (typedRenderer != null)
				{
					typedRenderer.Remove(element);
					break;
				}
			}
		}

		/// <summary>
		///     Draws the current frame.
		/// </summary>
		/// <param name="output">The output that the render context should render to.</param>
		public void Draw(RenderOutput output)
		{
			Assert.ArgumentNotNull(output);

			// Only clear the depth buffer; the color buffer will be completely overwritten anyway
			output.ClearDepth();

			// TODO: Replace old sprite batch rendering code
			//var camera = renderOutput.Camera;
			//renderOutput.Camera = _camera;

			//_spriteBatch.BlendState = BlendState.Premultiplied;
			//_spriteBatch.DepthStencilState = DepthStencilState.DepthDisabled;
			//_spriteBatch.SamplerState = SamplerState.PointClampNoMipmaps;
			//_camera.Viewport = renderOutput.Viewport;
			//_renderContext.DrawUserInterface(_spriteBatch, CameraManager.GameCamera);
			//_spriteBatch.DrawBatch(renderOutput);

			//renderOutput.Camera = camera;

			// Draw the skybox and the parallax effect
			RasterizerState.CullCounterClockwise.Bind();
			_skyboxRenderer.Draw(output);

			RasterizerState.CullCounterClockwise.Bind();
			_parallaxRenderer.Draw(output);

			// Draw all 3D elements
			foreach (var renderer in _renderers)
				renderer.Draw(output);

			// Draw all 2D elements into the 3D scenes
			_spriteBatch.BlendState = BlendState.Premultiplied;
			_spriteBatch.DepthStencilState = DepthStencilState.DepthRead;
			_spriteBatch.SamplerState = SamplerState.BilinearClampNoMipmaps;
			_spriteBatch.WorldMatrix = Matrix.CreateRotationX(-MathUtils.PiOver2);

			foreach (var renderer in _renderers)
				renderer.Draw(_spriteBatch);

			// Draw the level boundaries
			_boundaryEffect.Color = new Vector4(0.3f, 0, 0, 0.3f);
			_levelBoundary.Draw(output, _boundaryEffect.Default);

			_spriteBatch.DrawBatch(output);
		}

		/// <summary>
		///     Draws the user interface elements.
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
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_renderers.SafeDisposeAll();
			_levelBoundary.SafeDispose();
			_boundaryEffect.SafeDispose();
			_skyboxRenderer.SafeDispose();
			_parallaxRenderer.SafeDispose();
			_spriteBatch.SafeDispose();
			_spriteEffect.SafeDispose();
		}
	}
}