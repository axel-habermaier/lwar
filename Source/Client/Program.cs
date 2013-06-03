﻿using System;

namespace Lwar.Client
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
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
			var context = new AppContext
			{
				AppName = "lwar",
				DefaultFontName = "Fonts/Liberation Mono 12",
				SpriteEffect = new SpriteEffectAdapter(),
				Statistics = new Statistics()
			};

			Commands.Initialize();
			Cvars.Initialize();
			Bootstrapper<LwarClient>.Run(context);
		}
	}
}