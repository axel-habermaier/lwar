using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Graphics;

	public class Ship : Entity<Ship>
	{
		[Asset("Textures/Ship")]
		public static Texture2D Texture;

		public override void Draw()
		{
			var c = Health / 100.0f;
			//SpriteBatch.Draw(Texture, Position, Rotation, new Color(1.0f * c, 0.5f * c, 0.0f * c, 1.0f));
		}

		public static Ship Create(Player player)
		{
			Assert.ArgumentNotNull(player, () => player);

			var ship = GetInstance();
			ship.Player = player;
			return ship;
		}
	}
}