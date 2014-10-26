namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Components;
	using Network;
	using Network.Messages;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Synchronizes the entities of the game session with all connected clients.
	/// </summary>
	public class SyncToClientsBehavior : EntityBehavior<NetworkSync, Transform, RelativeTransform, Ray>
	{
		/// <summary>
		///     The allocator that is used to allocate network messages.
		/// </summary>
		private readonly PoolAllocator _allocator;

		/// <summary>
		///     The server logic that handles the communication between the server and the clients.
		/// </summary>
		private readonly ServerLogic _serverLogic;

		/// <summary>
		///     A cached array that stores the weapon energy levels of ship updates.
		/// </summary>
		private readonly int[] _weaponEnergyLevels = new int[NetworkProtocol.WeaponSlotCount];

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate network messages.</param>
		/// <param name="serverLogic">The server logic that handles the communication between the server and the clients.</param>
		public SyncToClientsBehavior(PoolAllocator allocator, ServerLogic serverLogic)
			: base(ComponentDependency.Default, ComponentDependency.Required, ComponentDependency.Optional, ComponentDependency.Optional)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNull(serverLogic);

			_allocator = allocator;
			_serverLogic = serverLogic;
		}

		/// <summary>
		///     Invoked when the given entity is affected by the behavior.
		/// </summary>
		/// <param name="entity">The entity that is affected by the behavior.</param>
		/// <param name="networkSync">The sync to clients component of the entity that is affected by the behavior.</param>
		/// <param name="transform">The transform component of the entity that is affected by the behavior.</param>
		/// <param name="relativeTransform">The relative transform component of the entity that is affected by the behavior.</param>
		/// <param name="ray">The ray component of the entity that is affected by the behavior.</param>
		protected override void OnAdded(Entity entity, NetworkSync networkSync, Transform transform,
										RelativeTransform relativeTransform, Ray ray)
		{
			_serverLogic.EntityAdded(entity);
		}

		/// <summary>
		///     Invoked when the given entity is no longer affected by the behavior.
		/// </summary>
		/// <param name="entity">The entity that is no longer affected by the behavior.</param>
		/// <param name="networkSync">The sync to clients component of the entity that is no longer affected by the behavior.</param>
		/// <param name="transform">The transform component of the entity that is no longer affected by the behavior.</param>
		/// <param name="relativeTransform">The relative transform component of the entity that is no longer affected by the behavior.</param>
		/// <param name="ray">The ray component of the entity that is no longer affected by the behavior.</param>
		protected override void OnRemoved(Entity entity, NetworkSync networkSync, Transform transform,
										  RelativeTransform relativeTransform, Ray ray)
		{
			_serverLogic.EntityRemoved(entity);
		}

		/// <summary>
		///     Sends the entity updates to all connected clients.
		/// </summary>
		public void SendEntityUpdates()
		{
			Process();
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="networkSync">The sync to clients components of the affected entities.</param>
		/// <param name="transforms">The transform components of the affected entities.</param>
		/// <param name="relativeTransforms">The relative transform components of the affected entities.</param>
		/// <param name="rays">The ray components of the affected entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected override void Process(Entity[] entities, NetworkSync[] networkSync, Transform[] transforms,
										RelativeTransform[] relativeTransforms, Ray[] rays, int count)
		{
			for (var i = 0; i < count; ++i)
			{
				var position = relativeTransforms[i] == null ? transforms[i].Position : relativeTransforms[i].Position;
				var orientation = relativeTransforms[i] == null ? transforms[i].Orientation : relativeTransforms[i].Orientation;
				_serverLogic.SendEntityUpdate(CreateUpdateMessage(networkSync[i], position, orientation, rays[i]));
			}
		}

		/// <summary>
		///     Creates the update message for the networked entity at the given index.
		/// </summary>
		/// <param name="networkSync">The sync to clients component of the affected entity.</param>
		/// <param name="position">The position of the affected entity.</param>
		/// <param name="orientation">The orientation of the affected entity.</param>
		/// <param name="ray">The ray component of the affected entity.</param>
		private Message CreateUpdateMessage(NetworkSync networkSync, Vector2 position, float orientation, Ray ray)
		{
			orientation = MathUtils.RadToDeg360(-orientation);

			switch (networkSync.UpdateMessageType)
			{
				case MessageType.UpdateTransform:
					return UpdateTransformMessage.Create(_allocator, networkSync.Identity, position, orientation);
				case MessageType.UpdatePosition:
					return UpdatePositionMessage.Create(_allocator, networkSync.Identity, position);
				case MessageType.UpdateRay:
					var target = GetRayTargetIdentifier(ray);
					return UpdateRayMessage.Create(_allocator, networkSync.Identity, target, position, ray.Length, orientation);
				case MessageType.UpdateShip:
					return UpdateShipMessage.Create(_allocator, networkSync.Identity, position, orientation, 100, 0, _weaponEnergyLevels);
				case MessageType.UpdateCircle:
					throw new NotImplementedException();
				default:
					throw new InvalidOperationException("Unsupported message type.");
			}
		}

		/// <summary>
		///     Gets the network identity of the target of a ray component.
		/// </summary>
		/// <param name="ray">The ray component the target identity should be returned for.</param>
		private static Identity GetRayTargetIdentifier(Ray ray)
		{
			Assert.NotNull(ray, "Expected a ray component.");

			if (ray.Target == Entity.None)
				return NetworkProtocol.ReservedEntityIdentity;

			var targetNetwork = ray.Target.GetComponent<NetworkSync>();
			Assert.NotNull(targetNetwork, "Expected ray target to be network synced.");

			return targetNetwork.Identity;
		}
	}
}