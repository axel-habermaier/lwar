using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	using Math;
	using Rendering;

	/// <summary>
	///   Represents a base class for templated UI elements.
	/// </summary>
	public abstract class Control : UIElement
	{
		/// <summary>
		///   The template that defines the control's appearance.
		/// </summary>
		public static readonly DependencyProperty<ControlTemplate> TemplateProperty =
			new DependencyProperty<ControlTemplate>(affectsMeasure: true, prohibitsAnimations: true);

		/// <summary>
		///   The child UI element that represents the root of this control's template. Null if no template has been created.
		/// </summary>
		private UIElement _templateRoot;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected Control()
		{
			AddChangedHandler(TemplateProperty, OnTemplateChanged);
		}

		/// <summary>
		///   Gets or sets the template that defines the control's appearance.
		/// </summary>
		public ControlTemplate Template
		{
			get { return GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}

		/// <summary>
		///   Gets the number of visual children for this visual.
		/// </summary>
		protected internal override int VisualChildrenCount
		{
			get { return _templateRoot == null ? 0 : 1; }
		}

		/// <summary>
		///   Gets an enumerator that can be used to enumerate all logical children of the UI element.
		/// </summary>
		protected internal override UIElementCollection.Enumerator LogicalChildren
		{
			get
			{
				if (_templateRoot == null)
					return UIElementCollection.Enumerator.Empty;

				return UIElementCollection.Enumerator.FromElement(_templateRoot);
			}
		}

		/// <summary>
		///   Changes the control's template root.
		/// </summary>
		private void OnTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<ControlTemplate> args)
		{
			if (_templateRoot != null)
				_templateRoot.ChangeLogicalParent(null);

			if (args.NewValue == null)
				_templateRoot = null;
			else
				_templateRoot = args.NewValue.Instantiate(this);

			if (_templateRoot != null)
				_templateRoot.ChangeLogicalParent(this);
		}

		/// <summary>
		///   Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected internal override Visual GetVisualChild(int index)
		{
			Assert.NotNull(_templateRoot);
			Assert.ArgumentSatisfies(index == 0, "The UI element has only one visual child.");

			return _templateRoot;
		}

		/// <summary>
		///   Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="constraint">
		///   The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///   to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///   element might be able to use scrolling in this case.
		/// </param>
		protected override SizeD MeasureCore(SizeD constraint)
		{
			if (_templateRoot == null)
				return new SizeD();

			_templateRoot.Measure(constraint);
			return _templateRoot.DesiredSize;
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
		protected override SizeD ArrangeCore(SizeD finalSize)
		{
			if (_templateRoot == null)
				return new SizeD();

			_templateRoot.Arrange(new RectangleD(0, 0, finalSize));
			return _templateRoot.RenderSize;
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
		}
	}
}