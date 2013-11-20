namespace Pegasus.Framework.UserInterface
{
	using System;
	using Controls;
	using Math;
	using Rendering.UserInterface;

	/// <summary>
	///   Provides layouting, input, and other base functionality for all UI elements.
	/// </summary>
	public abstract partial class UIElement : Visual
	{
		/// <summary>
		///   The cached font instance that is currently being used for text rendering.
		/// </summary>
		private Font _cachedFont;

		/// <summary>
		///   The size of the UI element that has been computed by the last measure pass of the layout engine.
		/// </summary>
		private SizeD _desiredSize;

		/// <summary>
		///   Stores the handlers of the UI element's routed events.
		/// </summary>
		private RoutedEventStore _eventStore = new RoutedEventStore();

		/// <summary>
		///   Caches the layouting information of the UI element during the measure and arrange phases for performance reasons.
		/// </summary>
		private LayoutInfo _layoutInfo;

		/// <summary>
		///   The resources used by the UI element.
		/// </summary>
		private ResourceDictionary _resources = new ResourceDictionary();

		/// <summary>
		///   A value indicating whether the UI element uses and implicitly set style.
		/// </summary>
		private bool _usesImplicitStyle = true;

		/// <summary>
		///   Initializes the type.
		/// </summary>
		static UIElement()
		{
			StyleProperty.Changed += OnStyleChanged;
			FontFamilyProperty.Changed += (o, e) => UnsetCachedFont(o);
			FontSizeProperty.Changed += (o, e) => UnsetCachedFont(o);
			FontBoldProperty.Changed += (o, e) => UnsetCachedFont(o);
			FontItalicProperty.Changed += (o, e) => UnsetCachedFont(o);
			TextOptions.TextRenderingModeProperty.Changed += (o, e) => UnsetCachedFont(o);
		}

		/// <summary>
		///   Adds the given handler to the given routed event.
		/// </summary>
		/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
		/// <param name="routedEvent">The routed event that should be handled.</param>
		/// <param name="handler">The handler that should be invoked when the routed event has been raised.</param>
		public void AddHandler<T>(RoutedEvent<T> routedEvent, RoutedEventHandler<T> handler)
			where T : class, IRoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(handler);

			_eventStore.AddHandler(routedEvent, handler);
		}

		/// <summary>
		///   Removes the given handler from the given routed event.
		/// </summary>
		/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
		/// <param name="routedEvent">The routed event that should be handled.</param>
		/// <param name="handler">The handler that should be invoked when the routed event has been raised.</param>
		public void RemoveHandler<T>(RoutedEvent<T> routedEvent, RoutedEventHandler<T> handler)
			where T : class, IRoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(handler);

			_eventStore.RemoveHandler(routedEvent, handler);
		}

		/// <summary>
		///   Unsets the cached font object.
		/// </summary>
		/// <param name="obj">The object defining the cached font object that should be unset.</param>
		private static void UnsetCachedFont(object obj)
		{
			var uiElement = obj as UIElement;
			if (uiElement != null)
				uiElement._cachedFont = null;
		}

		/// <summary>
		///   Checks whether the given horizontal alignment is a valid value.
		/// </summary>
		/// <param name="alignment">The alignment that should be checked.</param>
		private static bool ValidateAlignment(HorizontalAlignment alignment)
		{
			return alignment == HorizontalAlignment.Stretch ||
				   alignment == HorizontalAlignment.Left ||
				   alignment == HorizontalAlignment.Center ||
				   alignment == HorizontalAlignment.Right;
		}

		/// <summary>
		///   Checks whether the given vertical alignment is a valid value.
		/// </summary>
		/// <param name="alignment">The alignment that should be checked.</param>
		private static bool ValidateAlignment(VerticalAlignment alignment)
		{
			return alignment == VerticalAlignment.Stretch ||
				   alignment == VerticalAlignment.Top ||
				   alignment == VerticalAlignment.Center ||
				   alignment == VerticalAlignment.Bottom;
		}

		/// <summary>
		///   Checks whether the given value is a valid width or height value.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		private static bool ValidateWidthHeight(double value)
		{
			// NaN is used to represent Xaml's 'auto' value
			return Double.IsNaN(value) || (value >= 0.0 && !Double.IsPositiveInfinity(value));
		}

		/// <summary>
		///   Checks whether the given value is a valid minimum width or height value.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		private static bool ValidateMinWidthHeight(double value)
		{
			return !Double.IsNaN(value) && value >= 0.0 && !Double.IsPositiveInfinity(value);
		}

		/// <summary>
		///   Checks whether the given value is a valid maximum width or height value.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		private static bool ValidateMaxWidthHeight(double value)
		{
			return !Double.IsNaN(value) && value >= 0.0;
		}

		/// <summary>
		///   Invoked when a resource within the resource dictionary has been replaced, added, or removed, invalidating all
		///   resources for this UI element and all of its children.
		/// </summary>
		private void ResourceChanged(ResourceDictionary resources, object key)
		{
			InvalidateResources();
		}

		/// <summary>
		///   Raises the resources invalidated event for this UI element and all of its logical children.
		/// </summary>
		private void InvalidateResources()
		{
			if (ResourcesInvalidated != null)
				ResourcesInvalidated();

			// If we're using an implicit style, we have to check whether the style has changed
			if (_usesImplicitStyle)
				BindImplicitStyle();

			foreach (var child in LogicalChildren)
				child.InvalidateResources();
		}

		/// <summary>
		///   Applies a style change to the UI element.
		/// </summary>
		private static void OnStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<Style> property)
		{
			var uiElement = obj as UIElement;
			if (uiElement == null)
				return;

			if (property.OldValue != null)
				property.OldValue.Unset(uiElement);

			// Set the new style; if it is null, try to find an implicit style
			if (property.NewValue == null)
				uiElement.BindImplicitStyle();
			else
			{
				// No need to set the style if the UI element is not part of a logical tree
				property.NewValue.Apply(uiElement);
				uiElement._usesImplicitStyle = false;
			}
		}

		/// <summary>
		///   Binds the implicit style to the UI element, if no other style is set.
		/// </summary>
		private void BindImplicitStyle()
		{
			object style;
			if (!TryFindResource(GetType(), out style) || !(style is Style))
				SetValue(StyleProperty, null);
			else
			{
				Style = (Style)style;
				_usesImplicitStyle = true;
			}
		}

		/// <summary>
		///   Searches the tree for a resource with the given key.
		/// </summary>
		/// <param name="key">The key of the resource that should be returned.</param>
		/// <param name="resource">Returns the resource with the specified key, if it is found.</param>
		internal bool TryFindResource(object key, out object resource)
		{
			Assert.ArgumentNotNull(key);

			// If the key is in our resource dictionary, return the resource
			if (_resources.IsInitialized && _resources.TryGetValue(key, out resource))
				return true;

			// Otherwise, check the logical parent
			if (Parent != null)
				return Parent.TryFindResource(key, out resource);

			// If there is no logical parent, there is no resource with the given key
			resource = null;
			return false;
		}

		/// <summary>
		///   Searches the tree for a resource with the given key.
		/// </summary>
		/// <param name="key">The key of the resource that should be returned.</param>
		/// <param name="resource">Returns the resource with the specified key, if it is found.</param>
		internal bool TryFindResource<T>(object key, out T resource)
		{
			Assert.ArgumentNotNull(key);

			object untypedResource;
			if (!TryFindResource(key, out untypedResource))
			{
				resource = default(T);
				return false;
			}

			resource = (T)untypedResource;
			return true;
		}

		/// <summary>
		///   Changes the logical parent of the UI element.
		/// </summary>
		/// <param name="parent">
		///   The new logical parent of the UI element. If null, the UI element is no longer part of the logical tree.
		/// </param>
		internal void ChangeLogicalParent(UIElement parent)
		{
			if (parent == Parent)
				return;

			Assert.That(parent != this, "Detected a loop in the logical tree.");
			Assert.That(parent == null || Parent == null, "The element is already attached to the logical tree.");

			Parent = parent;

			// Changing the parent possibly invalidates the inherited property values of this UI element and its children
			InvalidateInheritedValues(parent);

			// Setting a new (valid) parent possibly invalidates the resources of this UI element and its children
			if (parent != null)
			{
				InvalidateResources();

				if (Style != null)
					Style.Apply(this);

				OnAttached();
			}
			else
			{
				// Unset the style to avoid memory leaks (style triggers register an event handler on this UI element;
				// therefore, this instance cannot be garbage collected before the triggers are garbage collected, which
				// can have a very long lifetime if declared at application scope)
				if (Style != null)
					Style.Unset(this);

				OnDetached();
			}
		}

		/// <summary>
		///   Notifies all inheriting objects about a change of an inheriting dependency property.
		/// </summary>
		/// <param name="property">The inheriting dependency property that has been changed.</param>
		/// <param name="newValue">The new value that should be inherited.</param>
		protected override sealed void InheritedValueChanged<T>(DependencyProperty<T> property, T newValue)
		{
			foreach (var child in LogicalChildren)
				child.SetInheritedValue(property, newValue);
		}

		/// <summary>
		///   Creates a data binding with the UI element's view model as the source object.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the target dependency property.</typeparam>
		/// <param name="path">The property path that should be evaluated on the UI element's view model to get the source value.</param>
		/// <param name="targetProperty">The dependency property that should be target of the binding.</param>
		public void CreateDataBinding<T>(string path, DependencyProperty<T> targetProperty)
		{
			Assert.ArgumentNotNullOrWhitespace(path);
			Assert.ArgumentNotNull(targetProperty);

			CreateDataBinding(this, "ViewModel." + path, targetProperty);
		}

		/// <summary>
		///   Creates a template binding.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the target dependency property.</typeparam>
		/// <param name="sourceObject">The source object that should provide the value that is bound.</param>
		/// <param name="sourceProperty">The dependency property that should be source of the binding.</param>
		/// <param name="targetProperty">The dependency property that should be target of the binding.</param>
		public void CreateTemplateBinding<T>(Control sourceObject, DependencyProperty<T> sourceProperty, DependencyProperty<T> targetProperty)
		{
			Assert.ArgumentNotNull(sourceObject);
			Assert.ArgumentNotNull(sourceProperty);
			Assert.ArgumentNotNull(targetProperty);

			var binding = new TemplateBinding<T>(sourceObject, sourceProperty);
			binding.Initialize(this, targetProperty);
		}

		/// <summary>
		///   Creates a resource binding.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the target dependency property.</typeparam>
		/// <param name="key">The key of the resource that should be bound to the dependency property.</param>
		/// <param name="targetProperty">The dependency property that should be target of the binding.</param>
		public void CreateResourceBinding<T>(object key, DependencyProperty<T> targetProperty)
		{
			Assert.ArgumentNotNull(key);
			Assert.ArgumentNotNull(targetProperty);

			var binding = new ResourceBinding<T>(key);
			binding.Initialize(this, targetProperty);
		}

		/// <summary>
		///   Updates the UI element's desired size. This method should be called from a parent UI element's MeasureCore method to
		///   perform a the first pass of a recursive layout update.
		/// </summary>
		/// <param name="availableSize">
		///   The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///   to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///   element might be able to use scrolling in this case.
		/// </param>
		public void Measure(SizeD availableSize)
		{
			if (Visibility == Visibility.Collapsed)
				return;

			_layoutInfo = new LayoutInfo(this);
			var hasWidth = !Double.IsNaN(_layoutInfo.Width);
			var hasHeight = !Double.IsNaN(_layoutInfo.Height);

			_desiredSize = MeasureCore(DecreaseByMargin(availableSize));

			Assert.That(!Double.IsInfinity(_desiredSize.Width) && !Double.IsNaN(_desiredSize.Width), "MeasureCore returned invalid width.");
			Assert.That(!Double.IsInfinity(_desiredSize.Height) && !Double.IsNaN(_desiredSize.Height), "MeasureCore returned invalid height.");

			if (hasWidth)
				_desiredSize.Width = _layoutInfo.Width;

			if (hasHeight)
				_desiredSize.Height = _layoutInfo.Height;

			_desiredSize.Width = MathUtils.Clamp(_desiredSize.Width, _layoutInfo.MinWidth, _layoutInfo.MaxWidth);
			_desiredSize.Height = MathUtils.Clamp(_desiredSize.Height, _layoutInfo.MinHeight, _layoutInfo.MaxHeight);

			_desiredSize = IncreaseByMargin(_desiredSize);
		}

		/// <summary>
		///   Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="availableSize">
		///   The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///   to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///   element might be able to use scrolling in this case.
		/// </param>
		protected abstract SizeD MeasureCore(SizeD availableSize);

		/// <summary>
		///   Determines the size and position of the UI element and all of its children. This method should be called from a
		///   parent UI element's ArrangeCore method to perform the second pass of a recursive layout update.
		/// </summary>
		/// <param name="finalRect">The final size and position of the UI element.</param>
		/// <remarks>
		///   The first time a UI element is layouted, Measure is always called before Arrange. Later layout passes
		///   triggered by some changes to the UI element's state might only call Arrange if the UI element's measurement remained
		///   unaffected by the state change.
		/// </remarks>
		public void Arrange(RectangleD finalRect)
		{
			if (Visibility == Visibility.Collapsed)
				return;

			_layoutInfo = new LayoutInfo(this);

			var availableSize = DecreaseByMargin(finalRect.Size);
			var desiredSize = DecreaseByMargin(_desiredSize);

			var width = Math.Min(desiredSize.Width, availableSize.Width);
			var height = Math.Min(desiredSize.Height, availableSize.Height);

			var finalSize = new SizeD(width, height);
			AdaptSize(ref finalSize, availableSize);

			var size = ArrangeCore(finalSize);
			AdaptSize(ref size, availableSize);

			ActualWidth = size.Width;
			ActualHeight = size.Height;

			RenderSize = size;
			VisualOffset = finalRect.Position + ComputeAlignmentOffset(finalRect.Size);
			RenderSize = IncreaseByMargin(size);
		}

		/// <summary>
		///   Adapts the size of the UI element according to the layouting information.
		/// </summary>
		/// <param name="size">The size that should be adapted.</param>
		/// <param name="availableSize">The size that is available to this UI element.</param>
		private void AdaptSize(ref SizeD size, SizeD availableSize)
		{
			// When stretching horizontally, fill all available width
			if (_layoutInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
				size.Width = availableSize.Width;

			// When stretching vertically, fill all available height
			if (_layoutInfo.VerticalAlignment == VerticalAlignment.Stretch)
				size.Height = availableSize.Height;

			// Use the requested width if one is set
			if (!Double.IsNaN(_layoutInfo.Width))
				size.Width = _layoutInfo.Width;

			// Use the requested height if one is set
			if (!Double.IsNaN(_layoutInfo.Height))
				size.Height = _layoutInfo.Height;

			// Clamp the width and the height to the minimum and maximum values
			size.Width = MathUtils.Clamp(size.Width, _layoutInfo.MinWidth, _layoutInfo.MaxWidth);
			size.Height = MathUtils.Clamp(size.Height, _layoutInfo.MinHeight, _layoutInfo.MaxHeight);
		}

		/// <summary>
		///   Determines the size of the UI element and positions all of its children. Returns the actual size used by the UI
		///   element. If this value is smaller than the given size, the UI element's alignment properties position it
		///   appropriately.
		/// </summary>
		/// <param name="finalSize">
		///   The final area allocated by the UI element's parent that the UI element should use to arrange
		///   itself and its children.
		/// </param>
		protected abstract SizeD ArrangeCore(SizeD finalSize);

		/// <summary>
		///   Computes the alignment offset based on the available size and the actual size of the UI element.
		/// </summary>
		/// <param name="availableSize">The available size the UI element should be aligned in.</param>
		private Vector2d ComputeAlignmentOffset(SizeD availableSize)
		{
			var offset = Vector2d.Zero;

			switch (HorizontalAlignment)
			{
				case HorizontalAlignment.Stretch:
					offset.X = _layoutInfo.Margin.Left;
					break;
				case HorizontalAlignment.Center:
					offset.X = (availableSize.Width - RenderSize.Width + _layoutInfo.Margin.Left - _layoutInfo.Margin.Right) / 2;
					break;
				case HorizontalAlignment.Left:
					offset.X = _layoutInfo.Margin.Left;
					break;
				case HorizontalAlignment.Right:
					offset.X = availableSize.Width - RenderSize.Width - _layoutInfo.Margin.Right;
					break;
				default:
					throw new InvalidOperationException("Unexpected alignment.");
			}

			switch (VerticalAlignment)
			{
				case VerticalAlignment.Stretch:
					offset.Y = _layoutInfo.Margin.Top;
					break;
				case VerticalAlignment.Center:
					offset.Y = (availableSize.Height - RenderSize.Height + _layoutInfo.Margin.Top - _layoutInfo.Margin.Bottom) / 2;
					break;
				case VerticalAlignment.Top:
					offset.Y = _layoutInfo.Margin.Top;
					break;
				case VerticalAlignment.Bottom:
					offset.Y = availableSize.Height - RenderSize.Height - _layoutInfo.Margin.Bottom;
					break;
				default:
					throw new InvalidOperationException("Unexpected alignment.");
			}

			offset.X = Math.Max(0, offset.X);
			offset.Y = Math.Max(0, offset.Y);
			return offset;
		}

		/// <summary>
		///   Increases the size to encompass the margin. For instance, if the width is 10 and the left and right margins are 2 and
		///   3, the returned size has a width of 10 + 2 + 3 = 15.
		/// </summary>
		/// <param name="size">The size the thickness should be added to.</param>
		private SizeD IncreaseByMargin(SizeD size)
		{
			return new SizeD(size.Width + _layoutInfo.Margin.Left + _layoutInfo.Margin.Right,
							 size.Height + _layoutInfo.Margin.Top + _layoutInfo.Margin.Bottom);
		}

		/// <summary>
		///   Decreases the size to encompass the margin. For instance, if the width is 10 and the left and right margins are 2 and
		///   3, the returned size has a width of 10 - 2 - 3 = 5.
		/// </summary>
		/// <param name="size">The size the thickness should be added to.</param>
		private SizeD DecreaseByMargin(SizeD size)
		{
			return new SizeD(size.Width - _layoutInfo.Margin.Left - _layoutInfo.Margin.Right,
							 size.Height - _layoutInfo.Margin.Top - _layoutInfo.Margin.Bottom);
		}

		/// <summary>
		///   Invoked when the UI element is attached to a new logical tree.
		/// </summary>
		protected virtual void OnAttached()
		{
		}

		/// <summary>
		///   Invoked when the UI element has been detached from its current logical tree.
		/// </summary>
		protected virtual void OnDetached()
		{
		}
	}
}