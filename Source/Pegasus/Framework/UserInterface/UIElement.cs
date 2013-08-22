using System;

namespace Pegasus.Framework.UserInterface
{
	using Math;
	using Platform.Graphics;
	using Platform.Logging;
	using Rendering.UserInterface;
	using Math = System.Math;

	/// <summary>
	///   Provides layouting, input, and other base functionality for all UI elements.
	/// </summary>
	public abstract class UIElement : Visual
	{
		/// <summary>
		///   The view model of the UI element.
		/// </summary>
		public static readonly DependencyProperty<ViewModel> ViewModelProperty =
			new DependencyProperty<ViewModel>(inherits: true, prohibitsAnimations: true);

		/// <summary>
		///   The foreground color of the UI element.
		/// </summary>
		public static readonly DependencyProperty<Color> ForegroundProperty =
			new DependencyProperty<Color>(defaultValue: new Color(255, 255, 255, 255), affectsRender: true);

		/// <summary>
		///   The style of the UI element.
		/// </summary>
		public static readonly DependencyProperty<Style> StyleProperty =
			new DependencyProperty<Style>(affectsMeasure: true);

		/// <summary>
		///   The font used for text rendering by the UI element.
		/// </summary>
		public static readonly DependencyProperty<Font> FontProperty =
			new DependencyProperty<Font>(affectsMeasure: true, inherits: true);

		/// <summary>
		///   Indicates whether the mouse is currently hovering the UI element.
		/// </summary>
		public static readonly DependencyProperty<bool> IsMouseOverProperty =
			new DependencyProperty<bool>();

		/// <summary>
		///   The width of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> WidthProperty =
			new DependencyProperty<double>(defaultValue: Double.NaN, affectsMeasure: true, validationCallback: ValidateWidthHeight);

		/// <summary>
		///   The height of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> HeightProperty =
			new DependencyProperty<double>(defaultValue: Double.NaN, affectsMeasure: true, validationCallback: ValidateWidthHeight);

		/// <summary>
		///   The minimum width constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MinWidthProperty =
			new DependencyProperty<double>(defaultValue: 0.0, affectsMeasure: true, validationCallback: ValidateMinWidthHeight);

		/// <summary>
		///   The minimum height constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MinHeightProperty =
			new DependencyProperty<double>(defaultValue: 0.0, affectsMeasure: true, validationCallback: ValidateMinWidthHeight);

		/// <summary>
		///   The maximum width constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MaxWidthProperty =
			new DependencyProperty<double>(defaultValue: Double.PositiveInfinity, affectsMeasure: true, validationCallback: ValidateMaxWidthHeight);

		/// <summary>
		///   The maximum height constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MaxHeightProperty =
			new DependencyProperty<double>(defaultValue: Double.PositiveInfinity, affectsMeasure: true, validationCallback: ValidateMaxWidthHeight);

		/// <summary>
		///   The actual width of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public static readonly DependencyProperty<double> ActualWidthProperty =
			new DependencyProperty<double>();

		/// <summary>
		///   The actual height of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public static readonly DependencyProperty<double> ActualHeightProperty =
			new DependencyProperty<double>();

		/// <summary>
		///   Indicates whether the UI element is visible.
		/// </summary>
		public static readonly DependencyProperty<bool> VisibleProperty =
			new DependencyProperty<bool>(defaultValue: true, affectsMeasure: true);

		/// <summary>
		///   The outer margin of the UI element.
		/// </summary>
		public static readonly DependencyProperty<Thickness> MarginProperty =
			new DependencyProperty<Thickness>(affectsMeasure: true);

		/// <summary>
		///   The horizontal alignment characteristics of the UI element.
		/// </summary>
		public static readonly DependencyProperty<HorizontalAlignment> HorizontalAlignmentProperty =
			new DependencyProperty<HorizontalAlignment>(defaultValue: HorizontalAlignment.Stretch,
														affectsArrange: true,
														validationCallback: ValidateAlignment);

		/// <summary>
		///   The vertical alignment characteristics of the UI element.
		/// </summary>
		public static readonly DependencyProperty<VerticalAlignment> VerticalAlignmentProperty =
			new DependencyProperty<VerticalAlignment>(defaultValue: VerticalAlignment.Stretch,
													  affectsArrange: true,
													  validationCallback: ValidateAlignment);

		/// <summary>
		///   The size of the UI element that has been computed by the last measure pass of the layout engine.
		/// </summary>
		private SizeD _desiredSize;

		/// <summary>
		///   The resources used by the UI element.
		/// </summary>
		private ResourceDictionary _resources;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected UIElement()
		{
			AddChangedHandler(StyleProperty, OnStyleChanged);
		}

		/// <summary>
		///   Gets or sets the view model of the UI element.
		/// </summary>
		public ViewModel ViewModel
		{
			get { return GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		/// <summary>
		///   Gets or sets the foreground color of the UI element.
		/// </summary>
		public Color Foreground
		{
			get { return GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}

		/// <summary>
		///   Gets or sets the style of the UI element.
		/// </summary>
		public Style Style
		{
			get { return GetValue(StyleProperty); }
			set { SetValue(StyleProperty, value); }
		}

		/// <summary>
		///   Gets or sets the font used for text rendering by the UI element.
		/// </summary>
		public Font Font
		{
			get { return GetValue(FontProperty); }
			set { SetValue(FontProperty, value); }
		}

		/// <summary>
		///   Gets or sets the resources used by the UI element.
		/// </summary>
		public ResourceDictionary Resources
		{
			get
			{
				if (_resources == null)
				{
					_resources = new ResourceDictionary();
					_resources.ResourceChanged += ResourceChanged;
				}

				return _resources;
			}
			set
			{
				if (_resources != null)
					_resources.ResourceChanged -= ResourceChanged;

				_resources = value ?? new ResourceDictionary();
				_resources.ResourceChanged += ResourceChanged;

				InvalidateResources();
			}
		}

		/// <summary>
		///   Gets or sets the width of the UI element, measured in pixels.
		/// </summary>
		public double Width
		{
			get { return GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}

		/// <summary>
		///   Gets or sets the height of the UI element, measured in pixels.
		/// </summary>
		public double Height
		{
			get { return GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}

		/// <summary>
		///   Gets or sets the minimum width constraint of the UI element, measured in pixels.
		/// </summary>
		public double MinWidth
		{
			get { return GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}

		/// <summary>
		///   Gets or sets the minimum height constraint of the UI element, measured in pixels.
		/// </summary>
		public double MinHeight
		{
			get { return GetValue(MinHeightProperty); }
			set { SetValue(MinHeightProperty, value); }
		}

		/// <summary>
		///   Gets or sets the maximum width constraint of the UI element, measured in pixels.
		/// </summary>
		public double MaxWidth
		{
			get { return GetValue(MaxWidthProperty); }
			set { SetValue(MaxWidthProperty, value); }
		}

		/// <summary>
		///   Gets or sets the maximum height constraint of the UI element, measured in pixels.
		/// </summary>
		public double MaxHeight
		{
			get { return GetValue(MaxHeightProperty); }
			set { SetValue(MaxHeightProperty, value); }
		}

		/// <summary>
		///   Gets  the actual width of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public double ActualWidth
		{
			get { return GetValue(ActualWidthProperty); }
		}

		/// <summary>
		///   Gets the actual height of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public double ActualHeight
		{
			get { return GetValue(ActualHeightProperty); }
		}

		/// <summary>
		///   Indicates whether the UI element is visible.
		/// </summary>
		public bool Visible
		{
			get { return GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}

		/// <summary>
		///   Gets or sets the outer margin of the UI element.
		/// </summary>
		public Thickness Margin
		{
			get { return GetValue(MarginProperty); }
			set { SetValue(MarginProperty, value); }
		}

		/// <summary>
		///   The horizontal alignment characteristics of the UI element.
		/// </summary>
		public HorizontalAlignment HorizontalAlignment
		{
			get { return GetValue(HorizontalAlignmentProperty); }
			set { SetValue(HorizontalAlignmentProperty, value); }
		}

		/// ///
		/// <summary>
		///   The vertical alignment characteristics of the UI element.
		/// </summary>
		public VerticalAlignment VerticalAlignment
		{
			get { return GetValue(VerticalAlignmentProperty); }
			set { SetValue(VerticalAlignmentProperty, value); }
		}

		/// <summary>
		///   Gets the size of the UI element that has been computed by the last measure pass of the layout engine.
		/// </summary>
		public SizeD DesiredSize
		{
			get
			{
				if (!Visible)
					return new SizeD(0, 0);

				return _desiredSize;
			}
		}

		/// <summary>
		///   Indicates whether the mouse is currently hovering the UI element.
		/// </summary>
		public bool IsMouseOver
		{
			get { return GetValue(IsMouseOverProperty); }
			set { SetValue(IsMouseOverProperty, value); }
		}

		/// <summary>
		///   Gets the logical parent of the UI element.
		/// </summary>
		public UIElement Parent { get; internal set; }

		/// <summary>
		///   Gets an enumerator that can be used to enumerate all logical children of the UI element.
		/// </summary>
		protected internal abstract UIElementCollection.Enumerator LogicalChildren { get; }

		/// <summary>
		///   Gets the final render size of the UI element.
		/// </summary>
		public SizeD RenderSize { get; internal set; }

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
			// NaN is used to represent XAML's 'auto' value
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
		///   Raised when a change to a resource dictionary in this UI element or one of its ancestors has occurred.
		/// </summary>
		internal event Action ResourcesInvalidated;

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

			BindImplicitStyle();
			foreach (var child in LogicalChildren)
				child.InvalidateResources();
		}

		/// <summary>
		///   Applies a style change to the UI element.
		/// </summary>
		private void OnStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<Style> property)
		{
			if (property.OldValue != null)
				property.OldValue.Unset(this);

			if (property.NewValue != null)
			{
				property.NewValue.Seal();
				property.NewValue.Apply(this);
			}
			else
				BindImplicitStyle();
		}

		/// <summary>
		///   Binds the implicit style to the UI element, if no other style is set.
		/// </summary>
		private void BindImplicitStyle()
		{
			object style;
			if (!TryFindResource(GetType(), out style) || !(style is Style))
			{
				Log.Warn("No style could be determined for an UI element of type '{0}'.", GetType().FullName);
				SetValue(StyleProperty, null);
			}
			else
				Style = (Style)style;
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
			if (_resources != null && _resources.TryGetValue(key, out resource))
				return true;

			// Otherwise, check the logical parent
			if (Parent != null)
				return Parent.TryFindResource(key, out resource);

			// If there is no logical parent, there is no resource with the given key
			resource = null;
			return false;
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
				OnAttached();
			}
			else
				OnDetached();
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
			_desiredSize = MeasureCore(availableSize);

			Assert.That(!Double.IsInfinity(_desiredSize.Width) && !Double.IsNaN(_desiredSize.Width), "MeasureCore returned invalid width.");
			Assert.That(!Double.IsInfinity(_desiredSize.Height) && !Double.IsNaN(_desiredSize.Height), "MeasureCore returned invalid height.");

			_desiredSize = IncreaseByMargin(_desiredSize);
		}

		/// <summary>
		///   Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="constraint">
		///   The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///   to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///   element might be able to use scrolling in this case.
		/// </param>
		protected abstract SizeD MeasureCore(SizeD constraint);

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
			var renderSize = ArrangeCore(finalRect.Size);

			if (HorizontalAlignment == HorizontalAlignment.Stretch)
				renderSize.Width = finalRect.Width - Margin.Left - Margin.Right;

			if (VerticalAlignment == VerticalAlignment.Stretch)
				renderSize.Height = finalRect.Height - Margin.Top - Margin.Bottom;

			var offset = ComputeAlignmentOffset(finalRect.Size);

			VisualOffset = finalRect.Position + offset;
			RenderSize = renderSize;
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
				case HorizontalAlignment.Center:
					offset.X = (availableSize.Width - RenderSize.Width) / 2 + Margin.Left - Margin.Right;
					break;
				case HorizontalAlignment.Stretch:
				case HorizontalAlignment.Left:
					offset.X = Margin.Left;
					break;
				case HorizontalAlignment.Right:
					offset.X = availableSize.Width - RenderSize.Width - Margin.Right;
					break;
				default:
					throw new InvalidOperationException("Unexepcted alignment.");
			}

			switch (VerticalAlignment)
			{
				case VerticalAlignment.Center:
					offset.Y = (availableSize.Height - RenderSize.Height) / 2 + Margin.Top - Margin.Bottom;
					break;
				case VerticalAlignment.Stretch:
				case VerticalAlignment.Top:
					offset.Y = Margin.Top;
					break;
				case VerticalAlignment.Bottom:
					offset.Y = availableSize.Height - RenderSize.Height - Margin.Bottom;
					break;
				default:
					throw new InvalidOperationException("Unexepcted alignment.");
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
			var margin = Margin;
			return new SizeD(size.Width + margin.Left + margin.Right, size.Height + margin.Top + margin.Bottom);
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