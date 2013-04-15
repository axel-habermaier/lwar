using System;

namespace Lwar.Client.Rendering
{
	using System.Linq;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;
	using Renderers;
	using Scripting;

	/// <summary>
	///   Represents the context in which rendering operations are performed.
	/// </summary>
	public class RenderContext : DisposableObject
	{
		/// <summary>
		///   The cvar registry that handles the application cvars.
		/// </summary>
		private readonly CvarRegistry _cvars;

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
			new RocketRenderer()
		};

		/// <summary>
		///   The renderer that is used to draw the skybox.
		/// </summary>
		private readonly SkyboxRenderer _skyboxRenderer;

		/// <summary>
		///   The rasterizer state that is used to draw in wireframe mode.
		/// </summary>
		private readonly RasterizerState _wireframe;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		/// <param name="cvars"> The cvar registry that handles the application cvars.</param>
		public RenderContext(GraphicsDevice graphicsDevice, AssetsManager assets, CvarRegistry cvars)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);
			Assert.ArgumentNotNull(cvars, () => cvars);

			_cvars = cvars;
			_wireframe = new RasterizerState(graphicsDevice) { CullMode = CullMode.Back, FillMode = FillMode.Wireframe };
			_skyboxRenderer = new SkyboxRenderer(graphicsDevice, assets);

			foreach (var renderer in _renderers)
				renderer.Initialize(graphicsDevice, assets);
		}

		/// <summary>
		///   Adds the given entity to the appropriate renderer.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity that should be added.</typeparam>
		/// <param name="entity">The entity that should be added.</param>
		public void Add<TEntity>(TEntity entity)
			where TEntity : class, IEntity
		{
			Assert.ArgumentNotNull(entity, () => entity);
			_renderers.OfType<Renderer<TEntity>>().Single().Add(entity);
		}

		/// <summary>
		///   Removes the given entity from the appropriate renderer.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity that should be removed.</typeparam>
		/// <param name="entity">The entity that should be removed.</param>
		public void Remove<TEntity>(TEntity entity)
			where TEntity : class, IEntity
		{
			Assert.ArgumentNotNull(entity, () => entity);
			_renderers.OfType<Renderer<TEntity>>().Single().Remove(entity);
		}

		/// <summary>
		///   Renders a frame.
		/// </summary>
		/// <param name="output">The output that the render context should render to.</param>
		public void Draw(RenderOutput output)
		{
			Assert.ArgumentNotNull(output, () => output);

			RasterizerState.CullCounterClockwise.Bind();
			_skyboxRenderer.Draw(output);

			if (_cvars.DrawWireframe)
				_wireframe.Bind();
			else
				RasterizerState.CullCounterClockwise.Bind();

			DepthStencilState.DepthEnabled.Bind();

			foreach (var renderer in _renderers)
				renderer.Draw(output);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_renderers.SafeDisposeAll();

			_skyboxRenderer.SafeDispose();
			_wireframe.SafeDispose();
		}
	}
}