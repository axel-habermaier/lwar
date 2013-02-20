using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Graphics;
	using Rendering;

	public class Planet : Entity<Planet>
	{
		[Asset("Textures/Sun")]
		public static CubeMap Texture;

		public Model Model { get; private set; }

		public override void Draw()
		{
			var c = Health / 100.0f;
			//SpriteBatch.Draw(Texture, Position, Rotation, new Color(0.1f * c, 0.3f * c, 0.8f * c, 1.0f));
			//Session.RenderContext.PlanetRenderer.Draw(this);
		}

		protected override void OnReturning()
		{
			Model.SafeDispose();
		}

		protected override void Added()
		{
			Model = Model.CreateSphere(GraphicsDevice, 100, 25);
			//Texture.GenerateMipmaps();
		}

		public static Planet Create(Player player)
		{
			Assert.ArgumentNotNull(player, () => player);

			var planet = GetInstance();
			planet.Player = player;
			return planet;
		}
	}
}