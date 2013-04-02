using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Represents a 2D texture associated with a sampler state that can be used by an effect.
	/// </summary>
	public struct Texture2DView
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="texture">The texture that should be bound.</param>
		/// <param name="sampler">The sampler that should be bound.</param>
		public Texture2DView(Texture2D texture, SamplerState sampler)
			: this()
		{
			Assert.ArgumentNotNull(texture, () => texture);
			Assert.ArgumentNotNull(sampler, () => sampler);
			Assert.That(sampler.Filter >= TextureFilter.NearestNoMipmaps || texture.HasMipmaps,
						"Texture filter '{0}' cannot be used to sample a 2D texture without any mipmaps.", sampler.Filter);

			Texture = texture;
			Sampler = sampler;
		}

		/// <summary>
		///   Gets the bound texture.
		/// </summary>
		public Texture2D Texture { get; private set; }

		/// <summary>
		///   Gets the bound sampler state.
		/// </summary>
		public SamplerState Sampler { get; private set; }

		/// <summary>
		///   Binds the texture and sampler state to the GPU.
		/// </summary>
		/// <param name="slot">The slot the texture and sampler state should be bound to.</param>
		internal void Bind(int slot)
		{
			Assert.NotNull(Texture, "No texture has been set.");
			Assert.NotNull(Sampler, "No sampler state has been set.");

			Texture.Bind(slot);
			Sampler.Bind(slot);
		}
	}
}