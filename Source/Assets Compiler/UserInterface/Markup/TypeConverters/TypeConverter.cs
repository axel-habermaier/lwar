namespace Pegasus.AssetsCompiler.UserInterface.Markup.TypeConverters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Platform.Logging;

	/// <summary>
	///   Converts a string into a given type.
	/// </summary>
	internal abstract class TypeConverter
	{
		/// <summary>
		///   Maps types to their type converters.
		/// </summary>
		private static Dictionary<Type, TypeConverter> _converters;

		/// <summary>
		///   Gets the type the string value is converted to.
		/// </summary>
		protected abstract Type TargetType { get; }

		/// <summary>
		///   Gets the runtime type for the given value.
		/// </summary>
		protected abstract string RuntimeType { get; }

		/// <summary>
		///   Converts the given string value into an instance of the target type.
		/// </summary>
		/// <param name="xamlFile">The Xaml file the value is specified in.</param>
		/// <param name="value">The value that should be converted.</param>
		protected abstract object Convert(XamlFile xamlFile, string value);

		/// <summary>
		///   Generates the code for the object value.
		/// </summary>
		/// <param name="value">The value the code should be generated for.</param>
		protected abstract string GenerateInstantiationCode(object value);

		/// <summary>
		///   Converts the given string value into an instance of the given type.
		/// </summary>
		/// <param name="xamlFile">The Xaml file the value is specified in.</param>
		/// <param name="targetType">The target type of the conversion.</param>
		/// <param name="value">The value that should be converted.</param>
		public static object Convert(XamlFile xamlFile, Type targetType, string value)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(targetType);
			Assert.ArgumentNotNull(value);

			if (_converters == null)
				_converters = Configuration.GetReflectionTypes()
										   .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TypeConverter)))
										   .Select(Activator.CreateInstance)
										   .Cast<TypeConverter>()
										   .ToDictionary(c => c.TargetType, c => c);

			TypeConverter converter;
			if (!_converters.TryGetValue(targetType, out converter))
				Log.Die("Unable to find a type converter for type '{0}'.", targetType.FullName);

			try
			{
				return converter.Convert(xamlFile, value.Trim());
			}
			catch (Exception e)
			{
				Log.Die("Failed to convert '{0}' to type '{1}': {2}", value, targetType.FullName, e.Message);
				return null;
			}
		}

		/// <summary>
		///   Generates the code for the object value.
		/// </summary>
		/// <param name="value">The value the code should be generated for.</param>
		public static string GenerateCode(object value)
		{
			Assert.ArgumentNotNull(value);
			Assert.NotNull(_converters);

			TypeConverter converter;
			if (!_converters.TryGetValue(value.GetType(), out converter))
				Log.Die("Unable to find a type converter for type '{0}'.", value.GetType().FullName);

			return converter.GenerateInstantiationCode(value);
		}

		/// <summary>
		///   Gets the runtime type for the given value.
		/// </summary>
		/// <param name="type">The type the runtime type should be returned for.</param>
		public static string GetRuntimeType(Type type)
		{
			Assert.ArgumentNotNull(type);
			Assert.NotNull(_converters);

			TypeConverter converter;
			if (!_converters.TryGetValue(type, out converter))
				Log.Die("Unable to find a type converter for type '{0}'.", type.FullName);

			return converter.RuntimeType;
		}
	}
}