using System;

namespace Lwar.Client
{
	using Pegasus.Framework;

	/// <summary>
	///   Starts up and configures the application.
	/// </summary>
	internal class LwarBootstrapper : Bootstrapper<LwarApp>
	{
		/// <summary>
		///   Gets the name of the application.
		/// </summary>
		protected override string AppName
		{
			get { return "lwar"; }
		}

		/// <summary>
		///   Entry point of the application.
		/// </summary>
		private static void Main()
		{
			new LwarBootstrapper().Run();
		}
	}
}