using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework.Platform.Graphics;

	public class Bullet : Entity<Bullet>
	{
		[Asset("Textures/Bullet")]
		public static Texture2D Texture;

		public override void Draw()
		{
			SpriteBatch.Draw(Texture, Position, new Color(121, 128, 123, 255));
		}

		public static Bullet Create()
		{
			return GetInstance();
		}
	}
}