namespace Pegasus.Framework.UserInterface
{
	using System;
	using Controls;
	using Converters;
	using Math;
	using Platform.Graphics;
	using Rendering;

	/// <summary>
	///     Provides layouting, input, hit testing, rendering, and other base functionality for all UI elements.
	/// </summary>
	public abstract partial class UIElement : DependencyObject
	{
		/// <summary>
		///     The cached font instance that is currently being used for text rendering.
		/// </summary>
		private Font _cachedFont;

		/// <summary>
		///     The size of the UI element that has been computed by the last measure pass of the layout engine.
		/// </summary>
		private SizeD _desiredSize;

		/// <summary>
		///     Stores the handlers of the UI element's routed events.
		/// </summary>
		private RoutedEventStore _eventStore = new RoutedEventStore();

		/// <summary>
		///     A value indicating whether the UI element is connected to the visual tree's root element.
		/// </summary>
		private bool _isAttachedToRoot;

		/// <summary>
		///     Caches the layouting information of the UI element during the measure and arrange phases for performance reasons.
		/// </summary>
		private LayoutInfo _layoutInfo;

		/// <summary>
		///     The resources used by the UI element.
		/// </summary>
		private ResourceDictionary _resources = new ResourceDictionary();

		/// <summary>
		///     A value indicating whether the UI element uses and implicitly set style.
		/// </summary>
		private bool _usesImplicitStyle = true;

		/// <summary>
		///     Initializes the type.
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
		///     Adds the given handler to the given routed event.
		/// </summary>
		/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
		/// <param name="routedEvent">The routed event that should be handled.</param>
		/// <param name="handler">The handler that should be invoked when the routed event has been raised.</param>
		public void AddHandler<T>(RoutedEvent<T> routedEvent, RoutedEventHandler<T> handler)
			where T : RoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(handler);

			_eventStore.AddHandler(routedEvent, handler);
		}

		/// <summary>
		///     Removes the given handler from the given routed event.
		/// </summary>
		/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
		/// <param name="routedEvent">The routed event that should be handled.</param>
		/// <param name="handler">The handler that should be invoked when the routed event has been raised.</param>
		public void RemoveHandler<T>(RoutedEvent<T> routedEvent, RoutedEventHandler<T> handler)
			where T : RoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(handler);

			_eventStore.RemoveHandler(routedEvent, handler);
		}

		/// <summary>
		///     Unsets the cached font object.
		/// </summary>
		/// <param name="obj">The object defining the cached font object that should be unset.</param>
		private static void UnsetCachedFont(object obj)
		{
			var uiElement = obj as UIElement;
			if (uiElement != null)
				uiElement._cachedFont = null;
		}

		/// <summary>
		///     Checks whether the given horizontal alignment is a valid value.
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
		///     Checks whether the given vertical alignment is a valid value.
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
		///     Checks whether the given value is a valid width or height value.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		private static bool ValidateWidthHeight(double value)
		{
			// NaN is used to represent Xaml's 'auto' value
			return Double.IsNaN(value) || (value >= 0.0 && !Double.IsPositiveInfinity(value));
		}

		/// <summary>
		///     Checks whether the given value is a valid minimum width or height value.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		private static bool ValidateMinWidthHeight(double value)
		{
			return !Double.IsNaN(value) && value >= 0.0 && !Double.IsPositiveInfinity(value);
		}

		/// <summary>
		///     Checks whether the given value is a valid maximum width or height value.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		private static bool ValidateMaxWidthHeight(double value)
		{
			return !Double.IsNaN(value) && value >= 0.0;
		}

		/// <summary>
		///     Invoked when a resource within the resource dictionary has been replaced, added, or removed, invalidating all
		///     resources for this UI element and all of its children.
		/// </summary>
		private void ResourceChanged(ResourceDictionary resources, object key)
		{
			InvalidateResources();
		}

		/// <summary>
		///     Raises the resources invalidated event for this UI element and all of its logical children.
		/// </summary>
		private void InvalidateResources()
		{
			if (!IsAttachedToRoot)
				return;

			if (ResourcesInvalidated != null)
				ResourcesInvalidated();

			// If we're using an implicit style, we have to check whether the style has changed
			if (_usesImplicitStyle)
				BindImplicitStyle();

			foreach (var child in LogicalChildren)
				child.InvalidateResources();

			if (Style != null)
				Style.Apply(this);
		}

		/// <summary>
		///     Applies a style change to the UI element.
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
		///     Binds the implicit style to the UI element, if no other style is set.
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
		///     Searches the tree for a resource with the given key.
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
		///     Searches the tree for a resource with the given key.
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

			Assert.OfType<T>(untypedResource);
			resource = (T)untypedResource;
			return true;
		}

		/// <summary>
		///     Changes the logical parent of the UI element.
		/// </summary>
		/// <param name="parent">
		///     The new logical parent of the UI element. If null, the UI element is no longer part of the logical tree.
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
				IsAttachedToRoot = parent.IsAttachedToRoot;
				InvalidateResources();

				OnAttached();
			}
			else
			{
				// Unset the style to avoid memory leaks (style triggers register an event handler on this UI element;
				// therefore, this instance cannot be garbage collected before the triggers are garbage collected, which
				// can have a very long lifetime if declared at application scope)
				if (Style != null)
					Style.Unset(this);

				IsAttachedToRoot = false;
				OnDetached();
			}
		}

		/// <summary>
		///     Notifies all inheriting objects about a change of an inheriting dependency property.
		/// </summary>
		/// <param name="property">The inheriting dependency property that has been changed.</param>
		/// <param name="newValue">The new value that should be inherited.</param>
		protected override sealed void InheritedValueChanged<T>(DependencyProperty<T> property, T newValue)
		{
			foreach (var child in LogicalChildren)
				child.SetInheritedValue(property, newValue);
		}

		/// <summary>
		///     Sets a binding for the dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be set.</param>
		/// <param name="binding">The binding that should be set.</param>
		private new void SetBinding<T>(DependencyProperty<T> property, Binding<T> binding)
		{
			Assert.ArgumentNotNull(property);
			Assert.ArgumentNotNull(binding);

			base.SetBinding(property, binding);
			binding.Active = IsAttachedToRoot;
		}

		/// <summary>
		/// Creates an event binding.
		/// </summary>
		/// <typeparam name="T">The type of the routed event arguments.</typeparam>
		/// <param name="routedEvent">The routed event that should be bound.</param>
		/// <param name="method">The method that should be invoked when the routed event is raised.</param>
		public void CreateEventBinding<T>(RoutedEvent<T> routedEvent, string method)
			where T : RoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNullOrWhitespace(method);

			var binding = new RoutedEventBinding<T>(this, routedEvent, method);
		}

		/// <summary>
		///     Creates a data binding.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the target dependency property.</typeparam>
		/// <param name="sourceObject">The source object that should provide the value that is bound.</param>
		/// <param name="targetProperty">The dependency property that should be target of the binding.</param>
		/// <param name="bindingMode">Indicates the direction of the data flow of the data binding.</param>
		/// <param name="property1">The name of the first property in the property path.</param>
		/// <param name="property2">The name of the second property in the property path.</param>
		/// <param name="property3">The name of the third property in the property path.</param>
		/// <param name="converter">The convert that should be used to convert the source value to the property type.</param>
		public void CreateDataBinding<T>(object sourceObject, DependencyProperty<T> targetProperty,
										 BindingMode bindingMode,
										 string property1, string property2 = null, string property3 = null,
										 IValueConverter converter = null)
		{
			Assert.ArgumentNotNull(sourceObject);
			Assert.ArgumentNotNull(targetProperty);
			Assert.ArgumentInRange(bindingMode);
			Assert.ArgumentNotNullOrWhitespace(property1);

			SetBinding(targetProperty, new DataBinding<T>(sourceObject, bindingMode, property1, property2, property3, converter));
		}

		/// <summary>
		///     Creates a data binding with the UI element's view model as the source object.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the target dependency property.</typeparam>
		/// <param name="targetProperty">The dependency property that should be target of the binding.</param>
		/// <param name="bindingMode">Indicates the direction of the data flow of the data binding.</param>
		/// <param name="property1">The name of the first property in the property path.</param>
		/// <param name="property2">The name of the second property in the property path.</param>
		/// <param name="converter">The convert that should be used to convert the source value to the property type.</param>
		public void CreateDataBinding<T>(DependencyProperty<T> targetProperty, BindingMode bindingMode,
										 string property1 = null, string property2 = null,
										 IValueConverter converter = null)
		{
			CreateDataBinding(this, targetProperty, bindingMode, "DataContext", property1, property2, converter);
		}

		/// <summary>
		///     Creates a template binding.
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

			SetBinding(targetProperty, new TemplateBinding<T>(sourceObject, sourceProperty));
		}

		/// <summary>
		///     Creates a resource binding.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the target dependency property.</typeparam>
		/// <param name="key">The key of the resource that should be bound to the dependency property.</param>
		/// <param name="targetProperty">The dependency property that should be target of the binding.</param>
		public void CreateResourceBinding<T>(object key, DependencyProperty<T> targetProperty)
		{
			Assert.ArgumentNotNull(key);
			Assert.ArgumentNotNull(targetProperty);

			SetBinding(targetProperty, new ResourceBinding<T>(key));
		}

		/// <summary>
		///     Gets the window the UI element is contained in or null if it isn't contained in any window.
		/// </summary>
		/// <param name="uiElement">The UI element the parent window should be returned for.</param>
		private static Window GetParentWindow(UIElement uiElement)
		{
			while (uiElement != null)
			{
				var window = uiElement as Window;
				if (window != null)
					return window;

				uiElement = uiElement.Parent;
			}

			return null;
		}

		/// <summary>
		///     Sets the keyboard focus to this UI element, provided that it is focusable.
		/// </summary>
		public void Focus()
		{
			if (!Focusable)
				return;

			var window = GetParentWindow(this);
			if (window != null)
				window.Keyboard.FocusedElement = this;
		}

		/// <summary>
		///     First invokes the class handlers of the given routed event, then the instance handlers.
		/// </summary>
		/// <typeparam name="T">The type of the event arguments.</typeparam>
		/// <param name="routedEvent">The routed event that should be raised.</param>
		/// <param name="eventArgs">The arguments the routed event should be raised with.</param>
		private void InvokeEventHandlers<T>(RoutedEvent<T> routedEvent, T eventArgs)
			where T : RoutedEventArgs
		{
			routedEvent.InvokeClassHandlers(this, eventArgs);
			_eventStore.InvokeHandlers(routedEvent, this, eventArgs);
		}

		/// <summary>
		///     Raises the given routed event with the given event arguments.
		/// </summary>
		/// <typeparam name="T">The type of the event arguments.</typeparam>
		/// <param name="routedEvent">The routed event that should be raised.</param>
		/// <param name="eventArgs">The arguments the routed event should be raised with.</param>
		internal void RaiseEvent<T>(RoutedEvent<T> routedEvent, T eventArgs)
			where T : RoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(eventArgs);

			eventArgs.Source = this;
			eventArgs.RoutedEvent = routedEvent;

			switch (routedEvent.RoutingStrategy)
			{
				case RoutingStrategy.Direct:
					InvokeEventHandlers(routedEvent, eventArgs);
					break;
				case RoutingStrategy.Bubble:
					RaiseBubblingEvent(routedEvent, eventArgs);
					break;
				case RoutingStrategy.Tunnel:
					RaiseTunnelingEvent(this, routedEvent, eventArgs);
					break;
				default:
					Assert.NotReached("Unknown routing strategy.");
					break;
			}
		}

		/// <summary>
		///     Raises the given bubbling routed event with the given event arguments.
		/// </summary>
		/// <typeparam name="T">The type of the event arguments.</typeparam>
		/// <param name="routedEvent">The routed event that should be raised.</param>
		/// <param name="eventArgs">The arguments the routed event should be raised with.</param>
		private void RaiseBubblingEvent<T>(RoutedEvent<T> routedEvent, T eventArgs)
			where T : RoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(eventArgs);
			Assert.ArgumentSatisfies(routedEvent.RoutingStrategy == RoutingStrategy.Bubble, "Unexpected routing strategy.");

			var uiElement = this;
			while (uiElement != null && !eventArgs.Handled)
			{
				uiElement.InvokeEventHandlers(routedEvent, eventArgs);
				uiElement = uiElement.Parent;
			}
		}

		/// <summary>
		///     Raises the given tunneling routed event with the given event arguments.
		/// </summary>
		/// <typeparam name="T">The type of the event arguments.</typeparam>
		/// <param name="uiElement">The UI element the event should be raised for.</param>
		/// <param name="routedEvent">The routed event that should be raised.</param>
		/// <param name="eventArgs">The arguments the routed event should be raised with.</param>
		private static void RaiseTunnelingEvent<T>(UIElement uiElement, RoutedEvent<T> routedEvent, T eventArgs)
			where T : RoutedEventArgs
		{
			Assert.ArgumentNotNull(uiElement);
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(eventArgs);
			Assert.ArgumentSatisfies(routedEvent.RoutingStrategy == RoutingStrategy.Tunnel, "Unexpected routing strategy.");

			if (uiElement.Parent != null)
				RaiseTunnelingEvent(uiElement.Parent, routedEvent, eventArgs);

			if (!eventArgs.Handled)
				uiElement.InvokeEventHandlers(routedEvent, eventArgs);
		}

		/// <summary>
		///     Returns the child UI element of the current UI element that lies at the given position.
		/// </summary>
		/// <param name="position">The position that should be checked for a hit.</param>
		internal UIElement HitTest(Vector2d position)
		{
			// If the element isn't visible or hit test visible, there can be no hit.
			if (Visibility != Visibility.Visible || !IsHitTestVisible)
				return null;

			// Check if the position lies within the elements's bounding box. If not, there can be no hit.
			var horizontalHit = position.X >= VisualOffset.X && position.X <= VisualOffset.X + RenderSize.Width;
			var verticalHit = position.Y >= VisualOffset.Y && position.Y <= VisualOffset.Y + RenderSize.Height;

			if (!horizontalHit || !verticalHit)
				return null;

			// Find and return the first visual child that is hit.
			// We have to iterate the visual children in reverse order, as they are enumerated from
			// bottom to top (optimized for drawing), whereas we want to check the topmost children first.
			var count = VisualChildrenCount;
			for (var i = count; i > 0; --i)
			{
				var child = GetVisualChild(i - 1);
				var hitTestResult = child.HitTest(position);

				if (hitTestResult != null)
					return hitTestResult;
			}

			// If no child was hit, check if we should return this UI element instead.
			return HitTestCore(position) ? this : null;
		}

		/// <summary>
		///     Performs a detailed hit test for the given position. The position is guaranteed to lie within the UI element's
		///     bounds. This method should be overridden to implement special hit testing logic that is more precise than a
		///     simple bounding box check.
		/// </summary>
		/// <param name="position">The position that should be checked for a hit.</param>
		/// <returns>Returns true if the UI element is hit; false, otherwise.</returns>
		protected virtual bool HitTestCore(Vector2d position)
		{
			return Background.HasValue;
		}

		/// <summary>
		///     Updates the UI element's desired size. This method should be called from a parent UI element's MeasureCore method to
		///     perform a the first pass of a recursive layout update.
		/// </summary>
		/// <param name="availableSize">
		///     The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///     to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///     element might be able to use scrolling in this case.
		/// </param>
		public void Measure(SizeD availableSize)
		{
			if (Visibility == Visibility.Collapsed)
				return;

			_layoutInfo = new LayoutInfo(this);
			availableSize = RestrictSize(availableSize);

			_desiredSize = MeasureCore(DecreaseByMargin(availableSize));

			Assert.That(!Double.IsInfinity(_desiredSize.Width) && !Double.IsNaN(_desiredSize.Width), "MeasureCore returned invalid width.");
			Assert.That(!Double.IsInfinity(_desiredSize.Height) && !Double.IsNaN(_desiredSize.Height), "MeasureCore returned invalid height.");

			_desiredSize = RestrictSize(_desiredSize);
			_desiredSize = IncreaseByMargin(_desiredSize);
		}

		/// <summary>
		///     Restricts the given size, taking the explicit size as well as the minimum and maximum size of the UI element into
		///     account.
		/// </summary>
		/// <param name="size">The size that should be restricted.</param>
		private SizeD RestrictSize(SizeD size)
		{
			if (_layoutInfo.HasExplicitWidth)
				size.Width = _layoutInfo.Width;

			if (_layoutInfo.HasExplicitHeight)
				size.Height = _layoutInfo.Height;

			size.Width = MathUtils.Clamp(size.Width, _layoutInfo.MinWidth, _layoutInfo.MaxWidth);
			size.Height = MathUtils.Clamp(size.Height, _layoutInfo.MinHeight, _layoutInfo.MaxHeight);

			return size;
		}

		/// <summary>
		///     Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="availableSize">
		///     The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///     to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///     element might be able to use scrolling in this case.
		/// </param>
		protected abstract SizeD MeasureCore(SizeD availableSize);

		/// <summary>
		///     Determines the size and position of the UI element and all of its children. This method should be called from a
		///     parent UI element's ArrangeCore method to perform the second pass of a recursive layout update.
		/// </summary>
		/// <param name="finalRect">The final size and position of the UI element.</param>
		/// <remarks>
		///     The first time a UI element is layouted, Measure is always called before Arrange. Later layout passes
		///     triggered by some changes to the UI element's state might only call Arrange if the UI element's measurement remained
		///     unaffected by the state change.
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

			var oldSize = RenderSize;
			RenderSize = size;

			VisualOffset = finalRect.Position + ComputeAlignmentOffset(finalRect.Size);
			RenderSize = IncreaseByMargin(size);

			if (oldSize != RenderSize)
				OnSizeChanged(oldSize, RenderSize);
		}

		/// <summary>
		///     Adapts the size of the UI element according to the layouting information.
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
		///     Determines the size of the UI element and positions all of its children. Returns the actual size used by the UI
		///     element. If this value is smaller than the given size, the UI element's alignment properties position it
		///     appropriately.
		/// </summary>
		/// <param name="finalSize">
		///     The final area allocated by the UI element's parent that the UI element should use to arrange
		///     itself and its children.
		/// </param>
		protected abstract SizeD ArrangeCore(SizeD finalSize);

		/// <summary>
		///     Computes the alignment offset based on the available size and the actual size of the UI element.
		/// </summary>
		/// <param name="availableSize">The available size the UI element should be aligned in.</param>
		private Vector2d ComputeAlignmentOffset(SizeD availableSize)
		{
			var offset = Vector2d.Zero;

			switch (HorizontalAlignment)
			{
				case HorizontalAlignment.Stretch:
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
		///     Increases the size to encompass the margin. For instance, if the width is 10 and the left and right margins are 2 and
		///     3, the returned size has a width of 10 + 2 + 3 = 15.
		/// </summary>
		/// <param name="size">The size the thickness should be added to.</param>
		private SizeD IncreaseByMargin(SizeD size)
		{
			return new SizeD(size.Width + _layoutInfo.Margin.Left + _layoutInfo.Margin.Right,
				size.Height + _layoutInfo.Margin.Top + _layoutInfo.Margin.Bottom);
		}

		/// <summary>
		///     Decreases the size to encompass the margin. For instance, if the width is 10 and the left and right margins are 2 and
		///     3, the returned size has a width of 10 - 2 - 3 = 5.
		/// </summary>
		/// <param name="size">The size the thickness should be added to.</param>
		private SizeD DecreaseByMargin(SizeD size)
		{
			return new SizeD(size.Width - _layoutInfo.Margin.Left - _layoutInfo.Margin.Right,
				size.Height - _layoutInfo.Margin.Top - _layoutInfo.Margin.Bottom);
		}

		/// <summary>
		///     Invoked when the UI element is attached to a new logical parent.
		/// </summary>
		protected virtual void OnAttached()
		{
		}

		/// <summary>
		///     Invoked when the UI element is now (transitively) attached to the root of a visual tree.
		/// </summary>
		protected virtual void OnAttachedToRoot()
		{
		}

		/// <summary>
		///     Invoked when the UI element has been detached from its current logical parent.
		/// </summary>
		protected virtual void OnDetached()
		{
		}

		/// <summary>
		///     Invoked when the UI element is no longer (transitively) attached to the root of a visual tree.
		/// </summary>
		protected virtual void OnDetachedFromRoot()
		{
		}

		/// <summary>
		///     Invoked when the size of the UI element has changed.
		/// </summary>
		/// <param name="oldSize">The old size of the UI element.</param>
		/// <param name="newSize">The new size of the UI element.</param>
		protected virtual void OnSizeChanged(SizeD oldSize, SizeD newSize)
		{
		}

		/// <summary>
		///     Invoked when the visual children of the UI element have changed.
		/// </summary>
		protected internal virtual void OnVisualChildrenChanged()
		{
		}

		/// <summary>
		///     Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected internal virtual UIElement GetVisualChild(int index)
		{
			Assert.That(false, "This visual does not have any visual children.");
			return null;
		}

		internal void Draw(SpriteBatch spriteBatch)
		{
			if (Visibility != Visibility.Visible)
				return;

			OnDraw(spriteBatch);

			var count = VisualChildrenCount;
			for (var i = 0; i < count; ++i)
			{
				++spriteBatch.Layer;

				var child = GetVisualChild(i);
				child.VisualOffset += VisualOffset;
				child.Draw(spriteBatch);
			}
		}

		protected abstract void OnDraw(SpriteBatch spriteBatch);
	}
}