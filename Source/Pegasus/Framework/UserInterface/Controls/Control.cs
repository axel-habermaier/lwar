﻿namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Rendering;

	/// <summary>
	///     Represents a base class for templated UI elements.
	/// </summary>
	public class Control : UIElement
	{
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
			get { return Enumerator<UIElement>.FromItem(_templateRoot); }
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
				control._templateRoot.ChangeLogicalParent(control);

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
		protected override SizeD MeasureCore(SizeD availableSize)
		{
			if (_templateRoot == null)
				return new SizeD();

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
		protected override SizeD ArrangeCore(SizeD finalSize)
		{
			if (_templateRoot == null)
				return new SizeD();

			_templateRoot.Arrange(new RectangleD(0, 0, finalSize));
			return _templateRoot.RenderSize;
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			var width = (int)Math.Round(ActualWidth);
			var height = (int)Math.Round(ActualHeight);
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);

			spriteBatch.Draw(new Rectangle(x, y, width, height), Texture2D.White, Background);
		}
	}
}