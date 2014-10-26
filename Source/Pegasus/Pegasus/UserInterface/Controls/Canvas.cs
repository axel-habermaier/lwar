namespace Pegasus.UserInterface.Controls
{
	using System;
	using Math;
	using Utilities;

	/// <summary>
	///     Represents a layout where all child elements can position themselves with coordinates relative to the canvas.
	/// </summary>
	public class Canvas : Panel
	{
		/// <summary>
		///     The distance between the left side of an UI element and and the left side of its parent Canvas.
		/// </summary>
		public static readonly DependencyProperty<float> LeftProperty =
			new DependencyProperty<float>(defaultValue: Single.NaN, affectsArrange: true, validationCallback: ValidatePosition);

		/// <summary>
		///     The distance between the right side of an UI element and the right side of its parent Canvas.
		///     If the Left property is also set, the Right value is ignored.
		/// </summary>
		public static readonly DependencyProperty<float> RightProperty =
			new DependencyProperty<float>(defaultValue: Single.NaN, affectsArrange: true, validationCallback: ValidatePosition);

		/// <summary>
		///     The distance between the top of an UI element and the top of its parent Canvas.
		/// </summary>
		public static readonly DependencyProperty<float> TopProperty =
			new DependencyProperty<float>(defaultValue: Single.NaN, affectsArrange: true, validationCallback: ValidatePosition);

		/// <summary>
		///     The distance between the bottom of an UI element and the bottom of its parent Canvas.
		///     If the Top property is also set, the Bottom value is ignored.
		/// </summary>
		public static readonly DependencyProperty<float> BottomProperty =
			new DependencyProperty<float>(defaultValue: Single.NaN, affectsArrange: true, validationCallback: ValidatePosition);

		/// <summary>
		///     Gets the distance between the left side of the given UI element and its parent Canvas.
		/// </summary>
		/// <param name="element">The element the distance should be returned for.</param>
		public static float GetLeft(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(LeftProperty);
		}

		/// <summary>
		///     Sets the distance between the left side of the given UI element and its parent Canvas.
		/// </summary>
		/// <param name="element">The element the distance should be set for.</param>
		/// <param name="distance">The distance that should be set.</param>
		public static void SetLeft(UIElement element, float distance)
		{
			Assert.ArgumentNotNull(element);
			element.SetValue(LeftProperty, distance);
		}

		/// <summary>
		///     Gets the distance between the right side of the given UI element and its parent Canvas.
		/// </summary>
		/// <param name="element">The element the distance should be returned for.</param>
		public static float GetRight(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(RightProperty);
		}

		/// <summary>
		///     Sets the distance between the right side of the given UI element and its parent Canvas.
		/// </summary>
		/// <param name="element">The element the distance should be set for.</param>
		/// <param name="distance">The distance that should be set.</param>
		public static void SetRight(UIElement element, float distance)
		{
			Assert.ArgumentNotNull(element);
			element.SetValue(RightProperty, distance);
		}

		/// <summary>
		///     Gets the distance between the top of the given UI element and its parent Canvas.
		/// </summary>
		/// <param name="element">The element the distance should be returned for.</param>
		public static float GetTop(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(TopProperty);
		}

		/// <summary>
		///     Sets the distance between the left side of the given UI element and its parent Canvas.
		/// </summary>
		/// <param name="element">The element the distance should be set for.</param>
		/// <param name="distance">The distance that should be set.</param>
		public static void SetTop(UIElement element, float distance)
		{
			Assert.ArgumentNotNull(element);
			element.SetValue(TopProperty, distance);
		}

		/// <summary>
		///     Gets the distance between the bottom of the given UI element and its parent Canvas.
		/// </summary>
		/// <param name="element">The element the distance should be returned for.</param>
		public static float GetBottom(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(BottomProperty);
		}

		/// <summary>
		///     Sets the distance between the bottom of the given UI element and its parent Canvas.
		/// </summary>
		/// <param name="element">The element the distance should be set for.</param>
		/// <param name="distance">The distance that should be set.</param>
		public static void SetBottom(UIElement element, float distance)
		{
			Assert.ArgumentNotNull(element);
			element.SetValue(BottomProperty, distance);
		}

		/// <summary>
		///     Checks whether the given value is a valid canvas position.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		private static bool ValidatePosition(float value)
		{
			// NaN as well as all positive/negative values are supported except for infinity
			return !Single.IsInfinity(value);
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
			// The canvas does not restrict the size its children
			var size = new Size(Single.PositiveInfinity, Single.PositiveInfinity);

			foreach (var child in Children)
				child.Measure(size);

			// The canvas itself consumes no space
			return new Size();
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
			// Arrange all children by checking the values of their Left, Right, Top, and Bottom properties
			foreach (var child in Children)
			{
				var x = 0.0f;
				var y = 0.0f;

				// Check if Left is set to a valid value (and if so, ignore Right)
				var left = child.GetValue(LeftProperty);
				if (!Single.IsNaN(left))
					x = left;
				else
				{
					// Else, check Right; if Right is also invalid, just position the element to the left of the canvas
					var right = child.GetValue(RightProperty);
					if (!Single.IsNaN(right))
						x = finalSize.Width - child.DesiredSize.Width - right;
				}

				// Check if Top is set to a valid value (and if so, ignore Bottom)
				var top = child.GetValue(TopProperty);
				if (!Single.IsNaN(top))
					y = top;
				else
				{
					// Else, check Bottom; if Bottom is also invalid, just position the element to the top of the canvas
					var bottom = child.GetValue(BottomProperty);
					if (!Single.IsNaN(bottom))
						y = finalSize.Height - child.DesiredSize.Height - bottom;
				}

				child.Arrange(new Rectangle(x, y, child.DesiredSize));
			}

			return finalSize;
		}
	}
}