namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Diagnostics;
	using System.Reflection;

	/// <summary>
	///   Provides helper methods for tasks involving reflection as well as default reflection info objects for often reflected
	///   methods and properties.
	/// </summary>
	internal static class ReflectionHelper
	{
		/// <summary>
		///   A cached event info instance for the INotifyPropertyChanged.PropertyChanged event.
		/// </summary>
		private static readonly EventInfo PropertyChangedEventInfo = typeof(INotifyPropertyChanged).GetEvent("PropertyChanged");

		/// <summary>
		///   A cached property info instance for the UIElement.ViewModel property.
		/// </summary>
		public static readonly PropertyInfo ViewModelPropertyInfo = typeof(UIElement).GetProperty("ViewModel");

		/// <summary>
		///   In debug builds, checks whether all cached reflection info objects could be resolved successfully.
		/// </summary>
		[Conditional("DEBUG")]
		public static void Validate()
		{
			Assert.NotNull(PropertyChangedEventInfo, "Unable to find the INotifyPropertyChanged.PropertyChanged event.");
			Assert.NotNull(ViewModelPropertyInfo, "Unable to find the UIElement.ViewModel property.");
		}

		/// <summary>
		///   Attaches the given handler to the given object's property changed event.
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
		///   Detaches the given handler from the given object's property changed event.
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
		///   Gets the instance of the dependency property with the given name or null if it could not be found.
		/// </summary>
		/// <param name="type">The type of the dependency object that declares the dependency property.</param>
		/// <param name="propertyName">The name of the dependency property without the 'Property' suffix.</param>
		public static DependencyProperty GetDependencyProperty(Type type, string propertyName)
		{
			Assert.ArgumentNotNull(type);
			Assert.ArgumentNotNullOrWhitespace(propertyName);

			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

			var fieldName = String.Format("{0}Property", propertyName);
			var propertyField = type.GetField(fieldName, bindingFlags);

			if (propertyField == null)
				return null;

			return (DependencyProperty)propertyField.GetValue(null);
		}
	}
}