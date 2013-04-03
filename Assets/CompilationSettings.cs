using System;
using Pegasus.AssetsCompiler.Assets.Attributes;

[assembly: Ignore("CompilationSettings.cs")]
[assembly: CubeMap("Textures/Sun.png")]
[assembly: CubeMap("Textures/SunHeat.png")]
[assembly: CubeMap("Textures/Space.png", Mipmaps = false, Uncompressed = false)]
[assembly: Texture2D("Textures/Heat.png", Mipmaps = false, Uncompressed = true)]