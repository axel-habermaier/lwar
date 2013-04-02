using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Represents a cubemap associated with a sampler state that can be used by an effect.
	/// </summary>
	public struct CubeMapView
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="cubeMap">The texture that should be bound.</param>
		/// <param name="sampler">The sampler that should be bound.</param>
		public CubeMapView(CubeMap cubeMap, SamplerState sampler)
			: this()
		{
			Assert.ArgumentNotNull(cubeMap, () => cubeMap);
			Assert.ArgumentNotNull(sampler, () => sampler);

			CubeMap = cubeMap;
			Sampler = sampler;
		}

		/// <summary>
		///   Gets the bound texture.
		/// </summary>
		public CubeMap CubeMap { get; private set; }

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
			Assert.NotNull(CubeMap, "No texture has been set.");
			Assert.NotNull(Sampler, "No sampler state has been set.");

			CubeMap.Bind(slot);
			Sampler.Bind(slot);
		}
	}
}