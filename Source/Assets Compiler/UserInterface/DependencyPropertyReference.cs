namespace Pegasus.AssetsCompiler.UserInterface
{
	using System;
	using System.Reflection;
	using Markup.TypeConverters;

	/// <summary>
	///   Represents a reference to a dependency property.
	/// </summary>
	internal class DependencyPropertyReference
	{
		/// <summary>
		///   The referenced property.
		/// </summary>
		private readonly PropertyInfo _property;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="property">The property that should be referenced.</param>
		public DependencyPropertyReference(PropertyInfo property)
		{
			Assert.ArgumentNotNull(property);
			_property = property;
		}

		/// <summary>
		///   Gets the runtime type of the property.
		/// </summary>
		public string RuntimePropertyType
		{
			get { return TypeConverter.GetRuntimeType(_property.PropertyType); }
		}

		/// <summary>
		///   Gets the runtime declaring type of the property.
		/// </summary>
		public string RuntimeDeclaringType
		{
			get { return _property.DeclaringType.GetRuntimeType(); }
		}

		/// <summary>
		///   Gets the type of the property.
		/// </summary>
		public Type PropertyType
		{
			get { return _property.PropertyType; }
		}

		/// <summary>
		///   Gets the declaring type of the property.
		/// </summary>
		public Type DeclaringType
		{
			get { return _property.DeclaringType; }
		}

		/// <summary>
		///   Gets the name of the property.
		/// </summary>
		public string Name
		{
			get { return _property.Name; }
		}
	}
}