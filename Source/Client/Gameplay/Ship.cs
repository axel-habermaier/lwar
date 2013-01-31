using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework.Platform.Graphics;

	public class Ship : Entity<Ship>
	{
		[Asset("Textures/Ship")]
		public static Texture2D Texture;

		public override void Draw()
		{
			SpriteBatch.Draw(Texture, Position, new Color(244, 128, 23, 255));
		}

		public static Ship Create()
		{
			return GetInstance();
		}
	}
}