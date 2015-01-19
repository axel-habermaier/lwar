namespace Pegasus.Utilities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	///     Provides access to all public types exported by the registered assemblies.
	/// </summary>
	internal static class AssemblyCache
	{
		/// <summary>
		///     The registered assemblies.
		/// </summary>
		private static readonly List<Assembly> Assemblies = new List<Assembly>();

		/// <summary>
		///     Registers the given assembly.
		/// </summary>
		/// <param name="assembly">The assembly that should be registered.</param>
		public static void Register(Assembly assembly)
		{
			Assert.ArgumentNotNull(assembly);
			
			if (!Assemblies.Contains(assembly))
				Assemblies.Add(assembly);
		}

		/// <summary>
		///     Searches for all exported types in all registered assemblies implementing the given interface or inheriting the given
		///     base class and returns an instance for each of these types.
		/// </summary>
		/// <typeparam name="T">The base type or interface of the types that should be instantiated.</typeparam>
		/// <param name="args">The parameters that should be passed to the constructors.</param>
		public static IEnumerable<T> CreateInstancesOfType<T>(params object[] args)
		{
			return from assembly in Assemblies
				   from type in assembly.DefinedTypes
				   where type.IsClass && !type.IsAbstract && typeof(T).GetTypeInfo().IsAssignableFrom(type)
				   select (T)Activator.CreateInstance(type.AsType(), args);
		}
	}
}