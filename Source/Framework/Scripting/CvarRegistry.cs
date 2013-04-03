using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;

	/// <summary>
	///   Provides access to all cvar instances.
	/// </summary>
	public class CvarRegistry
	{
		/// <summary>
		///   The cvar instances.
		/// </summary>
		private static readonly Dictionary<string, ICvar> RegisteredCvars = new Dictionary<string, ICvar>();

		/// <summary>
		///   Registers the given cvar.
		/// </summary>
		/// <param name="cvar">The cvar that should be registered.</param>
		internal static void Register(ICvar cvar)
		{
			Assert.ArgumentNotNull(cvar, () => cvar);
			Assert.ArgumentSatisfies(!String.IsNullOrWhiteSpace(cvar.Name), () => cvar, "Invalid cvar name.");
			Assert.That(CommandRegistry.Find(cvar.Name) == null, "A command with the same name already exists.");
			Assert.That(!RegisteredCvars.ContainsKey(cvar.Name), "A cvar called '{0}' has already been registered.", cvar.Name);

			RegisteredCvars.Add(cvar.Name, cvar);
		}

		/// <summary>
		///   Registers the given cvar.
		/// </summary>
		/// <param name="cvar">The cvar that should be registered.</param>
		internal static void Register(ICommand cvar)
		{
			//Assert.ArgumentNotNull(cvar, () => cvar);
			//Assert.ArgumentSatisfies(!String.IsNullOrWhiteSpace(cvar.Name), () => cvar, "Invalid cvar name.");
			//Assert.That(CommandRegistry.Find(cvar.Name) == null, "A command with the same name already exists.");
			//Assert.That(!RegisteredCvars.ContainsKey(cvar.Name), "A cvar called '{0}' has already been registered.", cvar.Name);

			//RegisteredCvars.Add(cvar.Name, cvar);
		}

		/// <summary>
		///   Finds the cvar instance with the given name. Returns null if no such cvar exists.
		/// </summary>
		/// <param name="name">The name of the cvar that should be returned.</param>
		internal static ICvar Find(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name, () => name);

			ICvar cvar;
			if (!RegisteredCvars.TryGetValue(name, out cvar))
				return null;

			return cvar;
		}
	}
}