using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	public class Planet : Entity<Planet>
	{
		[Asset("Textures/Planet")]
		public static Texture2D Texture;

		private Sphere _sphere;

		public override void Draw()
		{
			var c = Health / 100.0f;
			//SpriteBatch.Draw(Texture, Position, Rotation, new Color(0.1f * c, 0.3f * c, 0.8f * c, 1.0f));
			_sphere.Draw(GraphicsDevice);
		}

		protected override void OnReturning()
		{
			_sphere.SafeDispose();
		}

		protected override void Added()
		{
			_sphere = new Sphere(GraphicsDevice, Assets, 100);
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