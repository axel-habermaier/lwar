namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Network;
	using Network.Messages;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Syncs the entity to all peers.
	/// </summary>
	public class NetworkSync : Component
	{
		/// <summary>
		///     The network type of the entity.
		/// </summary>
		public EntityType EntityType;

		/// <summary>
		///     The network identity of the entity.
		/// </summary>
		public Identity Identity;

		/// <summary>
		///     The type of the update messages that are sent for the entity.
		/// </summary>
		public MessageType UpdateMessageType;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static NetworkSync()
		{
			ConstructorCache.Register(() => new NetworkSync());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private NetworkSync()
		{
		}

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="identity">The network identity of the entity.</param>
		/// <param name="entityType">The network type of the entity.</param>
		/// <param name="updateMessageType">The type of the update messages that should be sent for the entity.</param>
		public static NetworkSync Create(PoolAllocator allocator, Identity identity, EntityType entityType, MessageType updateMessageType)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentInRange(entityType);
			Assert.ArgumentSatisfies(updateMessageType == MessageType.UpdateCircle ||
									 updateMessageType == MessageType.UpdatePosition ||
									 updateMessageType == MessageType.UpdateTransform ||
									 updateMessageType == MessageType.UpdateShip ||
									 updateMessageType == MessageType.UpdateRay, "Invalid message type.");

			var component = allocator.Allocate<NetworkSync>();
			component.Identity = identity;
			component.EntityType = entityType;
			component.UpdateMessageType = updateMessageType;
			return component;
		}
	}
}