﻿namespace Pegasus.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Rendering;
	using Utilities;

	/// <summary>
	///     Represents a base class for templated UI elements.
	/// </summary>
	public class Control : UIElement
	{
		/// <summary>
		///     The border color of the control.
		/// </summary>
		public static readonly DependencyProperty<Color?> BorderBrushProperty = new DependencyProperty<Color?>();

		/// <summary>
		///     The padding inside a control.
		/// </summary>
		public static readonly DependencyProperty<Thickness> PaddingProperty = new DependencyProperty<Thickness>(affectsMeasure: true);

		/// <summary>
		///     The horizontal content of a control's content.
		/// </summary>
		public static readonly DependencyProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
			new DependencyProperty<HorizontalAlignment>(defaultValue: HorizontalAlignment.Center, affectsArrange: true);

		/// <summary>
		///     The vertical content of a control's content.
		/// </summary>
		public static readonly DependencyProperty<VerticalAlignment> VerticalContentAlignmentProperty =
			new DependencyProperty<VerticalAlignment>(defaultValue: VerticalAlignment.Center, affectsArrange: true);

		/// <summary>
		///     The border thickness of the control.
		/// </summary>
		public static readonly DependencyProperty<Thickness> BorderThicknessProperty =
			new DependencyProperty<Thickness>(defaultValue: new Thickness(1), affectsMeasure: true);

		/// <summary>
		///     The template that defines the control's appearance.
		/// </summary>
		public static readonly DependencyProperty<ControlTemplate> TemplateProperty =
			new DependencyProperty<ControlTemplate>(affectsMeasure: true, prohibitsAnimations: true);

		/// <summary>
		///     The foreground color of the control.
		/// </summary>
		public static readonly DependencyProperty<Color> ForegroundProperty =
			new DependencyProperty<Color>(defaultValue: new Color(0, 0, 0, 255), affectsRender: true, inherits: true);

		/// <summary>
		///     The child UI element that represents the root of this control's template. Null if no template has been created.
		/// </summary>
		private UIElement _templateRoot;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Control()
		{
			TemplateProperty.Changed += OnTemplateChanged;
		}

		/// <summary>
		///     Gets or sets the padding inside the control, provided that the control's template uses the control's padding as a
		///     parameter.
		/// </summary>
		public Thickness Padding
		{
			get { return GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}

		/// <summary>
		///     Gets or sets the horizontal alignment of the control's content, provided that the control's template uses
		///     the control's horizontal content alignment as a parameter.
		/// </summary>
		public HorizontalAlignment HorizontalContentAlignment
		{
			get { return GetValue(HorizontalContentAlignmentProperty); }
			set { SetValue(HorizontalContentAlignmentProperty, value); }
		}

		/// <summary>
		///     Gets or sets the vertical alignment of the control's content, provided that the control's template uses
		///     the control's vertical content alignment as a parameter.
		/// </summary>
		public VerticalAlignment VerticalContentAlignment
		{
			get { return GetValue(VerticalContentAlignmentProperty); }
			set { SetValue(VerticalContentAlignmentProperty, value); }
		}

		/// <summary>
		///     Gets or sets the border color of the control, provided that the control's template uses the control's border brush as a
		///     parameter.
		/// </summary>
		public Color? BorderBrush
		{
			get { return GetValue(BorderBrushProperty); }
			set { SetValue(BorderBrushProperty, value); }
		}

		/// <summary>
		///     Gets or sets the border thickness of the control, provided that the control's template uses the control's border
		///     thickness as a parameter.
		/// </summary>
		public Thickness BorderThickness
		{
			get { return GetValue(BorderThicknessProperty); }
			set { SetValue(BorderThicknessProperty, value); }
		}

		/// <summary>
		///     Gets or sets the foreground color of the control.
		/// </summary>
		public Color Foreground
		{
			get { return GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}

		/// <summary>
		///     Gets or sets the template that defines the control's appearance.
		/// </summary>
		public ControlTemplate Template
		{
			get { return GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}

		/// <summary>
		///     Gets the number of visual children for this visual.
		/// </summary>
		protected internal override int VisualChildrenCount
		{
			get { return _templateRoot == null ? 0 : 1; }
		}

		/// <summary>
		///     Gets an enumerator that can be used to enumerate all logical children of the UI element.
		/// </summary>
		protected internal override sealed Enumerator<UIElement> LogicalChildren
		{
			get { return Enumerator<UIElement>.FromItemOrEmpty(_templateRoot); }
		}

		/// <summary>
		///     Changes the control's template root.
		/// </summary>
		private static void OnTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<ControlTemplate> args)
		{
			var control = obj as Control;
			if (control == null)
				return;

			if (control._templateRoot != null)
				control._templateRoot.ChangeLogicalParent(null);

			if (args.NewValue == null)
				control._templateRoot = null;
			else
				control._templateRoot = args.NewValue(control);

			if (control._templateRoot != null)
			{
				control._templateRoot.ChangeLogicalParent(control);
				control._templateRoot.VisualParent = control;
			}

			control.OnVisualChildrenChanged();
			control.OnTemplateChanged(control._templateRoot);
		}

		/// <summary>
		///     Invoked when the template has been changed.
		/// </summary>
		/// <param name="templateRoot">The new root element of the template.</param>
		protected virtual void OnTemplateChanged(UIElement templateRoot)
		{
		}

		/// <summary>
		///     Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected internal override sealed UIElement GetVisualChild(int index)
		{
			Assert.NotNull(_templateRoot);
			Assert.ArgumentSatisfies(index == 0, "The UI element has only one visual child.");

			return _templateRoot;
		}

		/// <summary>
		///     Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="availableSize">
		///     The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///     to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///     element might be able to use scrolling in this case.
		/// </param>
		protected override Size MeasureCore(Size availableSize)
		{
			if (_templateRoot == null)
				return new Size();

			_templateRoot.Measure(availableSize);
			return _templateRoot.DesiredSize;
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
		protected override Size ArrangeCore(Size finalSize)
		{
			if (_templateRoot == null)
				return new Size();

			_templateRoot.Arrange(new Rectangle(0, 0, finalSize));
			return _templateRoot.RenderSize;
		}
	}
}