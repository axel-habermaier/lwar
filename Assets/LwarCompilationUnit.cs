using System;

namespace Lwar.Assets
{
	using Pegasus.Framework.Platform.Assets.Compilation;

	public class LwarCompilationUnit : CompilationUnit
	{
		/// <summary>
		///   Allows overriding the default compilation settings for all or some assets.
		/// </summary>
		protected override void AddSpecialAssets()
		{
			Add(new CubeMapCompiler("Textures/Sun.png"));
			Add(new CubeMapCompiler("Textures/SunHeat.png"));
		}
	}
}