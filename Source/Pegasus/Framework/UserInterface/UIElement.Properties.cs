namespace Pegasus.Framework.UserInterface
{
	using System;
	using Controls;
	using Input;
	using Math;
	using Platform.Graphics;
	using Platform.Logging;

	public partial class UIElement
	{
		/// <summary>
		///     The data context of the UI element.
		/// </summary>
		public static readonly DependencyProperty<object> DataContextProperty =
			new DependencyProperty<object>(inherits: true, prohibitsAnimations: true);

		/// <summary>
		///     The background color of the UI element.
		/// </summary>
		public static readonly DependencyProperty<Color?> BackgroundProperty =
			new DependencyProperty<Color?>(defaultValue: null, affectsRender: true);

		/// <summary>
		///     The style of the UI element.
		/// </summary>
		public static readonly DependencyProperty<Style> StyleProperty =
			new DependencyProperty<Style>(affectsMeasure: true);

		/// <summary>
		///     The font family used for text rendering by the UI element.
		/// </summary>
		public static readonly DependencyProperty<string> FontFamilyProperty =
			new DependencyProperty<string>(affectsMeasure: true, inherits: true);

		/// <summary>
		///     The font size used for text rendering by the UI element.
		/// </summary>
		public static readonly DependencyProperty<int> FontSizeProperty =
			new DependencyProperty<int>(affectsMeasure: true, inherits: true);

		/// <summary>
		///     Indicates whether a bold font is used for text rendering by the UI element.
		/// </summary>
		public static readonly DependencyProperty<bool> FontBoldProperty =
			new DependencyProperty<bool>(defaultValue: false, affectsMeasure: true, inherits: true);

		/// <summary>
		///     Indicates whether an italic font is used for text rendering by the UI element.
		/// </summary>
		public static readonly DependencyProperty<bool> FontItalicProperty =
			new DependencyProperty<bool>(defaultValue: false, affectsMeasure: true, inherits: true);

		/// <summary>
		///     Indicates whether the mouse is currently hovering the UI element.
		/// </summary>
		public static readonly DependencyProperty<bool> IsMouseOverProperty = new DependencyProperty<bool>(isReadOnly: true);

		/// <summary>
		///     The width of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> WidthProperty =
			new DependencyProperty<double>(defaultValue: Double.NaN, affectsMeasure: true, validationCallback: ValidateWidthHeight);

		/// <summary>
		///     The height of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> HeightProperty =
			new DependencyProperty<double>(defaultValue: Double.NaN, affectsMeasure: true, validationCallback: ValidateWidthHeight);

		/// <summary>
		///     The minimum width constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MinWidthProperty =
			new DependencyProperty<double>(defaultValue: 0.0, affectsMeasure: true, validationCallback: ValidateMinWidthHeight);

		/// <summary>
		///     The minimum height constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MinHeightProperty =
			new DependencyProperty<double>(defaultValue: 0.0, affectsMeasure: true, validationCallback: ValidateMinWidthHeight);

		/// <summary>
		///     The maximum width constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MaxWidthProperty =
			new DependencyProperty<double>(defaultValue: Double.PositiveInfinity, affectsMeasure: true, validationCallback: ValidateMaxWidthHeight);

		/// <summary>
		///     The maximum height constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MaxHeightProperty =
			new DependencyProperty<double>(defaultValue: Double.PositiveInfinity, affectsMeasure: true, validationCallback: ValidateMaxWidthHeight);

		/// <summary>
		///     The actual width of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public static readonly DependencyProperty<double> ActualWidthProperty = new DependencyProperty<double>();

		/// <summary>
		///     The actual height of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public static readonly DependencyProperty<double> ActualHeightProperty = new DependencyProperty<double>();

		/// <summary>
		///     The outer margin of the UI element.
		/// </summary>
		public static readonly DependencyProperty<Thickness> MarginProperty =
			new DependencyProperty<Thickness>(affectsMeasure: true);

		/// <summary>
		///     The horizontal alignment characteristics of the UI element.
		/// </summary>
		public static readonly DependencyProperty<HorizontalAlignment> HorizontalAlignmentProperty =
			new DependencyProperty<HorizontalAlignment>(defaultValue: HorizontalAlignment.Stretch,
				affectsArrange: true, validationCallback: ValidateAlignment);

		/// <summary>
		///     The vertical alignment characteristics of the UI element.
		/// </summary>
		public static readonly DependencyProperty<VerticalAlignment> VerticalAlignmentProperty =
			new DependencyProperty<VerticalAlignment>(defaultValue: VerticalAlignment.Stretch,
				affectsArrange: true, validationCallback: ValidateAlignment);

		/// <summary>
		///     Indicates whether the UI element is visible.
		/// </summary>
		public static readonly DependencyProperty<Visibility> VisibilityProperty =
			new DependencyProperty<Visibility>(defaultValue: Visibility.Visible, affectsMeasure: true);

		/// <summary>
		///     Indicates whether the UI element can receive the keyboard focus.
		/// </summary>
		public static readonly DependencyProperty<bool> FocusableProperty = new DependencyProperty<bool>();

		/// <summary>
		///     Indicates whether the UI element currently has the keyboard focus.
		/// </summary>
		public static readonly DependencyProperty<bool> IsFocusedProperty = new DependencyProperty<bool>(isReadOnly: true);

		/// <summary>
		///     Indicates whether the UI element participates in hit testing.
		/// </summary>
		public static readonly DependencyProperty<bool> IsHitTestVisibleProperty = new DependencyProperty<bool>(defaultValue: true);

		/// <summary>
		///     Gets or sets the data context of the UI element.
		/// </summary>
		public object DataContext
		{
			get { return GetValue(DataContextProperty); }
			set { SetValue(DataContextProperty, value); }
		}

		/// <summary>
		///     Gets or sets the background color of the control.
		/// </summary>
		public Color? Background
		{
			get { return GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}

		/// <summary>
		///     Gets or sets the style of the UI element.
		/// </summary>
		public Style Style
		{
			get { return GetValue(StyleProperty); }
			set { SetValue(StyleProperty, value); }
		}

		/// <summary>
		///     Gets or sets the font family used for text rendering by the UI element.
		/// </summary>
		public string FontFamily
		{
			get { return GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}

		/// <summary>
		///     Gets or sets the font size used for text rendering by the UI element.
		/// </summary>
		public int FontSize
		{
			get { return GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		/// <summary>
		///     Gets or sets a value indicating whether a bold font is used for text rendering by the UI element.
		/// </summary>
		public bool FontBold
		{
			get { return GetValue(FontBoldProperty); }
			set { SetValue(FontBoldProperty, value); }
		}

		/// <summary>
		///     Gets or sets a value indicating whether an italic font is used for text rendering by the UI element.
		/// </summary>
		public bool FontItalic
		{
			get { return GetValue(FontItalicProperty); }
			set { SetValue(FontItalicProperty, value); }
		}

		/// <summary>
		///     Gets the resources used by the UI element.
		/// </summary>
		public ResourceDictionary Resources
		{
			get
			{
				if (!_resources.IsInitialized)
				{
					_resources.Initialize();
					_resources.ResourceChanged += ResourceChanged;
				}

				return _resources;
			}
		}

		/// <summary>
		///     Gets or sets the width of the UI element, measured in pixels.
		/// </summary>
		public double Width
		{
			get { return GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}

		/// <summary>
		///     Gets or sets the height of the UI element, measured in pixels.
		/// </summary>
		public double Height
		{
			get { return GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}

		/// <summary>
		///     Gets or sets the minimum width constraint of the UI element, measured in pixels.
		/// </summary>
		public double MinWidth
		{
			get { return GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}

		/// <summary>
		///     Gets or sets the minimum height constraint of the UI element, measured in pixels.
		/// </summary>
		public double MinHeight
		{
			get { return GetValue(MinHeightProperty); }
			set { SetValue(MinHeightProperty, value); }
		}

		/// <summary>
		///     Gets or sets the maximum width constraint of the UI element, measured in pixels.
		/// </summary>
		public double MaxWidth
		{
			get { return GetValue(MaxWidthProperty); }
			set { SetValue(MaxWidthProperty, value); }
		}

		/// <summary>
		///     Gets or sets the maximum height constraint of the UI element, measured in pixels.
		/// </summary>
		public double MaxHeight
		{
			get { return GetValue(MaxHeightProperty); }
			set { SetValue(MaxHeightProperty, value); }
		}

		/// <summary>
		///     Gets  the actual width of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public double ActualWidth
		{
			get { return GetValue(ActualWidthProperty); }
			private set { SetValue(ActualWidthProperty, value); }
		}

		/// <summary>
		///     Gets the actual height of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public double ActualHeight
		{
			get { return GetValue(ActualHeightProperty); }
			private set { SetValue(ActualHeightProperty, value); }
		}

		/// <summary>
		///     Gets or sets the outer margin of the UI element.
		/// </summary>
		public Thickness Margin
		{
			get { return GetValue(MarginProperty); }
			set { SetValue(MarginProperty, value); }
		}

		/// <summary>
		///     The horizontal alignment characteristics of the UI element.
		/// </summary>
		public HorizontalAlignment HorizontalAlignment
		{
			get { return GetValue(HorizontalAlignmentProperty); }
			set { SetValue(HorizontalAlignmentProperty, value); }
		}

		/// <summary>
		///     The vertical alignment characteristics of the UI element.
		/// </summary>
		public VerticalAlignment VerticalAlignment
		{
			get { return GetValue(VerticalAlignmentProperty); }
			set { SetValue(VerticalAlignmentProperty, value); }
		}

		/// <summary>
		///     Gets or sets a value indicating whether the UI element is visible.
		/// </summary>
		public Visibility Visibility
		{
			get { return GetValue(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}

		/// <summary>
		///     Gets or sets a value indicating whether the UI element can receive the keyboard focus.
		/// </summary>
		public bool Focusable
		{
			get { return GetValue(FocusableProperty); }
			set { SetValue(FocusableProperty, value); }
		}

		/// <summary>
		///     Gets a value indicating whether the UI element currently has the keyboard focus.
		/// </summary>
		public bool IsFocused
		{
			get { return GetValue(IsFocusedProperty); }
			internal set { SetReadOnlyValue(IsFocusedProperty, value); }
		}

		/// <summary>
		///     Gets or sets a value indicating whether the UI element participates in hit testing.
		/// </summary>
		public bool IsHitTestVisible
		{
			get { return GetValue(IsHitTestVisibleProperty); }
			set { SetValue(IsHitTestVisibleProperty, value); }
		}

		/// <summary>
		///     Gets the size of the UI element that has been computed by the last measure pass of the layout engine.
		/// </summary>
		public SizeD DesiredSize
		{
			get { return _desiredSize; }
		}

		/// <summary>
		///     Gets value indicating whether the mouse is currently hovering the UI element or any of its visual children.
		/// </summary>
		public bool IsMouseOver
		{
			get { return GetValue(IsMouseOverProperty); }
			internal set { SetReadOnlyValue(IsMouseOverProperty, value); }
		}

		/// <summary>
		///     Gets the font used for text rendering.
		/// </summary>
		protected internal Font Font
		{
			get
			{
				if (_cachedFont == null)
				{
					IFontLoader fontLoader;
					if (!TryFindResource(typeof(IFontLoader), out fontLoader))
						Log.Die("Unable to find a font loader implementing '{0}' in the UI element's resources.", typeof(IFontLoader).FullName);

					var aliased = TextOptions.GetTextRenderingMode(this) == TextRenderingMode.Aliased;
					_cachedFont = fontLoader.LoadFont(FontFamily, FontSize, FontBold, FontItalic, aliased);
				}

				return _cachedFont;
			}
		}

		/// <summary>
		///     Gets the logical parent of the UI element.
		/// </summary>
		public UIElement LogicalParent { get; internal set; }

		/// <summary>
		///     Gets the visual parent of the UI element.
		/// </summary>
		public UIElement VisualParent
		{
			get { return _visualParent; }
			internal set
			{
				if (_visualParent == value)
					return;

				// The old parent's layout must be invalidated
				if (_visualParent != null)
					_visualParent.SetDirtyState(true, true);

				_visualParent = value;

				// The new parent's layout must be invalidated
				if (_visualParent != null)
					_visualParent.SetDirtyState(true, true);
			}
		}

		/// <summary>
		///     Gets an enumerator that can be used to enumerate all logical children of the UI element.
		/// </summary>
		protected internal abstract Enumerator<UIElement> LogicalChildren { get; }

		/// <summary>
		///     Gets the final render size of the UI element.
		/// </summary>
		public SizeD RenderSize { get; internal set; }

		/// <summary>
		///     Gets or sets the relative offset value of the UI element for drawing.
		/// </summary>
		protected internal Vector2d VisualOffset { get; set; }

		/// <summary>
		///     Gets the number of visual children for this UI element.
		/// </summary>
		protected internal virtual int VisualChildrenCount
		{
			get { return 0; }
		}

		/// <summary>
		///     Gets or sets a value indicating whether the UI element is attached to the visual tree's root element.
		/// </summary>
		protected internal bool IsAttachedToRoot
		{
			get { return (_state & State.IsAttachedToRoot) == State.IsAttachedToRoot; }
			set
			{
				if (IsAttachedToRoot == value)
					return;

				if (value)
					_state |= State.IsAttachedToRoot;
				else
					_state &= ~State.IsAttachedToRoot;

				SetBindingsActivationState(value);
				_eventStore.SetBindingsActivationState(value);

				if (_inputBindings != null)
					_inputBindings.Active = value;

				var childrenCount = VisualChildrenCount;
				for (var i = 0; i < childrenCount; ++i)
					GetVisualChild(i).IsAttachedToRoot = value;

				if (value)
					OnAttachedToRoot();
				else
					OnDetachedFromRoot();
			}
		}

		/// <summary>
		///     Gets a value indicating whether the UI element can receive the keyboard focus.
		/// </summary>
		internal bool CanBeFocused
		{
			get
			{
				if (!IsAttachedToRoot || !Focusable)
					return false;

				var element = this;
				while (element != null)
				{
					if (element.Visibility != Visibility.Visible || !element.IsHitTestVisible)
						return false;

					element = element.LogicalParent;
				}

				return true;
			}
		}

		/// <summary>
		///     Gets the list of input bindings associated with this UI element.
		/// </summary>
		public InputBindingCollection InputBindings
		{
			get { return _inputBindings ?? (_inputBindings = new InputBindingCollection(this) { Active = IsAttachedToRoot }); }
		}

		/// <summary>
		///     Gets the window the UI element is contained in or null if it isn't contained in any window.
		/// </summary>
		internal Window ParentWindow
		{
			get { return GetParentWindow(this); }
		}

		/// <summary>
		///     A value indicating whether the UI element uses and implicitly set style.
		/// </summary>
		private bool UsesImplicitStyle
		{
			get { return (_state & State.UsesImplicitStyle) == State.UsesImplicitStyle; }
			set
			{
				if (value)
					_state |= State.UsesImplicitStyle;
				else
					_state &= ~State.UsesImplicitStyle;
			}
		}

		/// <summary>
		///     A value indicating whether the UI element's cached measured layout is dirty and needs to be updated.
		/// </summary>
		private bool IsMeasureDataDirty
		{
			get { return (_state & State.MeasureDirty) == State.MeasureDirty; }
			set
			{
				if (value)
					_state |= State.MeasureDirty;
				else
					_state &= ~State.MeasureDirty;
			}
		}

		/// <summary>
		///     A value indicating whether the UI element's cached arranged layout is dirty and needs to be updated.
		/// </summary>
		private bool IsArrangeDataDirty
		{
			get { return (_state & State.ArrangeDirty) == State.ArrangeDirty; }
			set
			{
				if (value)
					_state |= State.ArrangeDirty;
				else
					_state &= ~State.ArrangeDirty;
			}
		}
	}
}