using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Rendering;

	public class Ship : Entity<Ship>
	{
		[Asset("Textures/Ship")]
		public static Texture2D Texture;

		public Model Model { get; private set; }

		public override void Draw()
		{
			var c = Health / 100.0f;
			Position = new Vector2(0, 0);
			//SpriteBatch.Draw(Texture, Position, Rotation, new Color(1.0f * c, 0.5f * c, 0.0f * c, 1.0f));
			RenderContext.ShipRenderer.Draw(this);
		}

		protected override void Added()
		{
			//Model = Model.CreateQuad(GraphicsDevice, Texture.Size);
			Model = Model.CreateQuad(GraphicsDevice, 100, 100);
			//Texture.GenerateMipmaps();
		}

		protected override void OnReturning()
		{
			Model.SafeDispose();
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