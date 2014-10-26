namespace Pegasus.Platform.Graphics
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents a cubemap associated with a sampler state that can be used by an effect.
	/// </summary>
	public struct CubeMapView : IEquatable<CubeMapView>
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
			Assert.That(sampler.Filter >= TextureFilter.NearestNoMipmaps || cubeMap.HasMipmaps,
				"Texture filter '{0}' cannot be used to sample a cubemap without any mipmaps.", sampler.Filter);

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
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(CubeMapView other)
		{
			return Equals(CubeMap, other.CubeMap) && Equals(Sampler, other.Sampler);
		}

		/// <summary>
		///     Binds the texture and sampler state to the GPU.
		/// </summary>
		/// <param name="slot">The slot the texture and sampler state should be bound to.</param>
		internal void Bind(int slot)
		{
			Assert.NotNull(CubeMap, "No texture has been set.");
			Assert.NotNull(Sampler, "No sampler state has been set.");

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

		/// <summary>
		///     Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to. </param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is CubeMapView && Equals((CubeMapView)obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((CubeMap != null ? CubeMap.GetHashCode() : 0) * 397) ^ (Sampler != null ? Sampler.GetHashCode() : 0);
			}
		}

		/// <summary>
		///     Checks whether the two cubemap views are equal.
		/// </summary>
		/// <param name="left">The first cubemap view to compare.</param>
		/// <param name="right">The second cubemap view to compare.</param>
		public static bool operator ==(CubeMapView left, CubeMapView right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Checks whether the two cubemap views are not equal.
		/// </summary>
		/// <param name="left">The first cubemap view to compare.</param>
		/// <param name="right">The second cubemap view to compare.</param>
		public static bool operator !=(CubeMapView left, CubeMapView right)
		{
			return !left.Equals(right);
		}
	}
}