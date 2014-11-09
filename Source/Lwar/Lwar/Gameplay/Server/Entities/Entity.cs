namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Network;
	using Network.Messages;
	using Pegasus.Math;
	using Pegasus.Scene;

	/// <summary>
	///     A base class for entity scene nodes.
	/// </summary>
	public abstract class Entity : SceneNode
	{
		/// <summary>
		///     Gets the network type of the entity.
		/// </summary>
		public EntityType NetworkType { get; protected set; }

		/// <summary>
		///     Gets or sets the network identity of the entity.
		/// </summary>
		public NetworkIdentity NetworkIdentity { get; set; }

		/// <summary>
		///     Gets the type of the update messages that are sent for the entity.
		/// </summary>
		public MessageType UpdateMessageType { get; protected set; }

		/// <summary>
		///     Gets or sets the game session the entity belongs to.
		/// </summary>
		public GameSession GameSession { get; set; }

		/// <summary>
		///     Gets or sets the entity's position in 2D space.
		/// </summary>
		public Vector2 Position2D
		{
			get
			{
				var position = Position;
				return new Vector2(position.X, position.Z);
			}
			set { Position = new Vector3(value.X, 0, value.Y); }
		}

		/// <summary>
		///     Gets the entity's world position in 2D space.
		/// </summary>
		public Vector2 WorldPosition2D
		{
			get
			{
				var position = WorldPosition;
				return new Vector2(position.X, position.Z);
			}
		}

		/// <summary>
		///     Gets or sets the entity's orientation in 2D space.
		/// </summary>
		public float Orientation
		{
			get { return Rotation.Y; }
			set
			{
				var rotation = Rotation;
				rotation.Y = value;
				Rotation = rotation;
			}
		}

		/// <summary>
		///     Gets or sets the velocity of the entity in 2D space.
		/// </summary>
		public Vector2 Velocity { get; set; }

		/// <summary>
		///     Gets or sets the player the entity belongs to.
		/// </summary>
		public Player Player { get; protected set; }

		/// <summary>
		///     Updates the server-side state of the entity.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public virtual void ServerUpdate(float elapsedSeconds)
		{
		}

		/// <summary>
		///     Updates the client-side state of the entity.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public virtual void ClientUpdate(float elapsedSeconds)
		{
		}

		/// <summary>
		///     Invoked when the server-side entity is added to a game session.
		/// </summary>
		/// <remarks>Hides the method from deriving types.</remarks>
		public virtual void OnServerAdded()
		{
		}

		/// <summary>
		///     Invoked when the server-side entity is removed from a game session.
		/// </summary>
		/// <remarks>This method is not called when the game session is disposed.</remarks>
		/// <remarks>Hides the method from deriving types.</remarks>
		public virtual void OnServerRemoved()
		{
		}

		/// <summary>
		///     Invoked when the client-side entity is added to a game session.
		/// </summary>
		/// <remarks>Hides the method from deriving types.</remarks>
		public virtual void OnClientAdded()
		{
		}

		/// <summary>
		///     Invoked when the client-side entity is removed from a game session.
		/// </summary>
		/// <remarks>This method is not called when the game session is disposed.</remarks>
		/// <remarks>Hides the method from deriving types.</remarks>
		public virtual void OnClientRemoved()
		{
		}

		/// <summary>
		///     Invoked when the scene node is attached to a parent scene node.
		/// </summary>
		/// <remarks>
		///     The method is intentionally hidden from deriving types; entities should use the appropriate client or server Add
		///     methods instead.
		/// </remarks>
		protected override sealed void OnAttached()
		{
		}

		/// <summary>
		///     Invoked when the scene node is detached from its scene graph.
		/// </summary>
		/// <remarks>This method is not called when the scene graph is disposed.</remarks>
		/// <remarks>
		///     The method is intentionally hidden from deriving types; entities should use the appropriate client or server Remove
		///     methods instead.
		/// </remarks>
		protected override sealed void OnDetached()
		{
			Position2D = Vector2.Zero;
			Orientation = 0;
			Velocity = Vector2.Zero;
			Player = null;
			GameSession = null;
		}
	}
}