namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;

	/// <summary>
	///     A base class for UI elements that decorate a single child element.
	/// </summary>
	public abstract class Decorator : UIElement
	{
		/// <summary>
		///     The UI element that is decorated.
		/// </summary>
		public static readonly DependencyProperty<UIElement> ChildProperty =
			new DependencyProperty<UIElement>(affectsMeasure: true, prohibitsAnimations: true);

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Decorator()
		{
			ChildProperty.Changed += OnChildChanged;
		}

		/// <summary>
		///     Gets or sets the UI element that is decorated.
		/// </summary>
		public UIElement Child
		{
			get { return GetValue(ChildProperty); }
			set { SetValue(ChildProperty, value); }
		}

		/// <summary>
		///     Gets an enumerator that can be used to enumerate all logical children of the UI element.
		/// </summary>
		protected internal override Enumerator<UIElement> LogicalChildren
		{
			get { return Enumerator<UIElement>.FromItem(Child); }
		}

		/// <summary>
		///     Gets the number of visual children for this UI element.
		/// </summary>
		protected internal override int VisualChildrenCount
		{
			get { return Child == null ? 0 : 1; }
		}

		/// <summary>
		///     Removes the current child from the logical tree and adds the new child.
		/// </summary>
		private static void OnChildChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<UIElement> args)
		{
			var decorator = obj as Decorator;
			if (decorator == null)
				return;

			if (args.OldValue != null)
				args.OldValue.ChangeLogicalParent(null);

			if (args.NewValue == null)
				return;

			args.NewValue.ChangeLogicalParent(decorator);
			args.NewValue.VisualParent = decorator;
		}

		/// <summary>
		///     Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected internal override UIElement GetVisualChild(int index)
		{
			Assert.NotNull(Child);
			Assert.ArgumentSatisfies(index == 0, "The UI element has only one visual child.");

			return Child;
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
			if (Child == null)
				return new SizeD();

			Child.Measure(availableSize);
			return Child.DesiredSize;
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
			if (Child == null)
				return new SizeD();

			Child.Arrange(new RectangleD(0, 0, finalSize));
			return Child.RenderSize;
		}
	}
}