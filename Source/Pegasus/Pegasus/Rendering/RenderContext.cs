namespace Pegasus.Rendering
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Scripting;
	using UserInterface;
	using Utilities;

	/// <summary>
	///     Represents the context of rendering operations.
	/// </summary>
	public sealed partial class RenderContext : DisposableObject
	{
		/// <summary>
		///     The asset bundles that have been loaded for the render context.
		/// </summary>
		private readonly List<AssetBundle> _assetBundles = new List<AssetBundle>();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used by the render context.</param>
		public unsafe RenderContext(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			GraphicsDevice = graphicsDevice;
			BlendStates = new CommonBlendStates(graphicsDevice);
			DepthStencilStates = new CommonDepthStencilStates(graphicsDevice);
			RasterizerStates = new CommonRasterizerStates(graphicsDevice);
			SamplerStates = new CommonSamplerStates(graphicsDevice);

			Commands.OnReloadAssets += ReloadAssets;

			var description = new TextureDescription
			{
				Width = 1,
				Height = 1,
				Depth = 1,
				ArraySize = 1,
				Type = TextureType.Texture2D,
				Format = SurfaceFormat.Rgba8,
				Mipmaps = 1,
				SurfaceCount = 1
			};

			var buffer = new byte[] { 255, 255, 255, 255 };
			using (var pointer = new BufferPointer(buffer))
			{
				var surfaces = new[]
				{
					new Surface
					{
						Data = pointer.Pointer,
						Width = 1,
						Height = 1,
						Depth = 1,
						SizeInBytes = 4,
						Stride = 4
					}
				};

				WhiteTexture2D = new Texture2D(graphicsDevice, description, surfaces);
				WhiteTexture2D.SetName("White");
			}
		}

		/// <summary>
		///     Gets a 1x1 pixels fully white two-dimensional texture object.
		/// </summary>
		public Texture2D WhiteTexture2D { get; private set; }

		/// <summary>
		///     Provides access to common blend states.
		/// </summary>
		public CommonBlendStates BlendStates { get; private set; }

		/// <summary>
		///     Provides access to common depth stencil states.
		/// </summary>
		public CommonDepthStencilStates DepthStencilStates { get; private set; }

		/// <summary>
		///     Provides access to common rasterizer states.
		/// </summary>
		public CommonRasterizerStates RasterizerStates { get; private set; }

		/// <summary>
		///     Provides access to common sampler states.
		/// </summary>
		public CommonSamplerStates SamplerStates { get; private set; }

		/// <summary>
		///     Gets the graphics device used by the render context.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///     Reloads all loaded asset bundles.
		/// </summary>
		private void ReloadAssets()
		{
			foreach (var bundle in _assetBundles)
				bundle.Reload();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.OnReloadAssets -= ReloadAssets;

			BlendStates.Dispose();
			DepthStencilStates.Dispose();
			RasterizerStates.Dispose();
			SamplerStates.Dispose();
			WhiteTexture2D.SafeDispose();
		}

		/// <summary>
		///     Adds the given asset bundle to the render context.
		/// </summary>
		/// <param name="assetBundle">The asset bundle that should be added.</param>
		internal void AddAssetBundle(AssetBundle assetBundle)
		{
			Assert.ArgumentNotNull(assetBundle);
			Assert.ArgumentSatisfies(!_assetBundles.Contains(assetBundle), "The asset bundle has already been added.");
			Assert.ArgumentSatisfies(_assetBundles.All(b => b.GetType() != assetBundle.GetType()),
				"An asset bundle of the given type has already been added.");

			_assetBundles.Add(assetBundle);
		}

		/// <summary>
		///     Removes the given asset bundle from the render context.
		/// </summary>
		/// <param name="assetBundle">The asset bundle that should be removed.</param>
		internal void RemoveAssetBundle(AssetBundle assetBundle)
		{
			Assert.ArgumentNotNull(assetBundle);
			Assert.ArgumentSatisfies(_assetBundles.Contains(assetBundle), "The asset bundle has never been added.");

			_assetBundles.Remove(assetBundle);
		}

		/// <summary>
		///     Gets the instance of the asset bundle with the given type if it has been loaded for this render context.
		/// </summary>
		/// <typeparam name="T">The type of the asset bundle that should be returned.</typeparam>
		public T GetAssetBundle<T>() where T : AssetBundle
		{
			foreach (var assetBundle in _assetBundles)
			{
				var typedBundle = assetBundle as T;
				if (typedBundle != null)
					return typedBundle;
			}

			Log.Die("An asset bundle of the given type has not been loaded.");
			return null;
		}

		/// <summary>
		///     Gets a font matching the given search criteria.
		/// </summary>
		/// <param name="family">The family of the font.</param>
		/// <param name="size">The size of the font.</param>
		/// <param name="bold">Indicates whether the font should be bold.</param>
		/// <param name="italic">Indicates whether the font should be italic.</param>
		/// <param name="aliased">Indicates whether the font should be aliased.</param>
		internal Font GetFont(string family, int size, bool bold, bool italic, bool aliased)
		{
			Assert.ArgumentNotNullOrWhitespace(family);

			foreach (var bundle in _assetBundles)
			{
				var font = bundle.GetFont(family, size, bold, italic, aliased);
				if (font != null)
					return font;
			}

			Log.Die("Unable to find a suitable font: Family='{0}', Size={1}, Bold={2}, Italic={3}, Aliased={4}",
				family, size, bold, italic, aliased);

			return null;
		}
	}
}