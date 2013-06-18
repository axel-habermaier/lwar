using System;
using Lwar.Assets.Templates.Compilation;
using Pegasus.AssetsCompiler.Assets.Attributes;

#region Ignored files

[assembly: Ignore("CompilationSettings.cs")]
[assembly: Ignore("Templates/Compilation/TemplateAsset.cs")]
[assembly: Ignore("Templates/Compilation/TemplatesAttribute.cs")]
[assembly: Ignore("Templates/Compilation/TemplateCompiler.cs")]
[assembly: Ignore("Templates/Compilation/Template.cs")]

#endregion

#region Cubemaps

[assembly: CubeMap("Textures/Sun.png")]
[assembly: CubeMap("Textures/Earth.png")]
[assembly: CubeMap("Textures/Mars.png")]
[assembly: CubeMap("Textures/Moon.png")]
[assembly: CubeMap("Textures/Jupiter.png")]
[assembly: CubeMap("Textures/Shields.png")]
[assembly: CubeMap("Textures/SunHeat.png")]
[assembly: CubeMap("Textures/Space.png", Mipmaps = false, Uncompressed = false)]

#endregion

#region 2D Textures

[assembly: Texture2D("Textures/Explosion.png", Mipmaps = false, Uncompressed = false)]
[assembly: Texture2D("Textures/Heat.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/Phaser.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/PhaserGlow.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/Bullet.png", Mipmaps = false, Uncompressed = true)]
[assembly: Texture2D("Textures/BulletGlow.png", Mipmaps = false, Uncompressed = true)]

#endregion

#region Templates

[assembly: Templates("Templates/Planets.cs")]
[assembly: Templates("Templates/Ships.cs")]
[assembly: Templates("Templates/Suns.cs")]
[assembly: Templates("Templates/Weapons.cs")]

#endregion