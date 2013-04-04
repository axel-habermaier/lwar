using System;

namespace Lwar.Client
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Rendering;
	using Scripting;

	/// <summary>
	///   Starts up and configures the application.
	/// </summary>
	internal static class Bootstrapper
	{
		/// <summary>
		///   The entry point of the application.
		/// </summary>
		private static void Main()
		{
			var context = new AppContext
			{
				AppName = "lwar",
				Commands = new CommandRegistry(),
				Cvars = new CvarRegistry(),
				DefaultFontName = "Fonts/Liberation Mono 12",
				SpriteEffect = new SpriteEffectAdapter(),
				Statistics = new Statistics()
			};

			Bootstrapper<App>.Run(context);
		}
	}
}