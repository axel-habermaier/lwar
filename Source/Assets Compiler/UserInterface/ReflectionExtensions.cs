namespace Pegasus.AssetsCompiler.UserInterface
{
	using System;
	using System.Linq;
	using System.Reflection;
	using Platform.Logging;

	/// <summary>
	///   Provides extension methods for reflection types.
	/// </summary>
	internal static class ReflectionExtensions
	{
		/// <summary>
		///   Gets the custom attributes of type T defined on the type, property, field, etc.
		/// </summary>
		/// <typeparam name="T">The type of the custom attribute.</typeparam>
		/// <param name="memberInfo">The type, property, field, etc. the custom attributes should be returned for.</param>
		/// <param name="inherit">Indicates whether the inheritance chain should be searched.</param>
		/// <remarks>Even though a similar method is available in .NET, Mono doesn't provide such a method.</remarks>
		public static T[] GetCustomAttributes<T>(this MemberInfo memberInfo, bool inherit = true)
			where T : Attribute
		{
			Assert.ArgumentNotNull(memberInfo);

			return memberInfo.GetCustomAttributes(typeof(T), inherit)
							 .OfType<T>()
							 .ToArray();
		}

		/// <summary>
		///   Gets the custom attribute of type T defined on the type, property, field, etc. Returns null if there is no attribute
		///   of the givne type or throws an exception if there is more than one attribute of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the custom attribute.</typeparam>
		/// <param name="memberInfo">The type, property, field, etc. the custom attributes should be returned for.</param>
		/// <param name="inherit">Indicates whether the inheritance chain should be searched.</param>
		/// <remarks>Even though a similar method is available in .NET, Mono doesn't provide such a method.</remarks>
		public static T GetCustomAttribute<T>(this MemberInfo memberInfo, bool inherit = true)
			where T : Attribute
		{
			var attributes = memberInfo.GetCustomAttributes<T>(inherit);
			if (attributes.Length == 0)
				return null;

			if (attributes.Length > 1)
				Log.Die("Found more than one attribute of type '{0}' on '{1}'.", typeof(T).FullName, memberInfo.Name);

			return attributes[0];
		}

		/// <summary>
		///   Gets the runtime type for the given type.
		/// </summary>
		/// <param name="type">The type the runtime type should be retrieved for.</param>
		public static string GetRuntimeType(this Type type)
		{
			Assert.ArgumentNotNull(type);

			var runtimeNamespace = type.GetCustomAttribute<RuntimeNamespaceAttribute>();
			if (runtimeNamespace == null)
				return type.FullName;

			return String.Format("{0}.{1}", runtimeNamespace.Name, type.Name);
		}
	}
}