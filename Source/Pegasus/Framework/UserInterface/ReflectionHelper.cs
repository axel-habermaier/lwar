namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	///     Provides helper methods for tasks involving reflection as well as default reflection info objects for often reflected
	///     methods and properties.
	/// </summary>
	internal static class ReflectionHelper
	{
		/// <summary>
		///     A cached event info instance for the INotifyPropertyChanged.PropertyChanged event.
		/// </summary>
		private static readonly EventInfo PropertyChangedEventInfo = typeof(INotifyPropertyChanged).GetRuntimeEvent("PropertyChanged");

		/// <summary>
		///     A cached method info instance for the Object.ToString() method.
		/// </summary>
		public static readonly MethodInfo ToStringMethodInfo = typeof(object).GetRuntimeMethod("ToString", new Type[] { });

		/// <summary>
		///     In debug builds, checks whether all cached reflection info objects could be resolved successfully.
		/// </summary>
		[Conditional("DEBUG")]
		public static void Validate()
		{
			Assert.NotNull(PropertyChangedEventInfo, "Unable to find the INotifyPropertyChanged.PropertyChanged event.");
			Assert.NotNull(ToStringMethodInfo, "Unable to find the Object.ToString() method.");
		}

		/// <summary>
		///     Attaches the given handler to the given object's property changed event.
		/// </summary>
		/// <param name="obj">The object the handler should be attached to.</param>
		/// <param name="handler">The handler that should be attached.</param>
		public static void AttachPropertyChangedEventHandler(INotifyPropertyChanged obj, PropertyChangedHandler handler)
		{
			Assert.ArgumentNotNull(obj);
			Assert.ArgumentNotNull(handler);

			PropertyChangedEventInfo.AddEventHandler(obj, handler);
		}

		/// <summary>
		///     Detaches the given handler from the given object's property changed event.
		/// </summary>
		/// <param name="obj">The object the handler should be detached from.</param>
		/// <param name="handler">The handler that should be detached.</param>
		public static void DetachPropertyChangedEventHandler(INotifyPropertyChanged obj, PropertyChangedHandler handler)
		{
			Assert.ArgumentNotNull(obj);
			Assert.ArgumentNotNull(handler);

			PropertyChangedEventInfo.RemoveEventHandler(obj, handler);
		}

		/// <summary>
		///     Gets the instance of the dependency property with the given name or null if it could not be found.
		/// </summary>
		/// <param name="type">The type of the dependency object that declares the dependency property.</param>
		/// <param name="propertyName">The name of the dependency property without the 'Property' suffix.</param>
		public static DependencyProperty GetDependencyProperty(Type type, string propertyName)
		{
			Assert.ArgumentNotNull(type);
			Assert.ArgumentNotNullOrWhitespace(propertyName);

			var fieldName = String.Format("{0}Property", propertyName);
			var propertyField = type.GetRuntimeFields().SingleOrDefault(f => f.IsPublic && f.IsStatic && f.Name == fieldName);

			// For some reason, inherited static fields are not returned by GetRuntimeFields(), so let's check the base
			// types explicitly if we didn't find a matching dependency property field on the current type
			var baseType = type.GetTypeInfo().BaseType;
			if (propertyField == null && baseType != typeof(object))
				return GetDependencyProperty(baseType, propertyName);

			Assert.NotNull(propertyField, "Unable to find dependency property '{0}' on '{1}'.", propertyName, type.FullName);
			return (DependencyProperty)propertyField.GetValue(null);
		}
	}
}