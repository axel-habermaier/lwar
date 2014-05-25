// ReSharper disable CheckNamespace

namespace Pegasus.Scripting
{
	using System;

	/// <summary>
	///     Dummy implementation for the asset compiler.
	/// </summary>
	public static class TypeRegistry
	{
		/// <summary>
		///     Gets the user-friendly description of the given type.
		/// </summary>
		/// <typeparam name="T">The type for which the user-friendly description should be returned.</typeparam>
		internal static string GetDescription<T>()
		{
			return typeof(T).Name;
		}
	}
}