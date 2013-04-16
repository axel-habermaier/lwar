using System;
using Pegasus.AssetsCompiler.Assets.Attributes;

[assembly: Ignore("CompilationSettings.cs")]
[assembly: CubeMap("Textures/Sun.png")]
[assembly: CubeMap("Textures/SunHeat.png")]
[assembly: CubeMap("Textures/Space.png", Mipmaps = false, Uncompressed = false)]
[assembly: Texture2D("Textures/Heat.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/Phaser.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/PhaserGlow.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/Bullet.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/BulletGlow.png", Mipmaps = false, Uncompressed = true)]