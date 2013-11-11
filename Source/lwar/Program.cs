namespace Lwar
{
	using System;
	using Assets;
	using Pegasus.Framework;
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

			Bootstrapper<App>.Run("lwar", Fonts.LiberationMono11);
		}
	}
}