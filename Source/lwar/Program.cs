using System;

namespace Lwar
{
	using Assets;
	using Pegasus.Framework;
	using Rendering;
	using Scripting;

	/// <summary>
	///   Starts up and configures the application.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		///   The entry point of the application.
		/// </summary>
		private static void Main()
		{
			Commands.Initialize();
			Cvars.Initialize();

			Bootstrapper<LwarApp>.Run("lwar", Fonts.LiberationMono11, new SpriteEffect());
		}
	}
}