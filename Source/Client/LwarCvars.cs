using System;

namespace Lwar.Client
{
	using System.ComponentModel;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Provides access to lwar-specific cvars.
	/// </summary>
	[ForceInitialization]
	public static class LwarCvars
	{
		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		public static readonly Cvar<bool> DrawWireframe = new Cvar<bool>("r_wireframe", false,
																		 "If true, all 3D geometry is drawn in wireframe mode.");

		[Description("If true, all 3D geometry is drawn in wireframe mode.")]
		[Cvar(CvarCategory.Rendering, "r_wireframe", CvarFlags.Constant)]
		public static Cvar<bool> DrawWireframe2;
	}

	[MeansImplicitUse()]
	internal class CvarAttribute : Attribute

	{
		public CvarAttribute(CvarCategory c,string n, CvarFlags f)
		{
		}
	}

	enum CvarCategory
	{
		Rendering,
		System,
	}

	internal enum CvarFlags
	{
		System,
		Constant,
		Persist,
	}
}