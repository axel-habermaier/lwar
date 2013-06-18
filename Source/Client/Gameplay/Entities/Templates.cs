//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Tuesday, 18 June 2013, 14:18:23
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lwar.Client.Gameplay.Entities
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	public static class Templates
	{
		public static Template Bullet { get; private set; }
		public static Template Earth { get; private set; }
		public static Template Gun { get; private set; }
		public static Template Jupiter { get; private set; }
		public static Template Mars { get; private set; }
		public static Template Moon { get; private set; }
		public static Template Phaser { get; private set; }
		public static Template Ray { get; private set; }
		public static Template Rocket { get; private set; }
		public static Template Ship { get; private set; }
		public static Template Shockwave { get; private set; }
		public static Template Sun { get; private set; }

		public static void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(assets);

			Bullet = new Template
			(
				maxEnergy: 00000000f,
				maxHealth: 00002000f,
				radius: 00000016f,
				texture: null,
				cubeMap: null,
				model: null
			);

			Earth = new Template
			(
				maxEnergy: 00000000f,
				maxHealth: 00000001f,
				radius: 00000256f,
				texture: null,
				cubeMap: assets.LoadCubeMap("Textures/Planet"),
				model: Model.CreateSphere(graphicsDevice, 256, 32)
			);

			Gun = new Template
			(
				maxEnergy: 00001000f,
				maxHealth: 00000001f,
				radius: 00000000f,
				texture: null,
				cubeMap: null,
				model: null
			);

			Jupiter = new Template
			(
				maxEnergy: 00000000f,
				maxHealth: 00000001f,
				radius: 00000512f,
				texture: null,
				cubeMap: assets.LoadCubeMap("Textures/Jupiter"),
				model: Model.CreateSphere(graphicsDevice, 512, 64)
			);

			Mars = new Template
			(
				maxEnergy: 00000000f,
				maxHealth: 00000001f,
				radius: 00000128f,
				texture: null,
				cubeMap: assets.LoadCubeMap("Textures/Mars"),
				model: Model.CreateSphere(graphicsDevice, 128, 16)
			);

			Moon = new Template
			(
				maxEnergy: 00000000f,
				maxHealth: 00000001f,
				radius: 00000064f,
				texture: null,
				cubeMap: assets.LoadCubeMap("Textures/Moon"),
				model: Model.CreateSphere(graphicsDevice, 64, 8)
			);

			Phaser = new Template
			(
				maxEnergy: 00001000f,
				maxHealth: 00000001f,
				radius: 00000000f,
				texture: null,
				cubeMap: null,
				model: null
			);

			Ray = new Template
			(
				maxEnergy: 00000001f,
				maxHealth: 00000001f,
				radius: 00004096f,
				texture: null,
				cubeMap: null,
				model: null
			);

			Rocket = new Template
			(
				maxEnergy: 00001000f,
				maxHealth: 00000001f,
				radius: 00000016f,
				texture: null,
				cubeMap: null,
				model: null
			);

			Ship = new Template
			(
				maxEnergy: 00001000f,
				maxHealth: 00003000f,
				radius: 00000064f,
				texture: null,
				cubeMap: null,
				model: null
			);

			Shockwave = new Template
			(
				maxEnergy: 00000000f,
				maxHealth: 00000000f,
				radius: 00000000f,
				texture: null,
				cubeMap: null,
				model: null
			);

			Sun = new Template
			(
				maxEnergy: 00000000f,
				maxHealth: 00000001f,
				radius: 00001500f,
				texture: null,
				cubeMap: null,
				model: null
			);

		}

		public static void Dispose()
		{
			Bullet.SafeDispose();
			Earth.SafeDispose();
			Gun.SafeDispose();
			Jupiter.SafeDispose();
			Mars.SafeDispose();
			Moon.SafeDispose();
			Phaser.SafeDispose();
			Ray.SafeDispose();
			Rocket.SafeDispose();
			Ship.SafeDispose();
			Shockwave.SafeDispose();
			Sun.SafeDispose();
		}
	}
}
