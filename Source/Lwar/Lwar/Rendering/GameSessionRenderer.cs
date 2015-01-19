namespace Lwar.Rendering
{
	using System;
	using Assets;
	using Pegasus;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Pegasus.Utilities;
	using Renderers;

	/// <summary>
	///     Represents the context in which rendering operations are performed.
	/// </summary>
	internal class GameSessionRenderer : DisposableObject
	{
		/// <summary>
		///     The renderers that the context uses to render the scene.
		/// </summary>
		private readonly IRenderer[] _renderers =
		{
			new SkyboxRenderer(),
			new StarfieldRenderer(),
			new BoundaryRenderer(),
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
		///     The sprite batch that is used to draw 2D sprites into the scene.
		/// </summary>
		private readonly SpriteBatch _spriteBatch = new SpriteBatch();

		/// <summary>
		///     Adds the given element to the appropriate renderer.
		/// </summary>
		/// <typeparam name="TElement">The type of the element that should be added.</typeparam>
		/// <param name="element">The element that should be added.</param>
		public void Add<TElement>(TElement element)
			where TElement : class
		{
			Assert.ArgumentNotNull(element);

			// Not using Linq for performance reasons
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

			// Not using Linq for performance reasons
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
		///     Initializes all renderers.
		/// </summary>
		public void Initialize()
		{
			var assets = Application.Current.RenderContext.GetAssetBundle<GameBundle>();
			foreach (var renderer in _renderers)
				renderer.Initialize(Application.Current.RenderContext, assets);
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

			// Draw all 3D elements
			foreach (var renderer in _renderers)
				renderer.Draw(output);

			// Draw all 2D elements into the 3D scenes
			_spriteBatch.BlendState = output.RenderContext.BlendStates.Premultiplied;
			_spriteBatch.DepthStencilState = output.RenderContext.DepthStencilStates.DepthRead;
			_spriteBatch.SamplerState = output.RenderContext.SamplerStates.BilinearClampNoMipmaps;
			_spriteBatch.WorldMatrix = Matrix.CreateRotationX(-MathUtils.PiOver2);

			foreach (var renderer in _renderers)
				renderer.Draw(_spriteBatch);

			_spriteBatch.DrawBatch(output);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_renderers.SafeDisposeAll();
			_spriteBatch.SafeDispose();
		}
	}
}