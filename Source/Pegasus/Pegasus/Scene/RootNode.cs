namespace Pegasus.Scene
{
	using System;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents the root of a scene graph.
	/// </summary>
	internal class RootNode : SceneNode
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static RootNode()
		{
			ConstructorCache.Register(() => new RootNode());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private RootNode()
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The game session the planet should belong to.</param>
		internal static RootNode Create(PoolAllocator allocator)
		{
			Assert.ArgumentNotNull(allocator);
			return allocator.Allocate<RootNode>();
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return "Root";
		}
	}
}