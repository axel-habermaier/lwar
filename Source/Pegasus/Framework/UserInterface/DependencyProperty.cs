namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	///     Represents an untyped property that has multiple sources (such as data bindings, style setters, animation,
	///     etc.).
	/// </summary>
	public abstract class DependencyProperty
	{
		/// <summary>
		///     The number of dependency properties that have been registered throughout the lifetime of the application.
		/// </summary>
		private static int _propertyCount;

		/// <summary>
		///     The list of all dependency properties that have been created, sorted by dependency property index.
		/// </summary>
		internal static readonly List<DependencyProperty> DependencyProperties = new List<DependencyProperty>();

		/// <summary>
		///     The metadata flags of the dependency property.
		/// </summary>
		private readonly MetadataFlags _flags;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="inherits">Indicates whether the value of the dependency property is inheritable.</param>
		/// <param name="affectsMeasure">
		///     Indicates that changes to the value of the dependency property potentially affect the measure pass of the layout
		///     engine. Implies affectsArrange and affectsRender.
		/// </param>
		/// <param name="affectsArrange">
		///     Indicates that changes to the value of the dependency property potentially affect the arrange pass of the layout
		///     engine. Implies affectsRender.
		/// </param>
		/// <param name="affectsRender">
		///     Indicates that changes to the value of the dependency property potentially requires a redraw, without affecting
		///     measurement or arrangement.
		/// </param>
		/// <param name="prohibitsAnimations">Indicates that the dependency property cannot be animated.</param>
		/// <param name="prohibitsDataBinding">Indicates that the dependency property does not support data binding.</param>
		/// <param name="isReadOnly">
		///     Indicates that the dependency property is read-only and can only be set internally by the
		///     framework.
		/// </param>
		/// <param name="defaultBindingMode">The default data binding mode of the dependency property.</param>
		protected DependencyProperty(bool inherits,
									 bool affectsMeasure,
									 bool affectsArrange,
									 bool affectsRender,
									 bool prohibitsAnimations,
									 bool prohibitsDataBinding,
									 bool isReadOnly,
									 BindingMode defaultBindingMode)
		{
			Assert.ArgumentInRange(defaultBindingMode);
			Assert.ArgumentSatisfies(defaultBindingMode != BindingMode.Default, "Unsupported binding mode.");

			DefaultBindingMode = defaultBindingMode;
			Index = _propertyCount++;

			if (inherits)
				_flags |= MetadataFlags.Inherits;

			if (affectsMeasure)
				_flags |= MetadataFlags.AffectsMeasure;

			if (affectsArrange)
				_flags |= MetadataFlags.AffectsArrange;

			if (affectsRender)
				_flags |= MetadataFlags.AffectsRender;

			if (prohibitsAnimations)
				_flags |= MetadataFlags.IsAnimationProhibited;

			if (prohibitsDataBinding)
				_flags |= MetadataFlags.IsDataBindingProhibited;

			if (isReadOnly)
				_flags |= MetadataFlags.IsReadOnly;

			DependencyProperties.Add(this);
		}

		/// <summary>
		///     Gets the default data binding mode of the dependency property.
		/// </summary>
		public BindingMode DefaultBindingMode { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the value of the dependency property is potentially inherited.
		/// </summary>
		public bool Inherits
		{
			get { return (_flags & MetadataFlags.Inherits) == MetadataFlags.Inherits; }
		}

		/// <summary>
		///     Gets a value indicating whether changes to the value of the dependency property potentially affect the measure pass
		///     of the layout engine.
		/// </summary>
		public bool AffectsMeasure
		{
			get { return (_flags & MetadataFlags.AffectsMeasure) == MetadataFlags.AffectsMeasure; }
		}

		/// <summary>
		///     Gets a value indicating whether changes to the value of the dependency property potentially affect the arrange pass
		///     of the layout engine.
		/// </summary>
		public bool AffectsArrange
		{
			get { return (_flags & MetadataFlags.AffectsArrange) == MetadataFlags.AffectsArrange || AffectsMeasure; }
		}

		/// <summary>
		///     Gets a value indicating whether changes to the value of the dependency property potentially requires a redraw,
		///     without affecting measurement or arrangement.
		/// </summary>
		public bool AffectsRender
		{
			get { return (_flags & MetadataFlags.AffectsRender) == MetadataFlags.AffectsRender || AffectsArrange || AffectsMeasure; }
		}

		/// <summary>
		///     Gets a value indicating whether the dependency property can be animated.
		/// </summary>
		public bool IsAnimationProhibited
		{
			get { return (_flags & MetadataFlags.IsAnimationProhibited) == MetadataFlags.IsAnimationProhibited; }
		}

		/// <summary>
		///     Gets a value indicating whether the dependency property supports data binding.
		/// </summary>
		public bool IsDataBindingProhibited
		{
			get { return (_flags & MetadataFlags.IsDataBindingProhibited) == MetadataFlags.IsDataBindingProhibited; }
		}

		/// <summary>
		///     Gets a value indicating whether the dependency property is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return (_flags & MetadataFlags.IsReadOnly) == MetadataFlags.IsReadOnly; }
		}

		/// <summary>
		///     The index of the dependency property that remains unchanged and unique throughout the lifetime of the application.
		/// </summary>
		internal int Index { get; private set; }

		/// <summary>
		///     Gets the type of the value stored by the dependency property.
		/// </summary>
		internal abstract Type ValueType { get; }

		/// <summary>
		///     Adds an untyped changed handler to the dependency property for the given dependency object. Returns the delegate that
		///     must can used to remove the untyped change handler.
		/// </summary>
		/// <param name="obj">The dependency object the handler should be added to.</param>
		/// <param name="handler">The handler that should be added.</param>
		internal abstract Delegate AddUntypedChangeHandler(DependencyObject obj, DependencyPropertyChangedHandler handler);

		/// <summary>
		///     Removes an untyped changed handler from the dependency property for the given dependency object.
		/// </summary>
		/// <param name="obj">The dependency object the handler should be removed from.</param>
		/// <param name="handler">The handler that should be removed.</param>
		internal abstract void RemoveUntypedChangeHandler(DependencyObject obj, Delegate handler);

		/// <summary>
		///     Gets the untyped value of the dependency property for the given dependency object.
		/// </summary>
		/// <param name="obj">The dependency object the value should be returned for.</param>
		internal abstract object GetValue(DependencyObject obj);

		/// <summary>
		///     Copies the dependency property's inherited value from source to target.
		/// </summary>
		/// <param name="source">The dependency object the value should be retrieved from.</param>
		/// <param name="target">The dependency object the value should be set for.</param>
		internal abstract void CopyInheritedValue(DependencyObject source, DependencyObject target);

		/// <summary>
		///     Unsets the inherited value of the given dependency object.
		/// </summary>
		/// <param name="obj">The dependency object whose inherited value should be unset.</param>
		internal abstract void UnsetInheritedValue(DependencyObject obj);

		/// <summary>
		///     Provides metadata for the dependency property.
		/// </summary>
		[Flags]
		private enum MetadataFlags : byte
		{
			/// <summary>
			///     Indicates that the value of the dependency property is potentially inherited.
			/// </summary>
			Inherits = 1,

			/// <summary>
			///     Indicates that changes to the value of the dependency property potentially affect the measure pass of the layout
			///     engine. Implies affectsArrange and affectsRender.
			/// </summary>
			AffectsMeasure = 2,

			/// <summary>
			///     Indicates that changes to the value of the dependency property potentially affect the arrange pass of the layout
			///     engine. Implies affectsRender.
			/// </summary>
			AffectsArrange = 4,

			/// <summary>
			///     Indicates that changes to the value of the dependency property potentially requires a redraw, without affecting
			///     measurement or arrangement.
			/// </summary>
			AffectsRender = 8,

			/// <summary>
			///     Indicates that the dependency property cannot be animated.
			/// </summary>
			IsAnimationProhibited = 16,

			/// <summary>
			///     Indicates that the dependency property does not support data binding.
			/// </summary>
			IsDataBindingProhibited = 32,

			/// <summary>
			///     Indicates that the dependency property is read-only and can only be set internally by the framework.
			/// </summary>
			IsReadOnly
		}
	}
}