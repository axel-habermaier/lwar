using Pegasus.AssetsCompiler.Assets.Attributes;

[assembly: Ignore("Textures/CompilationSettings.cs")]

#region Cubemaps

[assembly: CubeMap("Textures/Space.Cubemap.png", Mipmaps = false, Uncompressed = false)]

#endregion

#region 2D Textures

[assembly: Texture2D("Textures/Explosion.png", Mipmaps = false, Uncompressed = false)]
[assembly: Texture2D("Textures/Heat.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/Phaser.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/PhaserGlow.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/Bullet.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/BulletGlow.png", Mipmaps = false, Uncompressed = true)]

#endregion