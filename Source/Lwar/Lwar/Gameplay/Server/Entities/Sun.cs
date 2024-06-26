﻿namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Network;
	using Network.Messages;
	using Pegasus.Math;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a sun.
	/// </summary>
	internal class Sun : Entity
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Sun()
		{
			NetworkType = EntityType.Sun;
			UpdateMessageType = MessageType.UpdatePosition;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the entity belongs to.</param>
		/// <param name="position">The position of the sun.</param>
		public static Sun Create(GameSession gameSession, Vector2 position)
		{
			Assert.ArgumentNotNull(gameSession);

			var sun = gameSession.Allocate<Sun>();
			sun.GameSession = gameSession;
			sun.Player = gameSession.Players.ServerPlayer;
			sun.Position2D = position;
			return sun;
		}
	}
}