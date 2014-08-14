namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Represents a cubemap associated with a sampler state that can be used by an effect.
	/// </summary>
	public struct CubeMapView
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="cubeMap">The texture that should be bound.</param>
		/// <param name="sampler">The sampler that should be bound.</param>
		public CubeMapView(CubeMap cubeMap, SamplerState sampler)
			: this()
		{
			Assert.ArgumentNotNull(cubeMap);
			Assert.ArgumentNotNull(sampler);

			CubeMap = cubeMap;
			Sampler = sampler;
		}

		/// <summary>
		///     Gets the bound texture.
		/// </summary>
		public CubeMap CubeMap { get; private set; }

		/// <summary>
		///     Gets the bound sampler state.
		/// </summary>
		public SamplerState Sampler { get; private set; }

		/// <summary>
		///     Binds the texture and sampler state to the GPU.
		/// </summary>
		/// <param name="slot">The slot the texture and sampler state should be bound to.</param>
		internal void Bind(int slot)
		{
			Assert.NotNull(CubeMap, "No texture has been set.");
			Assert.NotNull(Sampler, "No sampler state has been set.");
			Assert.That(Sampler.Filter >= TextureFilter.NearestNoMipmaps || CubeMap.HasMipmaps,
						"Texture filter '{0}' cannot be used to sample a cubemap without any mipmaps.", Sampler.Filter);

			CubeMap.Bind(slot);
			Sampler.Bind(slot);
		}

		/// <summary>
		///     Unbinds the texture from the GPU if the texture is used as a render target.
		/// </summary>
		/// <param name="slot">The slot the texture should be unbound from.</param>
		internal void Unbind(int slot)
		{
			Assert.NotNull(CubeMap, "No texture has been set.");
			Assert.NotNull(Sampler, "No sampler state has been set.");

			if (CubeMap.IsColorBuffer || CubeMap.IsDepthStencilBuffer)
				CubeMap.Unbind(slot);
		}
	}
}