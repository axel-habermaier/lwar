using System;

namespace Lwar
{
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

			Bootstrapper<LwarApp>.Run("lwar", "Fonts/Liberation Mono 12", new SpriteEffect());
		}
	}
}