namespace Pegasus.Scripting
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	///     Provides access to all cvars.
	/// </summary>
	public static class CvarRegistry
	{
		/// <summary>
		///     The registered cvars.
		/// </summary>
		private static readonly Dictionary<string, ICvar> Cvars = new Dictionary<string, ICvar>();

		/// <summary>
		///     Gets all registered cvars.
		/// </summary>
		internal static IEnumerable<ICvar> All
		{
			get { return Cvars.Values; }
		}

		/// <summary>
		///     Registers the given cvar.
		/// </summary>
		/// <param name="cvar">The cvar that should be registered.</param>
		public static void Register(ICvar cvar)
		{
			Assert.ArgumentNotNull(cvar);
			Assert.NotNullOrWhitespace(cvar.Name, "The cvar cannot have an empty name.");
			Assert.That(!Cvars.ContainsKey(cvar.Name), "A cvar with the name '{0}' has already been registered.", cvar.Name);
			Assert.That(CommandRegistry.All.All(command => command.Name != cvar.Name),
						"A command with the name '{0}' has already been registered.", cvar.Name);

			Cvars.Add(cvar.Name, cvar);
		}

		/// <summary>
		///     Finds the cvar with the given name. Returns false if no such cvar is found.
		/// </summary>
		/// <param name="name">The name of the cvar that should be returned.</param>
		/// <param name="cvar">The cvar with the given name, if it is found.</param>
		internal static bool TryFind(string name, out ICvar cvar)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			return Cvars.TryGetValue(name, out cvar);
		}

		/// <summary>
		///     Resolves the cvar reference to the cvar with the given type and name.
		/// </summary>
		/// <param name="name">The name of the cvar that should be returned.</param>
		public static Cvar<T> Resolve<T>(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.That(Cvars.ContainsKey(name), "Could not resolve cvar '{0}'. Rerun the T4 templates.", name);
			Assert.That(Cvars[name] is Cvar<T>, "Cvar '{0}' has an unexpected type. Rerun the T4 templates.", name);

			return Cvars[name] as Cvar<T>;
		}

		/// <summary>
		///     Executes all deferred cvar updates for cvars with the given update mode.
		/// </summary>
		/// <param name="mode">The mode of the cvars that should be updated.</param>
		internal static void ExecuteDeferredUpdates(UpdateMode mode)
		{
			Assert.InRange(mode);

			foreach (var cvar in Cvars.Values.Where(cvar => cvar.UpdateMode == mode && cvar.HasDeferredValue))
				cvar.SetDeferredValue();
		}

		/// <summary>
		///     Executes all deferred cvar updates.
		/// </summary>
		internal static void ExecuteDeferredUpdates()
		{
			foreach (var cvar in Cvars.Values.Where(cvar => cvar.UpdateMode != UpdateMode.Immediate && cvar.HasDeferredValue))
				cvar.SetDeferredValue();
		}
	}
}