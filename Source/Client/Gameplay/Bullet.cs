using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Graphics;

	public class Bullet : Entity<Bullet>
	{
		[Asset("Textures/Bullet")]
		public static Texture2D Texture;

		public override void Draw()
		{
			var c = Health / 100.0f;
			SpriteBatch.Draw(Texture, Position, Rotation, new Color(c, c, c, 1.0f));
		}

		public static Bullet Create(Player player)
		{
			Assert.ArgumentNotNull(player, () => player);

			var bullet = GetInstance();
			bullet.Player = player;
			return bullet;
		}
	}
}