﻿using System;

namespace Lwar.Assets
{
	using Pegasus.AssetsCompiler;
	using Pegasus.AssetsCompiler.Assets;

	/// <summary>
	///   Represents a compilation unit that compiles all assets into a binary format.
	/// </summary>
	public class LwarCompilationUnit : CompilationUnit
	{
		/// <summary>
		///   Allows overriding the default compilation settings for all or some assets.
		/// </summary>
		protected override void AddSpecialAssets()
		{
			Add(new CubeMapAsset("Textures/Sun.png"));
			Add(new CubeMapAsset("Textures/SunHeat.png"));
		}
	}
}