namespace Pegasus.Platform.Graphics
{
	using System;
	using Math;

	/// <summary>
	///     Represents a 2D texture associated with a sampler state that can be used by an effect.
	/// </summary>
	public struct Texture2DView : IEquatable<Texture2DView>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="texture">The texture that should be bound.</param>
		/// <param name="sampler">The sampler that should be bound.</param>
		public Texture2DView(Texture2D texture, SamplerState sampler)
			: this()
		{
			Assert.ArgumentNotNull(texture);
			Assert.ArgumentNotNull(sampler);
			Assert.That(sampler.Filter >= TextureFilter.NearestNoMipmaps || texture.HasMipmaps,
				"Texture filter '{0}' cannot be used to sample a 2D texture without any mipmaps.", sampler.Filter);

			Texture = texture;
			Sampler = sampler;
		}

		/// <summary>
		///     Gets the bound texture.
		/// </summary>
		public Texture2D Texture { get; private set; }

		/// <summary>
		///     Gets the size of the bound texture.
		/// </summary>
		public Size Size
		{
			get
			{
				Assert.NotNull(Texture, "No texture has been set.");
				return Texture.Size;
			}
		}

		/// <summary>
		///     Gets the width of the bound texture.
		/// </summary>
		public int Width
		{
			get
			{
				Assert.NotNull(Texture, "No texture has been set.");
				return Texture.Width;
			}
		}

		/// <summary>
		///     Gets the height of the bound texture.
		/// </summary>
		public int Height
		{
			get
			{
				Assert.NotNull(Texture, "No texture has been set.");
				return Texture.Height;
			}
		}

		/// <summary>
		///     Gets the bound sampler state.
		/// </summary>
		public SamplerState Sampler { get; private set; }

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Texture2DView other)
		{
			return Equals(Texture, other.Texture) && Equals(Sampler, other.Sampler);
		}

		/// <summary>
		///     Binds the texture and sampler state to the GPU.
		/// </summary>
		/// <param name="slot">The slot the texture and sampler state should be bound to.</param>
		internal void Bind(int slot)
		{
			Assert.NotNull(Texture, "No texture has been set.");
			Assert.NotNull(Sampler, "No sampler state has been set.");

			Texture.Bind(slot);
			Sampler.Bind(slot);
		}

		/// <summary>
		///     Unbinds the texture from the GPU if the texture is used as a render target.
		/// </summary>
		/// <param name="slot">The slot the texture should be unbound from.</param>
		internal void Unbind(int slot)
		{
			Assert.NotNull(Texture, "No texture has been set.");
			Assert.NotNull(Sampler, "No sampler state has been set.");

			if (Texture.IsColorBuffer || Texture.IsDepthStencilBuffer)
				Texture.Unbind(slot);
		}

		/// <summary>
		///     Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to. </param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is Texture2DView && Equals((Texture2DView)obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((Texture != null ? Texture.GetHashCode() : 0) * 397) ^ (Sampler != null ? Sampler.GetHashCode() : 0);
			}
		}

		/// <summary>
		///     Checks whether the two texture views are equal.
		/// </summary>
		/// <param name="left">The first texture view to compare.</param>
		/// <param name="right">The second texture view to compare.</param>
		public static bool operator ==(Texture2DView left, Texture2DView right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Checks whether the two texture views are not equal.
		/// </summary>
		/// <param name="left">The first texture view to compare.</param>
		/// <param name="right">The second texture view to compare.</param>
		public static bool operator !=(Texture2DView left, Texture2DView right)
		{
			return !left.Equals(right);
		}
	}
}