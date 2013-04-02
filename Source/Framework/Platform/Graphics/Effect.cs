using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using Assets;

	// ReSharper disable InconsistentNaming

	/// <summary>
	///   Represents a shader-based rendering effect.
	/// </summary>
	public abstract class Effect
	{
		/// <summary>
		///   The context of the effect that can be used to load and bind effect resources.
		/// </summary>
		protected EffectContext __context;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		protected Effect(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			__context = new EffectContext(graphicsDevice, assets);
		}

		/// <summary>
		///   Creates a texture binding for the given texture and sampler state.
		/// </summary>
		/// <typeparam name="T">The type of the texture that should be bound.</typeparam>
		/// <param name="texture">The texture that should be bound.</param>
		/// <param name="sampler">The sampler state that should be bound.</param>
		public static TextureBinding<T> Bind<T>(T texture, SamplerState sampler)
			where T : Texture
		{
			Assert.ArgumentNotNull(texture, () => texture);
			Assert.ArgumentNotNull(sampler, () => sampler);

			return new TextureBinding<T>(texture, sampler);
		}
	}

	// ReSharper restore InconsistentNaming
}