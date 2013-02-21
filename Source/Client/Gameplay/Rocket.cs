using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Graphics;

	public class Rocket : Entity<Rocket>
	{
		[Asset("Textures/Rocket")]
		public static Texture2D Texture;

		public override void Draw()
		{
			//SpriteBatch.Draw(Texture, Position, Rotation, new Color(c, 0, 0, 1.0f));
		}

		public static Rocket Create(Player player)
		{
			Assert.ArgumentNotNull(player, () => player);

			var rocket = GetInstance();
			rocket.Player = player;
			return rocket;
		}
	}
}