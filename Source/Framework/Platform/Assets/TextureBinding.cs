using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Graphics;

	/// <summary>
	///   Binds a texture and a sampler to be used by an effect.
	/// </summary>
	/// <typeparam name="T">The type of the texture that is bound.</typeparam>
	public struct TextureBinding<T>
		where T : Texture
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="texture">The texture that should be bound.</param>
		/// <param name="sampler">The sampler that should be bound.</param>
		internal TextureBinding(T texture, SamplerState sampler)
			: this()
		{
			Assert.ArgumentNotNull(texture, () => texture);
			Assert.ArgumentNotNull(sampler, () => sampler);

			Texture = texture;
			Sampler = sampler;
		}

		/// <summary>
		///   Gets the bound texture.
		/// </summary>
		public T Texture { get; private set; }

		/// <summary>
		///   Gets the bound sampler state.
		/// </summary>
		public SamplerState Sampler { get; private set; }

		/// <summary>
		/// Binds the texture and sampler state to the GPU.
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