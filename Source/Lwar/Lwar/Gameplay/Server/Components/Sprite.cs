namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Provides a texture that is rendered as a sprite.
	/// </summary>
	public class Sprite : Component
	{
		/// <summary>
		///     The texture of the sprite.
		/// </summary>
		public Texture2D Texture;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Sprite()
		{
			ConstructorCache.Set(() => new Sprite());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Sprite()
		{
		}

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="texture">The texture of the sprite.</param>
		public static Sprite Create(PoolAllocator allocator, Texture2D texture)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNull(texture);

			var component = allocator.Allocate<Sprite>();
			component.Texture = texture;
			return component;
		}
	}
}