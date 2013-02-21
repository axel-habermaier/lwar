using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Graphics;
	using Rendering;

	public class Bullet : Entity<Bullet>
	{
		[Asset("Textures/Bullet")]
		public static Texture2D Texture;

		public Model Model { get; private set; }

		public override void Draw()
		{
			//SpriteBatch.Draw(Texture, Position, Rotation, new Color(c, c, c, 1.0f));
			RenderContext.BulletRenderer.Draw(this);
		}

		protected override void Added()
		{
			Model = Model.CreateQuad(GraphicsDevice, Texture.Size);
		}

		protected override void OnReturning()
		{
			Model.SafeDispose();
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