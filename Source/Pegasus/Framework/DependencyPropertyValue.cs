﻿namespace Pegasus.Framework
{
	using System;

	/// <summary>
	///   Represents the base class for a dependency property value.
	/// </summary>
	internal abstract class DependencyPropertyValue : SparseObjectStorage<DependencyPropertyValue>.IStorageLocation
	{
		/// <summary>
		///   The sources of the property's values.
		/// </summary>
		protected ValueSources _sources;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="property">The dependency property the value belongs to.</param>
		protected DependencyPropertyValue(DependencyProperty property)
		{
			Assert.ArgumentNotNull(property);
			Property = property;
		}

		/// <summary>
		///   Gets the dependency property the value belongs to.
		/// </summary>
		public DependencyProperty Property { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the property has an effective value.
		/// </summary>
		public bool HasEffectiveValue
		{
			get { return _sources != 0; }
		}

		/// <summary>
		///   Gets a value indicating whether the property has an inherited value.
		/// </summary>
		public bool HasInheritedValue
		{
			get { return (_sources & ValueSources.Inherited) == ValueSources.Inherited; }
		}

		/// <summary>
		///   Gets a value indicating whether the property has a bound value.
		/// </summary>
		public bool IsBound
		{
			get { return (_sources & ValueSources.Binding) == ValueSources.Binding; }
		}

		/// <summary>
		///   Gets the storage location of the dependency property value.
		/// </summary>
		public int Location
		{
			get { return Property.Index; }
		}

		/// <summary>
		///   Identifies the sources of the property's values.
		/// </summary>
		[Flags]
		protected enum ValueSources : byte
		{
			/// <summary>
			///   Indicates that the base value has been set directly or through a binding.
			/// </summary>
			Local = 1,

			/// <summary>
			///   Indicates that the base value has been set by a style.
			/// </summary>
			Style = 2,

			/// <summary>
			///   Indicates that the triggered value has been set by a style trigger.
			/// </summary>
			StyleTrigger = 4,

			/// <summary>
			///   Indicates that the triggered value has been set by a template trigger.
			/// </summary>
			TemplateTrigger = 8,

			/// <summary>
			///   Indicates that the animated value has been set.
			/// </summary>
			Animation = 16,

			/// <summary>
			///   Indicates that the value has been inherited from a parent object.
			/// </summary>
			Inherited = 32,

			/// <summary>
			///   Indicates that the value is determined by a binding.
			/// </summary>
			Binding = 64
		}
	}
}